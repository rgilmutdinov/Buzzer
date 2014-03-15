using System;
using System.Data;
using System.Data.Common;
using Buzzer.DataAccess.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   internal sealed class SaveNotificationLogItemCommand : CommandBase
   {
      new private static readonly FieldInfo CreditId = new FieldInfo("CreditID", DbType.Int32);
      new private static readonly FieldInfo PersonId = new FieldInfo("PersonID", DbType.Int32);
      private static readonly FieldInfo NotificationDate = new FieldInfo("NotificationDate", DbType.DateTime);
      private static readonly FieldInfo Comment = new FieldInfo("Comment", DbType.String, true);

      private readonly NotificationLogItemInfo _notificationLogItem;

      public SaveNotificationLogItemCommand(DbConnection connection, DbTransaction transaction, NotificationLogItemInfo notificationLogItem)
         : base(connection, transaction)
      {
         _notificationLogItem = notificationLogItem;
      }

      public void Execute()
      {
         if (_notificationLogItem.IsNew)
            saveNewNotificationLogItem();
         else
            editNotificationLogItem();
      }

      private void saveNewNotificationLogItem()
      {
         string query =
            string.Format(
               "INSERT INTO NotificationLog ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",
               CreditId.Name, PersonId.Name, NotificationDate.Name, Comment.Name,
               CreditId.ParameterName, PersonId.ParameterName, NotificationDate.ParameterName, Comment.ParameterName
               );

         using (DbCommand command = createCommand(query))
         {
            command.AddParameter(_notificationLogItem.CreditId, CreditId);
            command.AddParameter(_notificationLogItem.PersonId, PersonId);
            command.AddParameter(_notificationLogItem.NotificationDate, NotificationDate);
            command.AddParameter(_notificationLogItem.Comment, Comment);

            _notificationLogItem.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void editNotificationLogItem()
      {
         string query =
            string.Format(
               "UPDATE NotificationLog SET {0}={1} WHERE {2}={3}",
               Comment.Name, Comment.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(query))
         {
            command.AddParameter(_notificationLogItem.Comment, Comment);
            command.AddParameter(_notificationLogItem.Id, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}