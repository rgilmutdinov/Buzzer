using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class TodoItem : DomainObject
   {
      private TodoItem()
      {
      }

      public static TodoItem CreateNew(int creditId)
      {
         return new TodoItem
                   {
                      CreditId = creditId,
                      Description = string.Empty,
                      State = TodoItemState.None,
                      NotificationCount = 0,
                      NotificationDate = null
                   };
      }
      
      public static TodoItem Create(
         int id,
         int creditId,
         string description,
         TodoItemState state,
         int notificationCount,
         DateTime? notificationDate
         )
      {
         return new TodoItem
                   {
                      Id = id,
                      CreditId = creditId,
                      Description = description,
                      State = state,
                      NotificationCount = notificationCount,
                      NotificationDate = notificationDate
                   };
      }

      public int CreditId { get; set; }

      public string Description { get; set; }

      public TodoItemState State { get; set; }

      public int NotificationCount { get; set; }

      public DateTime? NotificationDate { get; set; }

      public void Notified()
      {
         if (NotificationDate.HasValue && NotificationDate.Value == DateTime.Today)
         {
            NotificationCount++;
         }
         else
         {
            NotificationDate = DateTime.Today;
            NotificationCount = 1;
         }
      }

      protected override string getErrorInfo(string columnName)
      {
         if (columnName == "Description")
            return validateDescription();

         throw new ArgumentException(columnName, "columnName");
      }
      
      protected override IEnumerable<string> getRequiredFields()
      {
         return new[] {"Description"};
      }

      private string validateDescription()
      {
         if (Description.SafeGetLength() > 255)
            return string.Format(Resources.MaxLengthExceeded, 255);

         return string.IsNullOrEmpty(Description) ? Resources.FieldMustBeFilled : null;
      }
   }
}