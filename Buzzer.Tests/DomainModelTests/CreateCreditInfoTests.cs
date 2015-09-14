using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Models;
using Buzzer.Tests.Common;
using NUnit.Framework;

namespace Buzzer.Tests.DomainModelTests
{
   [TestFixture]
   public class CreateCreditInfoTests
   {
      [Test]
      public void CreateNewCreditInfoTest()
      {
         // Arrange/Act.
         CreditInfo credit = CreditInfo.CreatNew();

         // Assert.
         Assert.IsNotNull(credit);
         Assert.IsNull(credit.CreditNumber);
         Assert.AreEqual(DateTime.Today, credit.ApplicationDate);
         Assert.IsNull(credit.ProtocolDate);
         Assert.AreEqual(0M, credit.CreditAmount);
         Assert.AreEqual(NullValues.DateTime, credit.CreditIssueDate);
         Assert.AreEqual(0, credit.MonthsCount);
         Assert.AreEqual(0M, credit.DiscountRate);
         Assert.IsNull(credit.EffectiveDiscountRate);
         Assert.IsNull(credit.ExchangeRate);
         Assert.AreEqual(CreditState.Consideration, credit.CreditState);
         Assert.IsNull(credit.RefusalReason);
         Assert.AreEqual(RowState.Modified, credit.RowState);
         Assert.IsNull(credit.CreditType);
         Assert.IsNullOrEmpty(credit.NotificationDescription);
         Assert.AreEqual(0, credit.NotificationCount);
         Assert.IsNull(credit.NotificationDate);

         Assert.IsNotNull(credit.Borrower);

         Assert.IsNotNull(credit.Guarantors);
         Assert.IsEmpty(credit.Guarantors);

         Assert.IsNotNull(credit.PaymentsSchedule);
         Assert.IsEmpty(credit.PaymentsSchedule);

         Assert.IsNotNull(credit.TodoList);
         Assert.IsEmpty(credit.TodoList);

         Assert.IsNotNull(credit.RequiredDocuments);
         Assert.IsEmpty(credit.RequiredDocuments);
      }

      [Test]
      [TestCaseSource("TestCases")]
      public void CreateCreditInfoTest(
         int id,
         string creditNumber,
         DateTime? applicationDate,
         DateTime? protocolDate,
         decimal creditAmount,
         DateTime creditIssueDate,
         int monthsCount,
         decimal discountRate,
         decimal? effectiveDiscountRate,
         decimal? exchangeRate,
         CreditState creditState,
         string refusalReason,
         RowState rowState,
         PersonInfo borrower,
         CreditType creditType,
         string notificationDescription,
         int notificationCount,
         DateTime notificationDate,
         PersonInfo[] guarantors,
         PaymentInfo[] paymentsSchedule,
         TodoItem[] todoList,
         RequiredDocument[] requiredDocuments,
         PayoffInfo[] payoffs
         )
      {
         // Arrange/Act.
         CreditInfo credit =
            CreditInfo.Create(
               id,
               creditNumber,
               applicationDate,
               protocolDate,
               creditAmount,
               creditIssueDate,
               monthsCount,
               discountRate,
               effectiveDiscountRate,
               exchangeRate,
               creditState,
               refusalReason,
               rowState,
               borrower,
               creditType,
               notificationDescription,
               notificationCount,
               notificationDate,
               guarantors,
               paymentsSchedule,
               todoList,
               requiredDocuments,
               payoffs
               );

         // Assert.
         Assert.IsNotNull(credit);

         Assert.AreEqual(id, credit.Id);
         Assert.AreEqual(creditNumber, credit.CreditNumber);
         Assert.AreEqual(applicationDate, credit.ApplicationDate);
         Assert.AreEqual(protocolDate, credit.ProtocolDate);
         Assert.AreEqual(creditAmount, credit.CreditAmount);
         Assert.AreEqual(creditIssueDate, credit.CreditIssueDate);
         Assert.AreEqual(monthsCount, credit.MonthsCount);
         Assert.AreEqual(discountRate, credit.DiscountRate);
         Assert.AreEqual(effectiveDiscountRate, credit.EffectiveDiscountRate);
         Assert.AreEqual(exchangeRate, credit.ExchangeRate);
         Assert.AreEqual(creditState, credit.CreditState);
         Assert.AreEqual(refusalReason, credit.RefusalReason);
         Assert.AreEqual(rowState, credit.RowState);

         AssertUtils.AssertPersonsAreEqual(borrower, credit.Borrower);

         Assert.AreEqual(creditType.Id, credit.CreditType.Id);
         Assert.AreEqual(notificationDescription, credit.NotificationDescription);
         Assert.AreEqual(notificationCount, credit.NotificationCount);
         Assert.AreEqual(notificationDate, credit.NotificationDate);

         AssertUtils.AssertCollectionsAreEqual(
            guarantors,
            credit.Guarantors,
            AssertUtils.AssertPersonsAreEqual
            );
         
         AssertUtils.AssertCollectionsAreEqual(
            paymentsSchedule,
            credit.PaymentsSchedule,
            AssertUtils.AssertPaymentsAreEqual
            );

         AssertUtils.AssertCollectionsAreEqual(
            todoList,
            credit.TodoList,
            AssertUtils.AssertTodoItemsAreEqual
            );

         AssertUtils.AssertCollectionsAreEqual(
            requiredDocuments,
            credit.RequiredDocuments,
            AssertUtils.AssertRequiredDocumentsAreEqual
            );
      }

      public TestCaseData[] TestCases
      {
         get
         {
            var testCases = new List<TestCaseData>();

            #region CreateValidCreditInfoTest

            {
               const int id = 1;
               const string creditNumber = "Credit number";
               DateTime? applicationDate = DateTime.Today.AddDays(-2);
               DateTime? protocolDate = DateTime.Today.AddDays(-1);
               const decimal creditAmount = 100000M;
               DateTime creditIssueDate = DateTime.Today;
               const int monthsCount = 2;
               const decimal discountRate = 0.36M;
               decimal? effectiveDiscountRate = null;
               decimal? exchangeRate = null;
               const CreditState creditState = CreditState.Repayed;
               const string refusalReason = "Refusal reason";
               const RowState rowState = RowState.Modified;

               PersonInfo borrower =
                  PersonInfo.Create(
                     1, id, "01234567890123", "Borrower name", "Borrower registration address",
                     "Borrower fact address", "Borrower passport number", DateTime.Today,
                     "Passport issuer", true, new[] {PhoneNumberInfo.Create(1, 1, "555123456")}
                     );

               CreditType creditType = CreditType.Create(1, "CreditType");
               const string notificationDescription = "Description";
               const int notificationCount = 5;
               DateTime? notificationDate = DateTime.Today;

               var guarantors =
                  new[]
                     {
                        PersonInfo.Create(
                           2, id, "12345678901234", "Guarantor 1 name", "Guarantor 1 registration address",
                           "Guarantor 1 fact address", "Guarantor 1 passport number", DateTime.Today,
                           "Passport issuer", false, new[] {PhoneNumberInfo.Create(2, 2, "555654321")}
                           ),
                        PersonInfo.Create(
                           3, id, "23456789012345", "Guarantor 2 name", "Guarantor 2 registration address",
                           "Guarantor 2 fact address", "Guarantor 2 passport number", DateTime.Today,
                           "Passport issuer", false,
                           new[]
                              {
                                 PhoneNumberInfo.Create(3, 3, "555111111"),
                                 PhoneNumberInfo.Create(4, 3, "555222222")
                              }
                           )
                     };

               var paymentsSchedule =
                  new[]
                     {
                        PaymentInfo.Create(1, 150000M, DateTime.Today, true),
                        PaymentInfo.Create(2, 150000M, DateTime.Today.AddMonths(1), false)
                     };

               TodoItem[] todoList =
                  {
                     TodoItem.Create(1, id, "TodoItem description 1", TodoItemState.None, 0, null),
                     TodoItem.Create(2, id, "TodoItem description 2", TodoItemState.Done, 1, DateTime.Today)
                  };

               RequiredDocument[] requiredDocuments =
                  {
                     RequiredDocument.Create(1, id, DocumentType.Create(1, "DT1"), RequiredDocumentState.None),
                     RequiredDocument.Create(2, id, DocumentType.Create(1, "DT2"), RequiredDocumentState.Carried)
                  };

               PayoffInfo[] payoffs = new PayoffInfo[0];

               var testCaseData =
                  new TestCaseData(
                     id, creditNumber, applicationDate, protocolDate, creditAmount,
                     creditIssueDate, monthsCount, discountRate, effectiveDiscountRate,
                     exchangeRate, creditState, refusalReason, rowState, borrower,
                     creditType, notificationDescription, notificationCount, notificationDate,
                     guarantors, paymentsSchedule, todoList, requiredDocuments, payoffs
                     );
               testCaseData.SetName("CreateValidCreditInfoTest");

               testCases.Add(testCaseData);
            }

            #endregion

            return testCases.ToArray();
         }
      }
   }
}