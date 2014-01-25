using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal sealed class SelectCreditsCommand : CommandBase
   {
      private readonly string _condition;

      public SelectCreditsCommand(SqlConnection connection, SqlTransaction transaction, string condition = null)
         : base(connection, transaction)
      {
         _condition = condition;
      }

      public CreditInfo[] Execute()
      {
         return execute<CreditInfo[]>(selectCredits);
      }

      private CreditInfo[] selectCredits()
      {
         string selectCreditsQuery;

         if (string.IsNullOrEmpty(_condition))
            selectCreditsQuery = string.Format("SELECT * FROM Credits");
         else
            selectCreditsQuery = string.Format("SELECT * FROM Credits WHERE {0}", _condition);

         using (SqlCommand command = createCommand(selectCreditsQuery))
         {
            using (var credits = new DataTable())
            {
               using (SqlDataReader reader = command.ExecuteReader())
                  credits.Load(reader);

               var result = new List<CreditInfo>();

               foreach (DataRow row in credits.Rows)
               {
                  int creditId = Convert.ToInt32(row[Id.Name]);
                  QueryPersonInfoResult queryResult = getPersons(creditId);

                  result.Add(
                     CreditInfo.Create(
                        creditId,
                        Convert.ToString(row[CreditNumber.Name]),
                        Convert.ToDecimal(row[CreditAmount.Name]),
                        Convert.ToDateTime(row[CreditIssueDate.Name]),
                        Convert.ToInt32(row[MonthsCount.Name]),
                        Convert.ToDecimal(row[DiscountRate.Name]),
                        get(row[EffectiveDiscountRate.Name], Convert.ToDecimal),
                        get(row[ExchangeRate.Name], Convert.ToDecimal),
                        queryResult.Borrower,
                        queryResult.Guarantors
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      private QueryPersonInfoResult getPersons(int creditId)
      {
         string selectPersonsQuery =
            string.Format("SELECT * FROM Persons WHERE {0} = {1}", CreditId.Name, creditId);

         PersonInfo borrower = null;
         var result = new List<PersonInfo>();

         using (SqlCommand command = createCommand(selectPersonsQuery))
         {
            using (var persons = new DataTable())
            {
               using (SqlDataReader reader = command.ExecuteReader())
                  persons.Load(reader);

               foreach (DataRow row in persons.Rows)
               {
                  int personId = Convert.ToInt32(row[Id.Name]);
                  bool isBorrower = Convert.ToBoolean(row[IsBorrower.Name]);

                  PersonInfo personInfo =
                     PersonInfo.Create(
                        personId,
                        Convert.ToInt32(row[CreditId.Name]),
                        Convert.ToString(row[PersonalNumber.Name]),
                        Convert.ToString(row[Name.Name]),
                        Convert.ToString(row[RegistrationAddress.Name]),
                        Convert.ToString(row[FactAddress.Name]),
                        Convert.ToString(row[PassportNumber.Name]),
                        Convert.ToDateTime(row[PassportIssueDate.Name]),
                        Convert.ToString(row[PassportIssuer.Name]),
                        isBorrower,
                        getPhoneNumbers(personId)
                        );

                  if (isBorrower)
                     borrower = personInfo;
                  else
                     result.Add(personInfo);
               }
            }
         }

         return new QueryPersonInfoResult(borrower, result);
      }

      private IEnumerable<PhoneNumberInfo> getPhoneNumbers(int personId)
      {
         string selectPhoneNumbersQuery =
            string.Format("SELECT * FROM PhoneNumbers WHERE {0} = {1}", PersonId.Name, personId);

         using (SqlCommand command = createCommand(selectPhoneNumbersQuery))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               var result = new List<PhoneNumberInfo>();

               while (reader.Read())
               {
                  result.Add(
                     PhoneNumberInfo.Create(
                        Convert.ToInt32(reader[Id.Name]),
                        Convert.ToInt32(reader[PersonId.Name]),
                        Convert.ToString(reader[PhoneNumber.Name])
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      private sealed class QueryPersonInfoResult
      {
         public QueryPersonInfoResult(PersonInfo borrower, IEnumerable<PersonInfo> guarantors)
         {
            Check.NotNull(borrower, "borrower");
            Check.NotNull(guarantors, "guarantors");
            Borrower = borrower;
            Guarantors = guarantors;
         }

         public PersonInfo Borrower { get; private set; }
         public IEnumerable<PersonInfo> Guarantors { get; private set; }
      }
   }
}