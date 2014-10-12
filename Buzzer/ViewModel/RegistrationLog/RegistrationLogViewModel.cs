using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;
using NLog;

namespace Buzzer.ViewModel.RegistrationLog
{
   public sealed class RegistrationLogViewModel : WorkspaceViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly IWorkspaceManager _workspaceManager;

      private DateTime _fromDate;
      private DateTime _toDate;
      private string _borrowerNameFilter;

      public RegistrationLogViewModel(BuzzerDatabase buzzerDatabase, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         Check.NotNull(workspaceManager, "workspaceManager");

         _buzzerDatabase = buzzerDatabase;
         _workspaceManager = workspaceManager;

         RegistrationLogItems = getRegistrationLogItems();
         DisplayName = Resources.RegistrationLogViewModel_Caption;
         
         initFilterPeriod();
         updateFilter();
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

      public string BorrowerNameFilter
      {
         get { return _borrowerNameFilter; }
         set
         {
            if (_borrowerNameFilter == value)
               return;

            _borrowerNameFilter = value;
            updateFilter();
         }
      }

      public RegistrationLogItemViewModel SelectedRegistrationLogItem { get; set; }

      public ListCollectionView RegistrationLogItems { get; private set; }

      public ICommand UpdateRegistrationLog
      {
         get { return new CommandDelegate(updateRegistrationLog); }
      }

      public ICommand SaveRegistrationLog
      {
         get { return new CommandDelegate(saveRegistrationLog); }
      }

      public ICommand DeleteRegistrationLogItem
      {
         get { return new CommandDelegate(deleteRegistrationLogItem); }
      }

      private ListCollectionView getRegistrationLogItems()
      {
         return
            new ListCollectionView(
               _buzzerDatabase
                  .GetAllCredits()
                  .Where(item => item.RowState != RowState.Deleted)
                  .Select(item => new RegistrationLogItemViewModel(item, _workspaceManager))
                  .ToList()
               );
      }

      private void initFilterPeriod()
      {
         _fromDate = new DateTime(2009, 1, 1);
         _toDate = DateTime.Today;
      }

      private void updateFilter()
      {
         RegistrationLogItems.Filter =
            item =>
               {
                  var logItem = (RegistrationLogItemViewModel)item;
                  return filterByApplicationDate(logItem) &&
                         filterByBorrowerName(logItem);
               };
      }

      private bool filterByApplicationDate(RegistrationLogItemViewModel logItem)
      {
         if (!logItem.ApplicationDate.HasValue)
            return true;

         DateTime date = logItem.ApplicationDate.Value;
         return _fromDate <= date && date <= _toDate;
      }

      private bool filterByBorrowerName(RegistrationLogItemViewModel logItem)
      {
         if (string.IsNullOrEmpty(_borrowerNameFilter))
            return true;

         return contains(logItem.BorrowerName, _borrowerNameFilter);
      }

      private void updateRegistrationLog()
      {
         RegistrationLogItems = getRegistrationLogItems();
         updateFilter();
         propertyChanged("RegistrationLogItems");
      }

      private void saveRegistrationLog()
      {
         try
         {
            foreach (RegistrationLogItemViewModel item in RegistrationLogItems)
            {
               if (item.IsChanged)
               {
                  _buzzerDatabase.SaveCredit(item.Original);
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

      private void deleteRegistrationLogItem()
      {
         if (SelectedRegistrationLogItem == null)
            return;

         MessageBoxResult answer =
            MessageBox.Show(Resources.RegistrationLogViewModel_DeleteRegistrationLogItemMessageBoxMessage,
                            Resources.RegistrationLogViewModel_DeleteRegistrationLogItemMessageBoxCaption,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

         if (answer == MessageBoxResult.Yes)
         {
            CreditInfo credit = SelectedRegistrationLogItem.Original;
            credit.Delete();

            try
            {
               _buzzerDatabase.SaveCredit(credit);
               RegistrationLogItems.Remove(SelectedRegistrationLogItem);
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
      }
      
      private static bool contains(string text, string value)
      {
         return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
      }
   }
}
