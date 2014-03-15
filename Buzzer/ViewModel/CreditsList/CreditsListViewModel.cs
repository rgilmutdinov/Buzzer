using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditsListViewModel : WorkspaceViewModel
   {
      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly IWorkspaceManager _workspaceManager;

      private DateTime _fromDate;
      private DateTime _toDate;
      private string _creditNumberBorrowerNameFilter;

      private ICommand _updateCreditsListCommand;

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

      private ListCollectionView getCreditsList()
      {
         return new ListCollectionView(
            _buzzerDatabase
               .GetAllCredits()
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

      private static bool contains(string text, string value)
      {
         return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
      }
   }
}