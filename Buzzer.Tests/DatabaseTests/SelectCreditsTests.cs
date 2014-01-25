using System;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
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
         const string creditNumber = "CN2";

         // Act.
         CreditInfo credit =
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.CreditNumber == creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.AreEqual(200000M, credit.CreditAmount);
         Assert.AreEqual(new DateTime(2014, 1, 2), credit.CreditIssueDate);
         Assert.AreEqual(24, credit.MonthsCount);
         Assert.AreEqual(0.36M, credit.DiscountRate);
         Assert.AreEqual(0.12M, credit.EffectiveDiscountRate);
         Assert.AreEqual(47.5M, credit.ExchangeRate);

         PersonInfo borrower = credit.Borrower;

         checkPersonInfo(
            borrower, credit.Id, "01234567890123", "Borrower of CN2", "Address",
            "Fact address", "Passport", new DateTime(2013, 12, 1), "Issuer"
            );

         Assert.IsNotNull(borrower.PhoneNumbers);
         Assert.IsEmpty(borrower.PhoneNumbers);

         Assert.IsNotNull(credit.Guarantors);
         Assert.IsEmpty(credit.Guarantors);
      }

      [Test]
      public void SelectCreditWithGuarantorsTest()
      {
         // Arrange.
         const string creditNumber = "CN3";

         // Act.
         CreditInfo credit =
            _database
               .GetAllCredits()
               .SingleOrDefault(item => item.CreditNumber == creditNumber);

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsNull(credit.EffectiveDiscountRate);
         Assert.IsNull(credit.ExchangeRate);
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
               guarantor, credit.Id, "34567890123456", "Guarantor 2 of CN3", "Address 2",
               "Fact address 2", "Passport 2", new DateTime(2013, 12, 3), "Issuer 2"
               );

            Assert.IsNotNull(guarantor.PhoneNumbers);
            Assert.AreEqual(2, guarantor.PhoneNumbers.Count);

            checkPhoneNumberInfo(guarantor.PhoneNumbers.First(), "555666666", guarantor.Id);
            checkPhoneNumberInfo(guarantor.PhoneNumbers.Last(), "777888888", guarantor.Id);
         }
      }

      private static void checkPersonInfo(
         PersonInfo borrower,
         int creditId,
         string personalNumber,
         string personName,
         string registrationAddress,
         string factAddress,
         string passportNumber,
         DateTime passportIssueDate,
         string passportIssuer
         )
      {
         Assert.IsNotNull(borrower);
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
   }
}
