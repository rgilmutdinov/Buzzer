using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.NotificationLog
{
   public sealed class NotificationComment : ViewModelBase
   {
      public NotificationComment(NotificationLogItemInfo notificationLogItem, int number)
      {
         Check.NotNull(notificationLogItem, "notificationLogItem");

         Original = notificationLogItem;
         Number = number;
      }

      public NotificationLogItemInfo Original { get; private set; }

      public bool IsChanged { get; set; }

      public int Number { get; private set; }

      public string Comment
      {
         get { return Original.Comment; }
         set
         {
            if (Original.Comment == value)
               return;

            Original.Comment = value;
            propertyChanged("Comment");

            IsChanged = true;
         }
      }
   }
}