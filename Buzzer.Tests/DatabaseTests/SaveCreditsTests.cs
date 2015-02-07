using System;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Tests.Common;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class SaveCreditsTests
   {
      private BuzzerDatabase _database;

      [TestFixtureSetUp]
      public void SetUpFixture()
      {
         _database = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SaveNewEmptyCreditTest()
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();

         // Act.
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase = getCreditById(credit.Id);
         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveNewCreditTest()
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();
         credit.CreditNumber = "Credit number";
         credit.ApplicationDate = DateTime.Today.AddDays(-2);
         credit.ProtocolDate = DateTime.Today.AddDays(-1);
         credit.CreditAmount = 100.0M;
         credit.CreditIssueDate = DateTime.Today;
         credit.MonthsCount = 12;
         credit.DiscountRate = 0.36M;
         credit.EffectiveDiscountRate = 0.01M;
         credit.ExchangeRate = 45.0M;
         credit.CreditState = CreditState.Repayed;
         credit.RefusalReason = "Refusal reason";
         credit.CreditType = getCreditTypeByName("SC_CT1");
         credit.NotificationDescription = "Notification description";
         credit.NotificationCount = 1;
         credit.NotificationDate = DateTime.Today;
         
         {
            PersonInfo borrower = credit.Borrower;
            borrower.PersonalNumber = "00000000000000";
            borrower.PersonName = "Borrower";
            borrower.RegistrationAddress = "Borrower registration address";
            borrower.FactAddress = "Borrower fact address";
            borrower.PassportNumber = "Borrower passport number";
            borrower.PassportIssuer = "Borrower passport issuer";
            borrower.PassportIssueDate = DateTime.Today;

            borrower.AddPhoneNumber().PhoneNumber = "111111111";
            borrower.AddPhoneNumber().PhoneNumber = "222222222";
         }

         {
            PersonInfo guarantor = credit.AddGuarantor();
            guarantor.PersonalNumber = "11111111111111";
            guarantor.PersonName = "Guarantor1";
            guarantor.RegistrationAddress = "Guarantor1 registration address";
            guarantor.FactAddress = "Guarantor1 fact address";
            guarantor.PassportNumber = "Guarantor1 passport number";
            guarantor.PassportIssuer = "Guarantor1 passport issuer";
            guarantor.PassportIssueDate = DateTime.Today;

            guarantor.AddPhoneNumber().PhoneNumber = "333333333";
         }

         {
            PersonInfo guarantor = credit.AddGuarantor();
            guarantor.PersonalNumber = "22222222222222";
            guarantor.PersonName = "Guarantor2";
            guarantor.RegistrationAddress = "Guarantor2 registration address";
            guarantor.FactAddress = "Guarantor2 fact address";
            guarantor.PassportNumber = "Guarantor2 passport number";
            guarantor.PassportIssuer = "Guarantor2 passport issuer";
            guarantor.PassportIssueDate = DateTime.Today;
         }

         {
            TodoItem todoItem = credit.AddTodoItem();
            todoItem.Description = "Todo item 1";
         }

         {
            TodoItem todoItem = credit.AddTodoItem();
            todoItem.Description = "Todo item 2";
            todoItem.State = TodoItemState.Done;
            todoItem.NotificationCount = 1;
            todoItem.NotificationDate = DateTime.Today;
         }

         {
            DocumentType documentType = getDocumentTypeByName("DT1");
            credit.AddRequiredDocument(documentType);
         }

         {
            DocumentType documentType = getDocumentTypeByName("DT2");
            RequiredDocument requiredDocument = credit.AddRequiredDocument(documentType);
            requiredDocument.State = RequiredDocumentState.Carried;
         }

         credit.BuildPaymentsSchedule();

         // Act.
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase = getCreditById(credit.Id);
         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveEditedCreditTest()
      {
         // Arrange.
         const string creditNumber = "CNE1";
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Act.
         credit.CreditNumber = "CN changed";
         credit.ApplicationDate = new DateTime(2012, 12, 20);
         credit.ProtocolDate = new DateTime(2012, 12, 25);
         credit.CreditAmount = 100.0M;
         credit.CreditIssueDate = new DateTime(2012, 12, 31);
         credit.MonthsCount = 24;
         credit.DiscountRate = 0.0036M;
         credit.EffectiveDiscountRate = 0.0024M;
         credit.ExchangeRate = 60.0M;
         credit.CreditState = CreditState.Repayed;
         credit.RefusalReason = "New refusal reason";
         credit.CreditType = getCreditTypeByName("SC_CT1");
         credit.NotificationDescription = "Changed notification description";
         credit.NotificationCount = 2;
         credit.NotificationDate = DateTime.Today;

         credit.Delete();

         PersonInfo borrower = credit.Borrower;
         borrower.PersonalNumber = "44444444444444";
         borrower.PersonName = "Borrower changed";
         borrower.RegistrationAddress = "Address changed";
         borrower.FactAddress = "Fact address changed";
         borrower.PassportNumber = "Passport changed";
         borrower.PassportIssuer = "Issuer changed";
         borrower.PassportIssueDate = new DateTime(2012, 12, 1);

         {
            PersonInfo guarantor = getPersonByPersonalNumber(credit, "22222222222222");
            credit.RemoveGuarantor(guarantor);
         }

         {
            PersonInfo guarantor = getPersonByPersonalNumber(credit, "33333333333333");
            guarantor.PersonalNumber = "55555555555555";
            guarantor.PersonName = "Guarantor 2 changed";
            guarantor.RegistrationAddress = "Address 2 changed";
            guarantor.FactAddress = "Fact address 2 changed";
            guarantor.PassportNumber = "Passport 2 changed";
            guarantor.PassportIssuer = "Issuer 2 changed";
            guarantor.PassportIssueDate = new DateTime(2012, 12, 2);

            guarantor.PhoneNumbers.First().PhoneNumber = "333333333";
            guarantor.RemovePhoneNumber(guarantor.PhoneNumbers.Last());
            guarantor.AddPhoneNumber().PhoneNumber = "444444444";
         }

         {
            PersonInfo guarantor = credit.AddGuarantor();
            guarantor.PersonalNumber = "66666666666666";
            guarantor.PersonName = "Guarantor 3";
            guarantor.RegistrationAddress = "Address 3";
            guarantor.FactAddress = "Fact address 3";
            guarantor.PassportNumber = "Passport 3";
            guarantor.PassportIssuer = "Issuer 3";
            guarantor.PassportIssueDate = new DateTime(2012, 12, 4);
         }

         {
            TodoItem todoItem = getTodoItemByDescription(credit, "Todo item 1");
            credit.RemoveTodoItem(todoItem);
         }

         {
            TodoItem todoItem = getTodoItemByDescription(credit, "Todo item 2");
            
            todoItem.Description = "Todo item 2 changed";
            todoItem.State = TodoItemState.Done;
            todoItem.NotificationCount = 1;
            todoItem.NotificationDate = DateTime.Today;
         }

         {
            TodoItem todoItem = credit.AddTodoItem();
            todoItem.Description = "Todo item 3";
         }

         {
            RequiredDocument requiredDocument =
               getRequiredDocumentByDocumentType(credit, getDocumentTypeByName("SC_DT1"));
            credit.RemoveRequiredDocument(requiredDocument);
         }

         {
            RequiredDocument requiredDocument =
               getRequiredDocumentByDocumentType(credit, getDocumentTypeByName("SC_DT2"));
            requiredDocument.State = RequiredDocumentState.Carried;
         }

         {
            DocumentType documentType = getDocumentTypeByName("SC_DT3");
            credit.AddRequiredDocument(documentType);
         }

         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase = getCreditById(credit.Id);
         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveCreditWithRebuildedPaymentsScheduleTest()
      {
         // Arrange.
         const string creditNumber = "CNE2";
         CreditInfo credit = getCreditByNumber(creditNumber);

         // Act.
         credit.CreditAmount = 500000M;
         credit.CreditIssueDate = new DateTime(2014, 1, 31);
         credit.MonthsCount = 5;
         credit.DiscountRate = 0.36M;
         credit.ExchangeRate = 60.0M;

         credit.BuildPaymentsSchedule();
         
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase = getCreditById(credit.Id);
         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveDeletedCreditTest()
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();
         credit.Delete();

         // Act.
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase = getCreditById(credit.Id);
         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      private CreditInfo getCreditById(int id)
      {
         return
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.Id == id);
      }

      private CreditType getCreditTypeByName(string name)
      {
         return
            _database
               .GetAllCreditTypes()
               .Single(item => item.Name == name);
      }

      private DocumentType getDocumentTypeByName(string name)
      {
         return
            _database
               .GetAllDocumentTypes()
               .Single(item => item.Name == name);
      }

      private CreditInfo getCreditByNumber(string creditNumber)
      {
         return
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);
      }

      private static PersonInfo getPersonByPersonalNumber(CreditInfo credit, string personalNumber)
      {
         return
            credit
               .Guarantors
               .Single(item => item.PersonalNumber == personalNumber);
      }

      private static TodoItem getTodoItemByDescription(CreditInfo credit, string description)
      {
         return
            credit
               .TodoList
               .Single(item => item.Description == description);
      }
      
      private RequiredDocument getRequiredDocumentByDocumentType(CreditInfo credit, DocumentType documentType)
      {
         return
            credit
               .RequiredDocuments
               .Single(item => item.DocumentType.Id == documentType.Id);
      }
   }
}