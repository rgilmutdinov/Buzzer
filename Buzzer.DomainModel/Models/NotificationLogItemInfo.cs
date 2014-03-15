using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class NotificationLogItemInfo : DomainObject
   {
      private NotificationLogItemInfo()
      {
      }

      public static NotificationLogItemInfo CreateNew(
         int creditId,
         int personId,
         DateTime notificationDate,
         string comment)
      {
         return new NotificationLogItemInfo
                   {
                      CreditId = creditId,
                      PersonId = personId,
                      NotificationDate = notificationDate,
                      Comment = comment
                   };
      }

      public static NotificationLogItemInfo Create(
         int id,
         int creditId,
         string creditNumber,
         int personId,
         string personName,
         DateTime notificationDate,
         string comment)
      {
         return new NotificationLogItemInfo
                   {
                      Id = id,
                      CreditId = creditId,
                      CreditNumber = creditNumber,
                      PersonId = personId,
                      PersonName = personName,
                      NotificationDate = notificationDate,
                      Comment = comment
                   };
      }

      public int CreditId { get; private set; }

      public string CreditNumber { get; private set; }

      public int PersonId { get; private set; }

      public string PersonName { get; private set; }

      public DateTime NotificationDate { get; private set; }

      public string Comment { get; set; }

      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "NotificationDate":
               return validateNotificationDate();

            case "Comment":
               return validateComment();
         }

         throw new ArgumentException(columnName, "columnName");
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[]
                   {
                      "NotificationDate"
                   };
      }

      private string validateNotificationDate()
      {
         return NotificationDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      private string validateComment()
      {
         return Comment.SafeGetLength() > 2000 ? string.Format(Resources.MaxLengthExceeded, 2000) : null;
      }
   }
}