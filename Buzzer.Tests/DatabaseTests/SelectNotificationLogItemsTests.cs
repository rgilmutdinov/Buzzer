using System;
using System.Collections.Generic;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class SelectNotificationLogItemsTests
   {
      private BuzzerDatabase _database;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _database = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SelectNotificationLogItemsTest()
      {
         // Arrange.
         const string creditNumber = "CNSNLI1";
         CreditInfo credit =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);
         
         // Act.
         NotificationLogItemInfo[] notificationLogItemInfos =
            _database
               .GetNotificationLogItems()
               .Where(item => item.CreditId == credit.Id &&
                              item.PersonId == credit.Borrower.Id)
               .ToArray();

         // Assert.
         Assert.AreEqual(2, notificationLogItemInfos.Length);

         {
            NotificationLogItemInfo logItem = notificationLogItemInfos[0];
            Assert.AreEqual(credit.CreditNumber, logItem.CreditNumber);
            Assert.AreEqual(credit.Borrower.PersonName, logItem.PersonName);
            Assert.AreEqual(new DateTime(2013, 12, 31, 10, 20, 30), logItem.NotificationDate);
            Assert.IsNull(logItem.Comment);
         }

         {
            NotificationLogItemInfo logItem = notificationLogItemInfos[1];
            Assert.AreEqual(credit.CreditNumber, logItem.CreditNumber);
            Assert.AreEqual(credit.Borrower.PersonName, logItem.PersonName);
            Assert.AreEqual(new DateTime(2014, 1, 15, 10, 20, 30), logItem.NotificationDate);
            Assert.AreEqual("Comment", logItem.Comment);
         }
      }

      [Test]
      public void SelectNotificationLogItemsDoesNotReturnDeletedNotificationLogItems()
      {
         // Arrange.
         const string creditNumber = "CNSDNLI1";
         CreditInfo credit =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);

         // Act.
         NotificationLogItemInfo[] logItems =
            _database
               .GetNotificationLogItems()
               .Where(item => item.CreditId == credit.Id &&
                              item.PersonId == credit.Borrower.Id)
               .ToArray();

         // Assert.
         Assert.IsEmpty(logItems);
      }
   }
}