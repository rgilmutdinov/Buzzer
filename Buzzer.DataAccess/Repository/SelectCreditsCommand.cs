using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal sealed class SelectCreditsCommand : CommandBase
   {
      private readonly string _condition;

      public SelectCreditsCommand(DbConnection connection, DbTransaction transaction, string condition = null)
         : base(connection, transaction)
      {
         _condition = condition;
      }

      public CreditInfo[] Execute()
      {
         return selectCredits();
      }

      private CreditInfo[] selectCredits()
      {
         string selectCreditsQuery;

         if (string.IsNullOrEmpty(_condition))
            selectCreditsQuery = string.Format("SELECT * FROM Credits");
         else
            selectCreditsQuery = string.Format("SELECT * FROM Credits WHERE {0}", _condition);

         using (DbCommand command = createCommand(selectCreditsQuery))
         {
            using (var credits = new DataTable())
            {
               using (DbDataReader reader = command.ExecuteReader())
                  credits.Load(reader);

               var result = new List<CreditInfo>();

               foreach (DataRow row in credits.Rows)
               {
                  int creditId = Convert.ToInt32(row[Id.Name]);
                  QueryPersonInfoResult queryResult = getPersons(creditId);
                  IEnumerable<PaymentInfo> payments = getPayments(creditId);

                  result.Add(
                     CreditInfo.Create(
                        creditId,
                        get(row[CreditNumber.Name], Convert.ToString),
                        getNullable(row[ApplicationDate.Name], Convert.ToDateTime),
                        getNullable(row[ProtocolDate.Name], Convert.ToDateTime),
                        Convert.ToDecimal(row[CreditAmount.Name]),
                        Convert.ToDateTime(row[CreditIssueDate.Name]),
                        Convert.ToInt32(row[MonthsCount.Name]),
                        Convert.ToDecimal(row[DiscountRate.Name]),
                        getNullable(row[EffectiveDiscountRate.Name], Convert.ToDecimal),
                        getNullable(row[ExchangeRate.Name], Convert.ToDecimal),
                        getCreditState(Convert.ToInt32(row[CreditState.Name])),
                        get(row[RefusalReason.Name], Convert.ToString),
                        queryResult.Borrower,
                        queryResult.Guarantors,
                        payments
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      private CreditState getCreditState(int creditState)
      {
         return (CreditState) creditState;
      }

      private QueryPersonInfoResult getPersons(int creditId)
      {
         string selectPersonsQuery =
            string.Format("SELECT * FROM Persons WHERE {0} = {1}", CreditId.Name, creditId);

         PersonInfo borrower = null;
         var result = new List<PersonInfo>();

         using (DbCommand command = createCommand(selectPersonsQuery))
         {
            using (var persons = new DataTable())
            {
               using (DbDataReader reader = command.ExecuteReader())
                  persons.Load(reader);

               foreach (DataRow row in persons.Rows)
               {
                  int personId = Convert.ToInt32(row[Id.Name]);
                  int personType = Convert.ToInt32(row[PersonType.Name]);
                  bool isBorrower = personType == (int) PersonTypes.Borrower;

                  PersonInfo personInfo =
                     PersonInfo.Create(
                        personId,
                        Convert.ToInt32(row[CreditId.Name]),
                        get(row[PersonalNumber.Name], Convert.ToString),
                        get(row[Name.Name], Convert.ToString),
                        get(row[RegistrationAddress.Name], Convert.ToString),
                        get(row[FactAddress.Name], Convert.ToString),
                        get(row[PassportNumber.Name], Convert.ToString),
                        Convert.ToDateTime(row[PassportIssueDate.Name]),
                        get(row[PassportIssuer.Name], Convert.ToString),
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

         using (DbCommand command = createCommand(selectPhoneNumbersQuery))
         {
            using (DbDataReader reader = command.ExecuteReader())
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

      private IEnumerable<PaymentInfo> getPayments(int creditId)
      {
         string selectPaymentsScheduleQuery =
            string.Format("SELECT * FROM PaymentsSchedule WHERE {0} = {1}", CreditId.Name, creditId);

         using (DbCommand command = createCommand(selectPaymentsScheduleQuery))
         {
            using (DbDataReader reader = command.ExecuteReader())
            {
               var result = new List<PaymentInfo>();

               while (reader.Read())
               {
                  result.Add(
                     PaymentInfo.Create(
                        Convert.ToInt32(reader[Id.Name]),
                        Convert.ToDecimal(reader[PaymentAmount.Name]),
                        Convert.ToDateTime(reader[PaymentDate.Name]),
                        Convert.ToBoolean(reader[IsNotified.Name])
                        )
                     );
               }

               return result;
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