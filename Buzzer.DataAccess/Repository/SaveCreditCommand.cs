using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveCreditCommand : CommandBase
   {
      private readonly CreditInfo _credit;

      public SaveCreditCommand(DbConnection connection, DbTransaction transaction, CreditInfo credit)
         : base(connection, transaction)
      {
         Check.NotNull(credit, "credit");
         _credit = credit;
      }

      public void Execute()
      {
         if (_credit.IsNew)
            saveNewCredit();
         else
            saveEditedCredit();
      }

      private void saveNewCredit()
      {
         int creditId = insertCredit(_credit);
         InsertPersonResult insertBorrowerResult = insertPerson(_credit.Borrower, creditId);
         ReadOnlyCollection<PersonInfo> guarantors = _credit.Guarantors;
         var insertGuarantorsResult = new InsertPersonResult[guarantors.Count];

         for (int i = 0; i < guarantors.Count; i++)
            insertGuarantorsResult[i] = insertPerson(guarantors[i], creditId);

         PaymentInfo[] paymentsSchedule = _credit.PaymentsSchedule;
         int[] paymentsIds = insertPayments(paymentsSchedule, creditId);

         int[] todoItemsIds = insertTodoItems(_credit.TodoList, creditId);
         int[] requiredDocumentsIds = insertRequiredDocuments(creditId);

         _credit.Id = creditId;
         fillPersonIds(_credit.Borrower, insertBorrowerResult, creditId);

         for (int i = 0; i < guarantors.Count; i++)
            fillPersonIds(guarantors[i], insertGuarantorsResult[i], creditId);

         fillPaymentsIds(paymentsIds);
         fillTodoItemsIds(todoItemsIds, creditId);
         fillRequiredDocumentsIds(requiredDocumentsIds, creditId);
      }

      private void saveEditedCredit()
      {
         var command = new SelectCreditsCommand(Connection, Transaction, "ID = " + _credit.Id);
         CreditInfo original = command.Execute().Single();

         updateCredit(_credit);
         updatePerson(_credit.Borrower);
         mergePhoneNumbers(_credit.Borrower.PhoneNumbers, original.Borrower.PhoneNumbers);

         var result = new List<InsertPersonResult>();

         foreach (var guarantor in _credit.Guarantors)
         {
            if (guarantor.IsNew)
            {
               result.Add(insertPerson(guarantor, _credit.Id));
            }
            else
            {
               var originalGuarantor = original.Guarantors.First(item => item.Id == guarantor.Id);
               updatePerson(guarantor);
               mergePhoneNumbers(guarantor.PhoneNumbers, originalGuarantor.PhoneNumbers);
            }
         }

         List<PersonInfo> deletedGuarantors =
            original.Guarantors.Where(o => _credit.Guarantors.All(c => c.Id != o.Id)).ToList();
         deletedGuarantors.ForEach(
            item =>
               {
                  foreach (var phone in item.PhoneNumbers)
                     deletePhoneNumber(phone.Id);
                  deletePerson(item.Id);
               }
            );

         PaymentInfo[] payments = _credit.PaymentsSchedule;

         if (payments.Length > 0 && payments[0].IsNew)
         {
            foreach (PaymentInfo payment in original.PaymentsSchedule)
               deletePaymentInfo(payment.Id);

            int[] paymentsIds = insertPayments(payments, _credit.Id);
            fillPaymentsIds(paymentsIds);
         }

         saveTodoList(_credit.TodoList, original.TodoList);
         saveRequiredDocuments(original.RequiredDocuments);

         int i = 0;

         foreach (var guarantor in _credit.Guarantors)
         {
            if (guarantor.IsNew)
            {
               fillPersonIds(guarantor, result[i++], _credit.Id);
            }
         }
      }

      private InsertPersonResult insertPerson(PersonInfo person, int creditId)
      {
         int personId = insertPersonQuery(person, creditId);
         ReadOnlyCollection<PhoneNumberInfo> phoneNumbers = person.PhoneNumbers;
         var phoneNumbersIds = new int[phoneNumbers.Count];

         for (int i = 0; i < phoneNumbers.Count; i++)
            phoneNumbersIds[i] = insertPhoneNumber(phoneNumbers[i], personId);

         return new InsertPersonResult(personId, phoneNumbersIds);
      }

      private void fillPersonIds(PersonInfo person, InsertPersonResult insertPersonResult, int creditId)
      {
         person.Id = insertPersonResult.PersonId;
         person.CreditId = creditId;

         for (int i = 0; i < person.PhoneNumbers.Count; i++)
         {
            PhoneNumberInfo phoneNumber = person.PhoneNumbers[i];
            phoneNumber.Id = insertPersonResult.PhoneNumberIds[i];
            phoneNumber.PersonId = person.Id;
         }
      }

      [Todo("Сделать откат ID inserted номеров телефонов, если транзакция упадет")]
      private void mergePhoneNumbers(IList<PhoneNumberInfo> current, IList<PhoneNumberInfo> original)
      {
         var insertedPhones = current.Where(c => c.IsNew).ToList();
         var updatedPhones = current.Where(c => original.Any(o => o.Id == c.Id)).ToList();
         var deletedPhones = original.Where(o => current.All(c => c.Id != o.Id)).ToList();

         insertedPhones.ForEach(item =>
                                   {
                                      int id = insertPhoneNumber(item, item.PersonId);
                                      item.Id = id;
                                   });
         updatedPhones.ForEach(updatePhoneNumber);
         deletedPhones.ForEach(item => deletePhoneNumber(item.Id));
      }

      private int[] insertPayments(PaymentInfo[] paymentsSchedule, int creditId)
      {
         var paymentsIds = new int[paymentsSchedule.Length];

         for (int i = 0; i < paymentsSchedule.Length; i++)
            paymentsIds[i] = insertPaymentInfo(paymentsSchedule[i], creditId);

         return paymentsIds;
      }

      private int[] insertTodoItems(ReadOnlyCollection<TodoItem> todoList, int creditId)
      {
         var todoItemsIds = new int[todoList.Count];

         for (int i = 0; i < todoList.Count; i++)
            todoItemsIds[i] = insertTodoItem(todoList[i], creditId);

         return todoItemsIds;
      }

      private int[] insertRequiredDocuments(int creditId)
      {
         ReadOnlyCollection<RequiredDocument> requiredDocuments = _credit.RequiredDocuments;
         var requiredDocumentsIds = new int[requiredDocuments.Count];

         for (int i = 0; i < requiredDocuments.Count; i++)
            requiredDocumentsIds[i] = insertRequiredDocument(requiredDocuments[i], creditId);

         return requiredDocumentsIds;
      }

      private void fillPaymentsIds(int[] paymentsIds)
      {
         for (int i = 0; i < paymentsIds.Length; i++)
            _credit.PaymentsSchedule[i].Id = paymentsIds[i];
      }

      private void fillTodoItemsIds(int[] todoItemsIds, int creditId)
      {
         for (int i = 0; i < todoItemsIds.Length; i++)
         {
            _credit.TodoList[i].Id = todoItemsIds[i];
            _credit.TodoList[i].CreditId = creditId;
         }
      }

      private void fillRequiredDocumentsIds(int[] requiredDocumentsIds, int creditId)
      {
         for (int i = 0; i < requiredDocumentsIds.Length; i++)
         {
            _credit.RequiredDocuments[i].Id = requiredDocumentsIds[i];
            _credit.RequiredDocuments[i].CreditId = creditId;
         }
      }

      private void saveTodoList(ReadOnlyCollection<TodoItem> current, ReadOnlyCollection<TodoItem> original)
      {
         foreach (TodoItem todoItem in current)
         {
            if (todoItem.IsNew)
            {
               int id = insertTodoItem(todoItem, _credit.Id);
               todoItem.Id = id;
            }
            else
               updateTodoItem(todoItem);
         }

         List<TodoItem> deletedTodoItems =
            original
               .Where(o => current.All(c => c.Id != o.Id))
               .ToList();
         deletedTodoItems.ForEach(deleteTodoItem);
      }

      private void saveRequiredDocuments(ReadOnlyCollection<RequiredDocument> original)
      {
         ReadOnlyCollection<RequiredDocument> current = _credit.RequiredDocuments;

         foreach (RequiredDocument requiredDocument in current)
         {
            if (requiredDocument.IsNew)
            {
               int id = insertRequiredDocument(requiredDocument, _credit.Id);
               requiredDocument.Id = id;
            }
            else
               updateRequiredDocument(requiredDocument);
         }

         List<RequiredDocument> deletedRequiredDocuments =
            original
               .Where(o => current.All(c => c.Id != o.Id))
               .ToList();
         deletedRequiredDocuments.ForEach(deleteRequiredDocument);
      }

      #region Credits

      private int insertCredit(CreditInfo creditInfo)
      {
         string insertCreditQuery =
            string.Format(
               "INSERT INTO Credits ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}) " +
               "VALUES ({16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31});" +
               "SELECT last_insert_rowid();",

               CreditNumber.Name, ApplicationDate.Name, ProtocolDate.Name,
               CreditAmount.Name, CreditIssueDate.Name, MonthsCount.Name,
               DiscountRate.Name, EffectiveDiscountRate.Name, ExchangeRate.Name,
               CreditState.Name, RefusalReason.Name, RowState.Name,
               RequiredDocumentCreditTypeId.Name, RequiredDocumentNotificationDescription.Name,
               RequiredDocumentNotificationCount.Name, RequiredDocumentNotificationDate.Name,

               CreditNumber.ParameterName, ApplicationDate.ParameterName, ProtocolDate.ParameterName,
               CreditAmount.ParameterName, CreditIssueDate.ParameterName, MonthsCount.ParameterName,
               DiscountRate.ParameterName, EffectiveDiscountRate.ParameterName, ExchangeRate.ParameterName,
               CreditState.ParameterName, RefusalReason.ParameterName, RowState.ParameterName,
               RequiredDocumentCreditTypeId.ParameterName, RequiredDocumentNotificationDescription.ParameterName,
               RequiredDocumentNotificationCount.ParameterName, RequiredDocumentNotificationDate.ParameterName
               );

         using (DbCommand command = createCommand(insertCreditQuery))
         {
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.ApplicationDate, ApplicationDate);
            command.AddParameter(creditInfo.ProtocolDate, ProtocolDate);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            command.AddParameter((int) creditInfo.CreditState, CreditState);
            command.AddParameter(creditInfo.RefusalReason, RefusalReason);
            command.AddParameter((int) creditInfo.RowState, RowState);
            command.AddParameter(
               creditInfo.CreditType == null ? null : (int?) creditInfo.CreditType.Id,
               RequiredDocumentCreditTypeId
               );
            command.AddParameter(creditInfo.NotificationDescription, RequiredDocumentNotificationDescription);
            command.AddParameter(creditInfo.NotificationCount, RequiredDocumentNotificationCount);
            command.AddParameter(creditInfo.NotificationDate, RequiredDocumentNotificationDate);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateCredit(CreditInfo creditInfo)
      {
         string updateCreditQuery =
            string.Format(
               "UPDATE Credits SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13}, {14}={15}, {16}={17}, {18}={19}, {20}={21}, {22}={23}, " +
               "{24}={25}, {26}={27}, {28}={29}, {30}={31} " +
               "WHERE {32}={33};",
               CreditNumber.Name, CreditNumber.ParameterName,
               ApplicationDate.Name, ApplicationDate.ParameterName,
               ProtocolDate.Name, ProtocolDate.ParameterName,
               CreditAmount.Name, CreditAmount.ParameterName,
               CreditIssueDate.Name, CreditIssueDate.ParameterName,
               MonthsCount.Name, MonthsCount.ParameterName,
               DiscountRate.Name, DiscountRate.ParameterName,
               EffectiveDiscountRate.Name, EffectiveDiscountRate.ParameterName,
               ExchangeRate.Name, ExchangeRate.ParameterName,
               CreditState.Name, CreditState.ParameterName,
               RefusalReason.Name, RefusalReason.ParameterName,
               RowState.Name, RowState.ParameterName,
               RequiredDocumentCreditTypeId.Name, RequiredDocumentCreditTypeId.ParameterName,
               RequiredDocumentNotificationDescription.Name, RequiredDocumentNotificationDescription.ParameterName,
               RequiredDocumentNotificationCount.Name, RequiredDocumentNotificationCount.ParameterName,
               RequiredDocumentNotificationDate.Name, RequiredDocumentNotificationDate.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updateCreditQuery))
         {
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.ApplicationDate, ApplicationDate);
            command.AddParameter(creditInfo.ProtocolDate, ProtocolDate);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            command.AddParameter((int) creditInfo.CreditState, CreditState);
            command.AddParameter(creditInfo.RefusalReason, RefusalReason);
            command.AddParameter((int) creditInfo.RowState, RowState);
            command.AddParameter(
               creditInfo.CreditType == null ? null : (int?) creditInfo.CreditType.Id,
               RequiredDocumentCreditTypeId
               );
            command.AddParameter(creditInfo.NotificationDescription, RequiredDocumentNotificationDescription);
            command.AddParameter(creditInfo.NotificationCount, RequiredDocumentNotificationCount);
            command.AddParameter(creditInfo.NotificationDate, RequiredDocumentNotificationDate);
            command.AddParameter(creditInfo.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region Persons

      private int insertPersonQuery(PersonInfo personInfo, int creditId)
      {
         string insertPersonQuery =
            string.Format(
               "INSERT INTO Persons ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) VALUES ({9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, PersonalNumber.Name, Name.Name, RegistrationAddress.Name, FactAddress.Name,
               PassportNumber.Name, PassportIssueDate.Name, PassportIssuer.Name, PersonType.Name,

               CreditId.ParameterName, PersonalNumber.ParameterName, Name.ParameterName,
               RegistrationAddress.ParameterName, FactAddress.ParameterName,
               PassportNumber.ParameterName, PassportIssueDate.ParameterName, PassportIssuer.ParameterName,
               PersonType.ParameterName
               );

         using (DbCommand command = createCommand(insertPersonQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, PersonType);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updatePerson(PersonInfo personInfo)
      {
         var updatePersonQuery =
            string.Format(
               "UPDATE Persons SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13}, {14}={15} WHERE {16}={17};",
               PersonalNumber.Name, PersonalNumber.ParameterName,
               Name.Name, Name.ParameterName,
               RegistrationAddress.Name, RegistrationAddress.ParameterName,
               FactAddress.Name, FactAddress.ParameterName,
               PassportNumber.Name, PassportNumber.ParameterName,
               PassportIssueDate.Name, PassportIssueDate.ParameterName,
               PassportIssuer.Name, PassportIssuer.ParameterName,
               PersonType.Name, PersonType.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = createCommand(updatePersonQuery))
         {
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, PersonType);
            command.AddParameter(personInfo.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      private void deletePerson(int personId)
      {
         string deletePersonQuery =
            string.Format(
               "DELETE FROM Persons WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (var command = createCommand(deletePersonQuery))
         {
            command.AddParameter(personId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region PhoneNumbers

      private int insertPhoneNumber(PhoneNumberInfo phoneNumber, int personId)
      {
         string insertPhoneNumberQuery =
            string.Format(
               "INSERT INTO PhoneNumbers ({0}, {1}) VALUES ({2}, {3});" +
               "SELECT last_insert_rowid();",
               PersonId.Name, PhoneNumber.Name,
               PersonId.ParameterName, PhoneNumber.ParameterName
               );

         using (DbCommand command = createCommand(insertPhoneNumberQuery))
         {
            command.AddParameter(personId, PersonId);
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updatePhoneNumber(PhoneNumberInfo phoneNumber)
      {
         string updatePhoneNumberQuery =
            string.Format(
               "UPDATE PhoneNumbers SET {0}={1} WHERE {2}={3};",
               PhoneNumber.Name, PhoneNumber.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updatePhoneNumberQuery))
         {
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            command.AddParameter(phoneNumber.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      private void deletePhoneNumber(int phoneNumberId)
      {
         string deletePhoneNumberQuery =
            string.Format(
               "DELETE FROM PhoneNumbers WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(deletePhoneNumberQuery))
         {
            command.AddParameter(phoneNumberId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region PaymentsSchedule

      private int insertPaymentInfo(PaymentInfo paymentInfo, int creditId)
      {
         string insertPaymentInfoQuery =
            string.Format(
               "INSERT INTO PaymentsSchedule ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, PaymentAmount.Name,
               PaymentDate.Name, IsNotified.Name,

               CreditId.ParameterName, PaymentAmount.ParameterName,
               PaymentDate.ParameterName, IsNotified.ParameterName
               );

         using (DbCommand command = createCommand(insertPaymentInfoQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(paymentInfo.PaymentAmount, PaymentAmount);
            command.AddParameter(paymentInfo.PaymentDate, PaymentDate);
            command.AddParameter(paymentInfo.IsNotified, IsNotified);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void deletePaymentInfo(int paymentId)
      {
         string deletePaymentInfoQuery =
            string.Format(
               "DELETE FROM PaymentsSchedule WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(deletePaymentInfoQuery))
         {
            command.AddParameter(paymentId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region TodoList

      private int insertTodoItem(TodoItem todoItem, int creditId)
      {
         string insertTodoItemQuery =
            string.Format(
               "INSERT INTO TodoItems ({0}, {1}, {2}, {3}, {4}) VALUES ({5}, {6}, {7}, {8}, {9});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, TodoItemDescription.Name, TodoItemState.Name,
               TodoItemNotificationCount.Name, TodoItemNotificationDate.Name,

               CreditId.ParameterName, TodoItemDescription.ParameterName, TodoItemState.ParameterName,
               TodoItemNotificationCount.ParameterName, TodoItemNotificationDate.ParameterName
               );

         using (DbCommand command = createCommand(insertTodoItemQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(todoItem.Description, TodoItemDescription);
            command.AddParameter(todoItem.State, TodoItemState);
            command.AddParameter(todoItem.NotificationCount, TodoItemNotificationCount);
            command.AddParameter(todoItem.NotificationDate, TodoItemNotificationDate);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateTodoItem(TodoItem todoItem)
      {
         string updateTodoItemQuery =
            string.Format(
               "UPDATE TodoItems SET {0}={1}, {2}={3}, {4}={5}, {6}={7} WHERE {8}={9};",
               TodoItemDescription.Name, TodoItemDescription.ParameterName,
               TodoItemState.Name, TodoItemState.ParameterName,
               TodoItemNotificationCount.Name, TodoItemNotificationCount.ParameterName,
               TodoItemNotificationDate.Name, TodoItemNotificationDate.ParameterName,
               Id.Name, Id.ParameterName
               );
         
         using (DbCommand command = createCommand(updateTodoItemQuery))
         {
            command.AddParameter(todoItem.Description, TodoItemDescription);
            command.AddParameter(todoItem.State, TodoItemState);
            command.AddParameter(todoItem.NotificationCount, TodoItemNotificationCount);
            command.AddParameter(todoItem.NotificationDate, TodoItemNotificationDate);
            command.AddParameter(todoItem.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      private void deleteTodoItem(TodoItem todoItem)
      {
         string deleteTodoItemQuery =
            string.Format("DELETE FROM TodoItems WHERE {0}={1};", Id.Name, Id.ParameterName);

         using (DbCommand command = createCommand(deleteTodoItemQuery))
         {
            command.AddParameter(todoItem.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region RequiredDocuments

      private int insertRequiredDocument(RequiredDocument requiredDocument, int creditId)
      {
         string insertRequiredDocumentQuery =
            string.Format(
               "INSERT INTO RequiredDocuments ({0}, {1}, {2}) VALUES ({3}, {4}, {5});" +
               "SELECT last_insert_rowid();",
               CreditId.Name, RequiredDocumentType.Name, RequiredDocumentState.Name,
               CreditId.ParameterName, RequiredDocumentType.ParameterName, RequiredDocumentState.ParameterName
               );

         using (DbCommand command = createCommand(insertRequiredDocumentQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(requiredDocument.DocumentType.Id, RequiredDocumentType);
            command.AddParameter((int) requiredDocument.State, RequiredDocumentState);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateRequiredDocument(RequiredDocument requiredDocument)
      {
         string updateRequiredDocumentQuery =
            string.Format(
               "UPDATE RequiredDocuments SET {0}={1} WHERE {2}={3};",
               RequiredDocumentState.Name, RequiredDocumentState.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updateRequiredDocumentQuery))
         {
            command.AddParameter((int) requiredDocument.State, RequiredDocumentState);
            command.AddParameter(requiredDocument.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      private void deleteRequiredDocument(RequiredDocument requiredDocument)
      {
         string deleteRequiredDocument =
            string.Format(
               "DELETE FROM RequiredDocuments WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(deleteRequiredDocument))
         {
            command.AddParameter(requiredDocument.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      private sealed class InsertPersonResult
      {
         public InsertPersonResult(int personId, int[] phoneNumberIds)
         {
            PersonId = personId;
            PhoneNumberIds = phoneNumberIds;
         }

         public int PersonId { get; private set; }
         public int[] PhoneNumberIds { get; private set; }
      }
   }
}