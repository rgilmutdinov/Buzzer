using System;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class SaveNotificationLogItemTests
   {
      private BuzzerDatabase _database;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _database = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SaveNewNotificationLogItem()
      {
         // Arrange.
         const string creditNumber = "CNSNLI2";

         CreditInfo creditInfo =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);

         DateTime notificationDate = DateTime.Now;
         const string comment = "Comment of the new notification log item";
         var notificationLogItem =
            NotificationLogItemInfo.CreateNew(
               creditInfo.Id, creditInfo.Borrower.Id,
               notificationDate, comment);

         // Act.
         _database.SaveNotificationLogItem(notificationLogItem);

         // Assert.
         assertNotificationLogItem(notificationLogItem);
      }

      [Test]
      public void SaveEditedNotificationLogItem()
      {
         // Arrange.
         const string creditNumber = "CNSNLI2";

         CreditInfo credit =
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);

         var notificationDate = new DateTime(2013, 12, 31, 10, 20, 30);

         NotificationLogItemInfo notificationLogItem =
            _database
               .GetNotificationLogItems()
               .Single(item => item.CreditId == credit.Id &&
                               item.PersonId == credit.Borrower.Id &&
                               item.NotificationDate == notificationDate);

         // Act.
         const string comment = "Change comment of the notification log item";
         notificationLogItem.Comment = comment;
         _database.SaveNotificationLogItem(notificationLogItem);

         // Assert.
         assertNotificationLogItem(notificationLogItem);
      }

      private void assertNotificationLogItem(NotificationLogItemInfo notificationLogItem)
      {
         NotificationLogItemInfo notificationLogItemFromDatabase =
            _database
               .GetNotificationLogItems()
               .SingleOrDefault(item => item.Id == notificationLogItem.Id);

         Assert.IsNotNull(notificationLogItemFromDatabase);
         Assert.AreEqual(notificationLogItem.CreditId, notificationLogItemFromDatabase.CreditId);
         Assert.AreEqual(notificationLogItem.PersonId, notificationLogItemFromDatabase.PersonId);
         Assert.AreEqual(notificationLogItem.NotificationDate, notificationLogItemFromDatabase.NotificationDate);
         Assert.AreEqual(notificationLogItem.Comment, notificationLogItemFromDatabase.Comment);
      }
   }
}