using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DomainModelTests
{
   [TestFixture]
   public class TodoItemTests
   {
      private const int CreditId = 1;

      [Test]
      [TestCaseSource("TestCases")]
      public void NotifyTest(
         DateTime? notificationDate,
         int notificationCount,
         int expectedNotificationCount
         )
      {
         TodoItem todoItem = TodoItem.CreateNew(CreditId);
         todoItem.NotificationDate = notificationDate;
         todoItem.NotificationCount = notificationCount;
         
         todoItem.Notified();

         Assert.AreEqual(expectedNotificationCount, todoItem.NotificationCount);
         Assert.AreEqual(DateTime.Today, todoItem.NotificationDate);
      }

      public TestCaseData[] TestCases
      {
         get
         {
            var testCases =
               new List<TestCaseData>
                  {
                     new TestCaseData(DateTime.Today, 0, 1),
                     new TestCaseData(DateTime.Today, 1, 2),
                     new TestCaseData(null, 0, 1),
                     new TestCaseData(DateTime.Today.AddDays(-1), 1, 1)
                  };

            return testCases.ToArray();
         }
      }
   }
}