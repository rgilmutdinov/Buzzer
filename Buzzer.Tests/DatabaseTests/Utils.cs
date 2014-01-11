using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Model;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   public static class Utils
   {
      public static void AssertCreditsAreEqual(CreditInfo expected, CreditInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.CreditNumber, actual.CreditNumber);
         Assert.AreEqual(expected.CreditAmount, actual.CreditAmount);
         Assert.AreEqual(expected.CreditIssueDate, actual.CreditIssueDate);
         Assert.AreEqual(expected.MonthsCount, actual.MonthsCount);
         Assert.AreEqual(expected.DiscountRate, actual.DiscountRate);
         Assert.AreEqual(expected.EffectiveDiscountRate, actual.EffectiveDiscountRate);
         Assert.AreEqual(expected.ExchangeRate, actual.ExchangeRate);

         assertPersonsAreEqual(expected.Borrower, actual.Borrower);
         assertCollectionsAreEqual(expected.Guarantors, actual.Guarantors,
                                   assertPersonsAreEqual);
      }

      private static void assertPersonsAreEqual(PersonInfo expected, PersonInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.CreditId, actual.CreditId);
         Assert.AreEqual(expected.PersonalNumber, actual.PersonalNumber);
         Assert.AreEqual(expected.PersonName, actual.PersonName);
         Assert.AreEqual(expected.RegistrationAddress, actual.RegistrationAddress);
         Assert.AreEqual(expected.FactAddress, actual.FactAddress);
         Assert.AreEqual(expected.PassportNumber, actual.PassportNumber);
         Assert.AreEqual(expected.PassportIssuer, actual.PassportIssuer);
         Assert.AreEqual(expected.PassportIssueDate, actual.PassportIssueDate);

         assertCollectionsAreEqual(expected.PhoneNumbers, actual.PhoneNumbers,
                                   assertPhoneNumbersAreEqual);
      }

      private static void assertCollectionsAreEqual<T>(
         IEnumerable<T> expectedCollection,
         IEnumerable<T> actualCollection,
         Action<T, T> assert)
         where T : RepositoryItem
      {
         Assert.IsNotNull(expectedCollection);
         Assert.IsNotNull(actualCollection);

         var expected = expectedCollection.ToArray();
         var actual = actualCollection.ToArray();

         Assert.AreEqual(expected.Length, actual.Length);

         var expectedPhones = expected.OrderBy(item => item.Id);
         var actualPhones = actual.OrderBy(item => item.Id);
         var phoneNumbers =
            expectedPhones
               .Zip(actualPhones, (expectedPhone, actualPhone) => new {expectedPhone, actualPhone})
               .ToList();
         phoneNumbers.ForEach(pair => assert(pair.expectedPhone, pair.actualPhone));
      }

      private static void assertPhoneNumbersAreEqual(PhoneNumberInfo expected, PhoneNumberInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.PersonId, actual.PersonId);
         Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
      }
   }
}