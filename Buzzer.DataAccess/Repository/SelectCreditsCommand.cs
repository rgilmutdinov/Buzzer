using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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

               Dictionary<int, CreditType> creditTypesById =
                  getCreditTypes().ToDictionary(item => item.Id);
               Dictionary<int, DocumentType> documentTypesById =
                  getDocumentTypes().ToDictionary(item => item.Id);

               var result = new List<CreditInfo>();

               foreach (DataRow row in credits.Rows)
               {
                  int creditId = Convert.ToInt32(row[Id.Name]);

                  CreditType creditType =
                     getCreditType(creditTypesById,
                                   getNullable(row[RequiredDocumentCreditTypeId.Name], Convert.ToInt32));

                  QueryPersonInfoResult queryResult = getPersons(creditId);
                  IEnumerable<PaymentInfo> payments = getPayments(creditId);
                  IEnumerable<TodoItem> todoList = getTodoList(creditId);
                  IEnumerable<RequiredDocument> requiredDocuments = getRequiredDocuments(creditId, documentTypesById);

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
                        getRowState(Convert.ToInt32(row[RowState.Name])),
                        queryResult.Borrower,
                        creditType,
                        get(row[RequiredDocumentNotificationDescription.Name], Convert.ToString),
                        Convert.ToInt32(row[RequiredDocumentNotificationCount.Name]),
                        getNullable(row[RequiredDocumentNotificationDate.Name], Convert.ToDateTime),
                        queryResult.Guarantors,
                        payments,
                        todoList,
                        requiredDocuments
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      private IEnumerable<CreditType> getCreditTypes()
      {
         var selectCommand = new SelectCreditTypesCommand(Connection, Transaction);
         return selectCommand.Execute();
      }

      private IEnumerable<DocumentType> getDocumentTypes()
      {
         var selectCommand = new SelectDocumentTypesCommand(Connection, Transaction);
         return selectCommand.Execute();
      } 

      private CreditState getCreditState(int creditState)
      {
         return (CreditState) creditState;
      }

      private RowState getRowState(int rowState)
      {
         return (RowState) rowState;
      }

      private CreditType getCreditType(Dictionary<int, CreditType> creditTypesById, int? id)
      {
         if (!id.HasValue)
            return null;

         return creditTypesById[id.Value];
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

      private IEnumerable<TodoItem> getTodoList(int creditId)
      {
         string selectTodoItemsQuery =
            string.Format("SELECT * FROM TodoItems WHERE {0} = {1};", CreditId.Name, creditId);

         using (DbCommand command = createCommand(selectTodoItemsQuery))
         {
            using (DbDataReader reader = command.ExecuteReader())
            {
               var result = new List<TodoItem>();

               while (reader.Read())
               {
                  result.Add(
                     TodoItem.Create(
                        Convert.ToInt32(reader[Id.Name]),
                        Convert.ToInt32(reader[CreditId.Name]),
                        Convert.ToString(reader[TodoItemDescription.Name]),
                        getTodoItemState(Convert.ToInt32(reader[TodoItemState.Name])),
                        Convert.ToInt32(reader[TodoItemNotificationCount.Name]),
                        getNullable(reader[TodoItemNotificationDate.Name], Convert.ToDateTime)
                        )
                     );
               }

               return result;
            }
         }
      }

      private TodoItemState getTodoItemState(int todoItemState)
      {
         return (TodoItemState) todoItemState;
      }

      private IEnumerable<RequiredDocument> getRequiredDocuments(int creditId, Dictionary<int, DocumentType> documentTypesById)
      {
         string selectRequiredDocuments =
            string.Format("SELECT * FROM RequiredDocuments WHERE {0} = {1};", CreditId.Name, creditId);

         using (DbCommand command = createCommand(selectRequiredDocuments))
         using (DbDataReader reader = command.ExecuteReader())
         {
            var result = new List<RequiredDocument>();

            while (reader.Read())
            {
               result.Add(
                  RequiredDocument.Create(
                     Convert.ToInt32(reader[Id.Name]),
                     Convert.ToInt32(reader[CreditId.Name]),
                     documentTypesById[Convert.ToInt32(reader[RequiredDocumentType.Name])],
                     getRequiredDocumentState(Convert.ToInt32(reader[RequiredDocumentState.Name]))
                     )
                  );
            }

            return result;
         }
      }

      private RequiredDocumentState getRequiredDocumentState(int requiredDocumentState)
      {
         return (RequiredDocumentState) requiredDocumentState;
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