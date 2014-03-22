using System;
using System.Collections.Generic;
using System.Linq;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.NotificationLog
{
   public sealed class NotificationLogItemViewModel : ViewModelBase
   {
      private readonly NotificationComment[] _comments;

      public NotificationLogItemViewModel(
         string creditNumber,
         string borrowerName,
         DateTime notificationDate,
         IEnumerable<NotificationLogItemInfo> notificationLogItems)
      {
         Check.NotNull(creditNumber, "creditNumber");
         Check.NotNull(borrowerName, "borrowerName");
         Check.NotNull(notificationLogItems, "notificationLogItems");

         CreditNumber = creditNumber;
         BorrowerName = borrowerName;
         NotificationDate = notificationDate;

         _comments = getComments(notificationLogItems);
      }

      public string CreditNumber { get; private set; }

      public string BorrowerName { get; private set; }

      public DateTime NotificationDate { get; private set; }

      public IEnumerable<NotificationComment> Comments
      {
         get { return _comments; }
      }

      public IEnumerable<NotificationComment> GetChangedComments()
      {
         return _comments.Where(item => item.IsChanged);
      }

      private NotificationComment[] getComments(IEnumerable<NotificationLogItemInfo> notificationLogItems)
      {
         return
            notificationLogItems
               .OrderBy(item => item.NotificationDate)
               .Select((item, index) => new NotificationComment(item, index + 1))
               .ToArray();
      }
   }
}