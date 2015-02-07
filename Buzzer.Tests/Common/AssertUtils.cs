using System;
using System.Collections.Generic;
using System.Linq;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.Common
{
   public static class AssertUtils
   {
      public static void AssertCreditsAreEqual(CreditInfo expected, CreditInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.CreditNumber, actual.CreditNumber);
         Assert.AreEqual(expected.ApplicationDate, actual.ApplicationDate);
         Assert.AreEqual(expected.ProtocolDate, actual.ProtocolDate);
         Assert.AreEqual(expected.CreditAmount, actual.CreditAmount);
         Assert.AreEqual(expected.CreditIssueDate, actual.CreditIssueDate);
         Assert.AreEqual(expected.MonthsCount, actual.MonthsCount);
         Assert.AreEqual(expected.DiscountRate, actual.DiscountRate);
         Assert.AreEqual(expected.EffectiveDiscountRate, actual.EffectiveDiscountRate);
         Assert.AreEqual(expected.ExchangeRate, actual.ExchangeRate);
         Assert.AreEqual(expected.CreditState, actual.CreditState);
         Assert.AreEqual(expected.RefusalReason, actual.RefusalReason);
         Assert.AreEqual(expected.RowState, actual.RowState);
         AssertCreditTypesAreEqual(expected.CreditType, actual.CreditType);
         Assert.AreEqual(expected.NotificationDescription, actual.NotificationDescription);
         Assert.AreEqual(expected.NotificationCount, actual.NotificationCount);
         Assert.AreEqual(expected.NotificationDate, actual.NotificationDate);

         AssertPersonsAreEqual(expected.Borrower, actual.Borrower);
         AssertCollectionsAreEqual(expected.Guarantors, actual.Guarantors,
                                   AssertPersonsAreEqual);

         AssertCollectionsAreEqual(expected.PaymentsSchedule, actual.PaymentsSchedule,
                                   AssertPaymentsAreEqual);

         AssertCollectionsAreEqual(expected.TodoList, actual.TodoList,
                                   AssertTodoItemsAreEqual);

         AssertCollectionsAreEqual(expected.RequiredDocuments, actual.RequiredDocuments,
                                   AssertRequiredDocumentsAreEqual);
      }

      public static void AssertPersonsAreEqual(PersonInfo expected, PersonInfo actual)
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

         AssertCollectionsAreEqual(expected.PhoneNumbers, actual.PhoneNumbers,
                                   AssertPhoneNumbersAreEqual);
      }

      public static void AssertPhoneNumbersAreEqual(PhoneNumberInfo expected, PhoneNumberInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.PersonId, actual.PersonId);
         Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
      }

      public static void AssertPaymentsAreEqual(PaymentInfo expected, PaymentInfo actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.PaymentAmount, actual.PaymentAmount);
         Assert.AreEqual(expected.PaymentAmount, actual.PaymentAmount);
         Assert.AreEqual(expected.IsNotified, actual.IsNotified);
      }

      public static void AssertTodoItemsAreEqual(TodoItem expected, TodoItem actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.CreditId, actual.CreditId);
         Assert.AreEqual(expected.Description, actual.Description);
         Assert.AreEqual(expected.State, actual.State);
         Assert.AreEqual(expected.NotificationCount, actual.NotificationCount);
         Assert.AreEqual(expected.NotificationDate, actual.NotificationDate);
      }

      public static void AssertDocumentTypesAreEqual(DocumentType expectedDocumentType, DocumentType actualDocumentType)
      {
         Assert.IsNotNull(expectedDocumentType);
         Assert.IsNotNull(actualDocumentType);

         Assert.AreEqual(expectedDocumentType.Id, actualDocumentType.Id);
         Assert.AreEqual(expectedDocumentType.Name, actualDocumentType.Name);
      }

      public static void AssertCreditTypesAreEqual(CreditType expected, CreditType actual)
      {
         if (expected == null && actual == null)
            return;

         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.Id, actual.Id);
         Assert.AreEqual(expected.Name, actual.Name);
      }

      public static void AssertRequiredDocumentsAreEqual(
         RequiredDocument expecteRequiredDocument, RequiredDocument actualRequiredDocument
         )
      {
         Assert.IsNotNull(expecteRequiredDocument);
         Assert.IsNotNull(actualRequiredDocument);

         Assert.AreEqual(expecteRequiredDocument.Id, actualRequiredDocument.Id);
         AssertDocumentTypesAreEqual(expecteRequiredDocument.DocumentType, actualRequiredDocument.DocumentType);
         Assert.AreEqual(expecteRequiredDocument.State, actualRequiredDocument.State);
      }

      public static void AssertCollectionsAreEqual<T>(
         IEnumerable<T> expectedCollection,
         IEnumerable<T> actualCollection,
         Action<T, T> assert)
         where T : DomainObject
      {
         Assert.IsNotNull(expectedCollection);
         Assert.IsNotNull(actualCollection);

         var expected = expectedCollection.ToArray();
         var actual = actualCollection.ToArray();

         Assert.AreEqual(expected.Length, actual.Length);

         var expectedOrdered = expected.OrderBy(item => item.Id);
         var actualOrdered = actual.OrderBy(item => item.Id);
         var zipped =
            expectedOrdered
               .Zip(actualOrdered, (expectedItem, actualItem) => new { expectedItem, actualItem })
               .ToList();
         zipped.ForEach(pair => assert(pair.expectedItem, pair.actualItem));
      }
   }
}