using System;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.NotificationLog
{
   public sealed class NotificationLogItemViewModel : ViewModelBase
   {
      private readonly NotificationLogItemInfo _notificationLogItem;

      public NotificationLogItemViewModel(NotificationLogItemInfo notificationLogItem)
      {
         Check.NotNull(notificationLogItem, "notificationLogItem");
         _notificationLogItem = notificationLogItem;
      }

      public NotificationLogItemInfo Original
      {
         get { return _notificationLogItem; }
      }

      public bool IsChanged { get; set; }

      public string CreditNumber
      {
         get { return _notificationLogItem.CreditNumber; }
      }

      public string PersonName
      {
         get { return _notificationLogItem.PersonName; }
      }

      public DateTime NotificationDate
      {
         get { return _notificationLogItem.NotificationDate; }
      }

      public string Comment
      {
         get { return _notificationLogItem.Comment; }
         set
         {
            if (_notificationLogItem.Comment == value)
               return;

            _notificationLogItem.Comment = value;
            propertyChanged("Comment");

            IsChanged = true;
         }
      }
   }
}