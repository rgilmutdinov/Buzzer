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
         DateTime today = DateTime.Today;
         _fromDate = new DateTime(today.Year, today.Month, 1);
         _toDate = _fromDate.AddMonths(1).AddDays(-1);
      }

      private void updateFilter()
      {
         CreditsList.Filter =
            item =>
               {
                  var credit = (CreditViewModel) item;
                  DateTime date = credit.CreditIssueDate;
                  return FromDate <= date && date <= ToDate;
               };
      }

      private void updateCreditsList()
      {
         CreditsList = getCreditsList();
         updateFilter();
         propertyChanged("CreditsList");
      }
   }
}