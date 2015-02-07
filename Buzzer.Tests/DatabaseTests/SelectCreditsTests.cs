using System;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Tests.Common;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class SelectCreditsTests
   {
      private BuzzerDatabase _database;

      [TestFixtureSetUp]
      public void SetUpFixture()
      {
         _database = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SelectCreditWithoutGuarantorsTest()
      {
         // Arrange.
         const string creditNumber = "CNS1";

         // Act.
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsFalse(credit.IsNew);
         Assert.AreEqual(credit.ApplicationDate, new DateTime(2013, 12, 31));
         Assert.AreEqual(credit.ProtocolDate, new DateTime(2014, 1, 1));
         Assert.AreEqual(200000M, credit.CreditAmount);
         Assert.AreEqual(new DateTime(2014, 1, 2), credit.CreditIssueDate);
         Assert.AreEqual(24, credit.MonthsCount);
         Assert.AreEqual(0.36M, credit.DiscountRate);
         Assert.AreEqual(0.12M, credit.EffectiveDiscountRate);
         Assert.AreEqual(47.5M, credit.ExchangeRate);
         Assert.AreEqual(CreditState.Current, credit.CreditState);
         Assert.IsNull(credit.RefusalReason);
         Assert.AreEqual(RowState.Modified, credit.RowState);
         Assert.IsNull(credit.CreditType);
         Assert.IsNullOrEmpty(credit.NotificationDescription);
         Assert.AreEqual(0, credit.NotificationCount);
         Assert.IsNull(credit.NotificationDate);

         PersonInfo borrower = credit.Borrower;

         checkPersonInfo(
            borrower, false, credit.Id, "01234567890123", "Borrower of CN2", "Address",
            "Fact address", "Passport", new DateTime(2013, 12, 1), "Issuer"
            );

         Assert.IsNotNull(borrower.PhoneNumbers);
         Assert.IsEmpty(borrower.PhoneNumbers);

         Assert.IsNotNull(credit.Guarantors);
         Assert.IsEmpty(credit.Guarantors);

         Assert.IsNotNull(credit.PaymentsSchedule);
         Assert.IsEmpty(credit.PaymentsSchedule);

         Assert.IsNotNull(credit.TodoList);
         Assert.IsEmpty(credit.TodoList);
      }

      [Test]
      public void SelectCreditWithGuarantorsTest()
      {
         // Arrange.
         const string creditNumber = "CNS2";

         // Act.
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsNull(credit.ApplicationDate);
         Assert.IsNull(credit.ProtocolDate);
         Assert.IsNull(credit.EffectiveDiscountRate);
         Assert.IsNull(credit.ExchangeRate);
         Assert.AreEqual(CreditState.Repayed, credit.CreditState);
         Assert.AreEqual("Refusal reason", credit.RefusalReason);
         Assert.AreEqual(RowState.Deleted, credit.RowState);
         Assert.IsNotNull(credit.Borrower);

         Assert.IsNotNull(credit.Guarantors);
         Assert.AreEqual(2, credit.Guarantors.Count);

         {
            PersonInfo guarantor = credit.Guarantors.First();
            Assert.IsNotNull(guarantor);
            Assert.IsNotNull(guarantor.PhoneNumbers);
            Assert.IsEmpty(guarantor.PhoneNumbers);
         }

         {
            PersonInfo guarantor = credit.Guarantors.Last();

            checkPersonInfo(
               guarantor, false, credit.Id, "34567890123456", "Guarantor 2 of CN3", "Address 2",
               "Fact address 2", "Passport 2", new DateTime(2013, 12, 3), "Issuer 2"
               );

            Assert.IsNotNull(guarantor.PhoneNumbers);
            Assert.AreEqual(2, guarantor.PhoneNumbers.Count);

            checkPhoneNumberInfo(guarantor.PhoneNumbers.First(), "555666666", guarantor.Id);
            checkPhoneNumberInfo(guarantor.PhoneNumbers.Last(), "777888888", guarantor.Id);
         }

         Assert.IsNotNull(credit.PaymentsSchedule);
         Assert.IsEmpty(credit.PaymentsSchedule);

         Assert.IsNotNull(credit.TodoList);
         Assert.IsEmpty(credit.TodoList);
      }

      [Test]
      public void SelectCreditWithPaymentsScheduleTest()
      {
         // Arrange.
         const string creditNumber = "CNS3";

         // Act.
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsNotNull(credit.PaymentsSchedule);
         Assert.AreEqual(4, credit.PaymentsSchedule.Length);

         checkPaymentInfo(credit.PaymentsSchedule[0], false, 107851M, new DateTime(2014, 2, 25), true);
         checkPaymentInfo(credit.PaymentsSchedule[1], false, 107793M, new DateTime(2014, 3, 25), true);
         checkPaymentInfo(credit.PaymentsSchedule[2], false, 107734M, new DateTime(2014, 4, 25), false);
         checkPaymentInfo(credit.PaymentsSchedule[3], false, 107674M, new DateTime(2014, 5, 25), false);
      }

      [Test]
      public void SelectCreditWithTodoListTest()
      {
         // Arrange.
         const string creditNumber = "CNS4";
         
         // Act.
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsNotNull(credit.TodoList);
         Assert.AreEqual(2, credit.TodoList.Count);

         checkTodoItem(credit.TodoList[0], credit.Id, "Todo description 1", TodoItemState.None, 0, null);
         checkTodoItem(credit.TodoList[1], credit.Id, "Todo description 2", TodoItemState.Done, 1, new DateTime(2015, 1, 24));
      }

      [Test]
      public void SelectCreditWithRequiredDocumentsTest()
      {
         // Arrange.
         const string creditNumber = "CNS5";

         // Act.
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Assert.
         Assert.IsNotNull(credit);

         CreditType creditType = getCreditTypeByName("CNS5_CT1");
         AssertUtils.AssertCreditTypesAreEqual(creditType, credit.CreditType);

         Assert.AreEqual("Description", credit.NotificationDescription);
         Assert.AreEqual(1, credit.NotificationCount);
         Assert.AreEqual(new DateTime(2015, 2, 7), credit.NotificationDate);

         Assert.IsNotNull(credit.RequiredDocuments);
         Assert.AreEqual(2, credit.RequiredDocuments.Count);

         checkRequiredDocument(credit.RequiredDocuments[0], credit.Id,
                               getDocumentTypeByName("CNS5_DT1"), RequiredDocumentState.None);
         checkRequiredDocument(credit.RequiredDocuments[1], credit.Id,
                               getDocumentTypeByName("CNS5_DT2"), RequiredDocumentState.Carried);
      }

      private CreditInfo getCreditByNumber(string creditNumber)
      {
         return
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.CreditNumber == creditNumber);
      }

      private CreditType getCreditTypeByName(string name)
      {
         return
            _database
               .GetAllCreditTypes()
               .SingleOrDefault(item => item.Name == name);
      }

      private DocumentType getDocumentTypeByName(string name)
      {
         return
            _database
               .GetAllDocumentTypes()
               .Single(item => item.Name == name);
      }

      private static void checkPersonInfo(
         PersonInfo borrower,
         bool isNew,
         int creditId,
         string personalNumber,
         string personName,
         string registrationAddress,
         string factAddress,
         string passportNumber,
         DateTime passportIssueDate,
         string passportIssuer)
      {
         Assert.IsNotNull(borrower);
         Assert.AreEqual(isNew, borrower.IsNew);
         Assert.AreEqual(creditId, borrower.CreditId);
         Assert.AreEqual(personalNumber, borrower.PersonalNumber);
         Assert.AreEqual(personName, borrower.PersonName);
         Assert.AreEqual(registrationAddress, borrower.RegistrationAddress);
         Assert.AreEqual(factAddress, borrower.FactAddress);
         Assert.AreEqual(passportNumber, borrower.PassportNumber);
         Assert.AreEqual(passportIssueDate, borrower.PassportIssueDate);
         Assert.AreEqual(passportIssuer, borrower.PassportIssuer);
      }

      private static void checkPhoneNumberInfo(
         PhoneNumberInfo phoneNumberInfo,
         string phoneNumber,
         int personId)
      {
         Assert.IsNotNull(phoneNumberInfo);
         Assert.AreEqual(personId, phoneNumberInfo.PersonId);
         Assert.AreEqual(phoneNumber, phoneNumberInfo.PhoneNumber);
      }

      private void checkPaymentInfo(
         PaymentInfo paymentInfo,
         bool isNew,
         decimal paymentAmount,
         DateTime paymentDate,
         bool isNotified)
      {
         Assert.IsNotNull(paymentInfo);
         Assert.AreEqual(isNew, paymentInfo.IsNew);
         Assert.AreEqual(paymentAmount, paymentInfo.PaymentAmount);
         Assert.AreEqual(paymentDate, paymentInfo.PaymentDate);
         Assert.AreEqual(isNotified, paymentInfo.IsNotified);
      }

      private void checkTodoItem(
         TodoItem todoItem,
         int creditId,
         string description,
         TodoItemState state,
         int notificationCount,
         DateTime? notificationDate
         )
      {
         Assert.IsNotNull(todoItem);
         Assert.AreEqual(creditId, todoItem.CreditId);
         Assert.AreEqual(description, todoItem.Description);
         Assert.AreEqual(state, todoItem.State);
         Assert.AreEqual(notificationCount, todoItem.NotificationCount);
         Assert.AreEqual(notificationDate, todoItem.NotificationDate);
      }

      private void checkRequiredDocument(
         RequiredDocument requiredDocument,
         int creditId,
         DocumentType documentType,
         RequiredDocumentState state
         )
      {
         Assert.IsNotNull(requiredDocument);
         Assert.AreEqual(creditId, requiredDocument.CreditId);
         AssertUtils.AssertDocumentTypesAreEqual(documentType, requiredDocument.DocumentType);
         Assert.AreEqual(state, requiredDocument.State);
      }
   }
}
