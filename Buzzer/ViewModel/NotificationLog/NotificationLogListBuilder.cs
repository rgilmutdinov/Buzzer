using System;
using System.Collections.Generic;
using System.Linq;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.ViewModel.NotificationLog
{
   internal sealed class NotificationLogListBuilder
   {
      private readonly NotificationLogItemInfo[] _notificationLogItemInfos;

      public NotificationLogListBuilder(NotificationLogItemInfo[] notificationLogItemInfos)
      {
         Check.NotNull(notificationLogItemInfos, "notificationLogItemInfos");
         _notificationLogItemInfos = notificationLogItemInfos;
      }

      public List<NotificationLogItemViewModel> Build()
      {
         IEnumerable<NotificationLogItemViewModel> result =
            _notificationLogItemInfos
               .GroupBy(
                  logItem => new GroupKey(logItem),
                  (key, logItems) =>
                  new NotificationLogItemViewModel(key.CreditNumber,
                                                   key.PersonName,
                                                   key.NotificationDate,
                                                   logItems),
                  new Comparer()
               );

         return result.ToList();
      }

      private sealed class GroupKey
      {
         public GroupKey(NotificationLogItemInfo notificationLogItemInfo)
         {
            Check.NotNull(notificationLogItemInfo, "notificationLogItemInfo");

            CreditId = notificationLogItemInfo.CreditId;
            CreditNumber = notificationLogItemInfo.CreditNumber;
            PersonId = notificationLogItemInfo.PersonId;
            PersonName = notificationLogItemInfo.PersonName;
            NotificationDate = notificationLogItemInfo.NotificationDate.Date;
         }

         public int CreditId { get; private set; }
         public string CreditNumber { get; private set; }
         public int PersonId { get; private set; }
         public string PersonName { get; private set; }
         public DateTime NotificationDate { get; private set; }
      }

      private sealed class Comparer : IEqualityComparer<GroupKey>
      {
         public bool Equals(GroupKey x, GroupKey y)
         {
            return
               x.CreditId == y.CreditId &&
               x.PersonId == y.PersonId &&
               x.NotificationDate == y.NotificationDate;
         }

         public int GetHashCode(GroupKey obj)
         {
            return obj.NotificationDate.GetHashCode();
         }
      }
   }
}