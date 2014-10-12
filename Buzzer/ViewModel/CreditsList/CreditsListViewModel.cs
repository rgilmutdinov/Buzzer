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

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditsListViewModel : WorkspaceViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly IWorkspaceManager _workspaceManager;

      private DateTime _fromDate;
      private DateTime _toDate;
      private string _creditNumberBorrowerNameFilter;

      private ICommand _updateCreditsListCommand;
      private ICommand _payOffCreditCommand;

      public CreditsListViewModel(BuzzerDatabase buzzerDatabase, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         Check.NotNull(workspaceManager, "workspaceManager");

         _buzzerDatabase = buzzerDatabase;
         _workspaceManager = workspaceManager;

         CreditsList = getCreditsList();
         DisplayName = Resources.CreditsListViewModel_Caption;

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

      public CreditViewModel SelectedCredit { get; set; }

      public ListCollectionView CreditsList { get; private set; }

      public ICommand UpdateCreditsList
      {
         get
         {
            if (_updateCreditsListCommand != null)
               return _updateCreditsListCommand;

            _updateCreditsListCommand = new CommandDelegate(updateCreditsList);
            return _updateCreditsListCommand;
         }
      }

      public ICommand PayOffCredit
      {
         get
         {
            if (_payOffCreditCommand != null)
               return _payOffCreditCommand;

            _payOffCreditCommand = new CommandDelegate(payOffCredit);
            return _payOffCreditCommand;
         }
      }

      private ListCollectionView getCreditsList()
      {
         return new ListCollectionView(
            _buzzerDatabase
               .GetAllCredits()
               .Where(item => item.RowState != RowState.Deleted)
               .Where(item => item.CreditState == CreditState.Current ||
                              item.CreditState == CreditState.Repayed)
               .Select(item => new CreditViewModel(item, _workspaceManager))
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
         CreditsList.Filter =
            item =>
               {
                  var credit = (CreditViewModel) item;
                  return filterByCreditIssueDate(credit) &&
                         filterByCreditNumberOrBorrowerName(credit);
               };
      }

      private bool filterByCreditIssueDate(CreditViewModel credit)
      {
         DateTime date = credit.CreditIssueDate;
         return FromDate <= date && date <= ToDate;
      }

      private bool filterByCreditNumberOrBorrowerName(CreditViewModel credit)
      {
         if (string.IsNullOrEmpty(_creditNumberBorrowerNameFilter))
            return true;

         return contains(credit.CreditNumber, _creditNumberBorrowerNameFilter) ||
                contains(credit.BorrowerName, _creditNumberBorrowerNameFilter);
      }

      private void updateCreditsList()
      {
         CreditsList = getCreditsList();
         updateFilter();
         propertyChanged("CreditsList");
      }

      private void payOffCredit()
      {
         if (SelectedCredit == null || SelectedCredit.CreditState == CreditState.Repayed)
            return;

         MessageBoxResult answer =
            MessageBox.Show(Resources.CreditsListViewModel_PayOffCreditMessageBoxMessage,
                            Resources.CreditsListViewModel_PayOffCreditMessageBoxCaption,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
         
         if (answer == MessageBoxResult.Yes)
         {
            CreditInfo creditInfo = SelectedCredit.Original;
            creditInfo.CreditState = CreditState.Repayed;
            
            try
            {
               _buzzerDatabase.SaveCredit(creditInfo);
               SelectedCredit.CreditState = creditInfo.CreditState;
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