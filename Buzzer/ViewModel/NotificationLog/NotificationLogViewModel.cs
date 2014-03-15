using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;
using NLog;

namespace Buzzer.ViewModel.NotificationLog
{
   public sealed class NotificationLogViewModel : WorkspaceViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;
      private ListCollectionView _notificationLogItems;

      private DateTime _fromDate;
      private DateTime _toDate;
      private string _creditNumberBorrowerNameFilter;

      public NotificationLogViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _buzzerDatabase = buzzerDatabase;
         _notificationLogItems = getNotificationLogItems();
         
         DisplayName = Resources.NotificationLogViewModel_Caption;

         initFilterPeriod();
         updateFilter();
      }

      public ListCollectionView NotificationLogItems
      {
         get
         {
            SortDescriptionCollection sortDescriptions = _notificationLogItems.SortDescriptions;
            sortDescriptions.Add(new SortDescription("NotificationDate", ListSortDirection.Descending));
            return _notificationLogItems;
         }
      }

      public ICommand Update
      {
         get { return new CommandDelegate(update); }
      }

      public ICommand Save
      {
         get { return new CommandDelegate(save); }
      }

      public DateTime FromDate
      {
         get { return _fromDate; }
         set
         {
            if (_fromDate == value)
               return;

            _fromDate = value;
            updateFilter();
         }
      }

      public DateTime ToDate
      {
         get { return _toDate; }
         set
         {
            if (_toDate == value)
               return;

            _toDate = value;
            updateFilter();
         }
      }

      public string CreditNumberBorrowerNameFilter
      {
         get { return _creditNumberBorrowerNameFilter; }
         set
         {
            if (_creditNumberBorrowerNameFilter == value)
               return;

            _creditNumberBorrowerNameFilter = value;
            updateFilter();
         }
      }

      private void save()
      {
         try
         {
            foreach (NotificationLogItemViewModel item in NotificationLogItems)
            {
               if (item.IsChanged)
               {
                  _buzzerDatabase.SaveNotificationLogItem(item.Original);
                  item.IsChanged = false;
               }
            }
         }
         catch (Exception e)
         {
            MessageBox.Show(Resources.ErrorWhileSavingInformationToDatabase,
                            Resources.BuzzerErrorMessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Logger.Error(e);
         }
      }

      private void update()
      {
         _notificationLogItems = getNotificationLogItems();
         propertyChanged("NotificationLogItems");
      }

      private ListCollectionView getNotificationLogItems()
      {
         return
            new ListCollectionView(
               _buzzerDatabase
                  .GetNotificationLogItems()
                  .Select(item => new NotificationLogItemViewModel(item))
                  .ToList()
               );
      }

      private void initFilterPeriod()
      {
         _toDate = DateTime.Today;
         _fromDate = DateTime.Today.AddMonths(-1).AddDays(1);
      }

      private void updateFilter()
      {
         NotificationLogItems.Filter =
            item =>
               {
                  var notificationLogItem = (NotificationLogItemViewModel) item;
                  return filterByNotificationDate(notificationLogItem) &&
                         filterByCreditNumberOrBorrowerName(notificationLogItem);
               };
      }

      private bool filterByNotificationDate(NotificationLogItemViewModel credit)
      {
         DateTime notificationDate = credit.NotificationDate.Date;
         return FromDate <= notificationDate && notificationDate <= ToDate;
      }

      private bool filterByCreditNumberOrBorrowerName(NotificationLogItemViewModel credit)
      {
         if (string.IsNullOrEmpty(_creditNumberBorrowerNameFilter))
            return true;

         return contains(credit.CreditNumber, _creditNumberBorrowerNameFilter) ||
                contains(credit.PersonName, _creditNumberBorrowerNameFilter);
      }

      private static bool contains(string text, string value)
      {
         return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
      }
   }
}
