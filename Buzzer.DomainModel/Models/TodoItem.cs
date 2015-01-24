using System;
using System.Collections.Generic;
using System.Linq;

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

      protected override string getErrorInfo(string columnName)
      {
         return null;
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return Enumerable.Empty<string>();
      }
   }
}