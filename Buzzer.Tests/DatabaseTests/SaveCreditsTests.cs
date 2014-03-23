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
      public void SaveNewCreditTest()
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();
         credit.CreditNumber = "Credit number";
         credit.CreditAmount = 100.0M;
         credit.CreditIssueDate = DateTime.Today;
         credit.MonthsCount = 12;
         credit.DiscountRate = 0.36M;
         credit.EffectiveDiscountRate = 0.01M;
         credit.ExchangeRate = 45.0M;
         credit.CreditState = CreditState.Repayed;
         
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

         credit.BuildPaymentsSchedule();

         // Act.
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase =
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.Id == credit.Id);

         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveEditedCreditTest()
      {
         // Arrange.
         const string creditNumber = "CNE1";
         CreditInfo credit =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);

         // Act.
         credit.CreditNumber = "CN changed";
         credit.CreditAmount = 100.0M;
         credit.CreditIssueDate = new DateTime(2012, 12, 31);
         credit.MonthsCount = 24;
         credit.DiscountRate = 0.0036M;
         credit.EffectiveDiscountRate = 0.0024M;
         credit.ExchangeRate = 60.0M;
         credit.CreditState = CreditState.Repayed;

         PersonInfo borrower = credit.Borrower;
         borrower.PersonalNumber = "44444444444444";
         borrower.PersonName = "Borrower changed";
         borrower.RegistrationAddress = "Address changed";
         borrower.FactAddress = "Fact address changed";
         borrower.PassportNumber = "Passport changed";
         borrower.PassportIssuer = "Issuer changed";
         borrower.PassportIssueDate = new DateTime(2012, 12, 1);

         {
            PersonInfo guarantor =
               credit
                  .Guarantors
                  .Single(item => item.PersonalNumber == "22222222222222");
            credit.RemoveGuarantor(guarantor);
         }

         {
            PersonInfo guarantor =
               credit
                  .Guarantors
                  .Single(item => item.PersonalNumber == "33333333333333");
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

         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase =
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.Id == credit.Id);

         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }

      [Test]
      public void SaveCreditWithRebuildedPaymentsScheduleTest()
      {
         // Arrange.
         const string creditNumber = "CNE2";
         CreditInfo credit =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);

         // Act.
         credit.CreditAmount = 500000M;
         credit.CreditIssueDate = new DateTime(2014, 1, 31);
         credit.MonthsCount = 5;
         credit.DiscountRate = 0.36M;
         credit.ExchangeRate = 60.0M;

         credit.BuildPaymentsSchedule();
         
         _database.SaveCredit(credit);

         // Assert.
         CreditInfo creditFromDatabase =
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.Id == credit.Id);

         AssertUtils.AssertCreditsAreEqual(credit, creditFromDatabase);
      }
   }
}