using System;
using System.Linq;
using System.Windows.Data;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;
using DataAccess.Repository;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditsListViewModel : WorkspaceViewModel
   {
      private readonly BuzzerDatabase _buzzerDatabase;
      private DateTime _fromDate;
      private DateTime _toDate;

      public CreditsListViewModel(BuzzerDatabase buzzerDatabase, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         _buzzerDatabase = buzzerDatabase;
         
         CreditsList = new ListCollectionView(
            _buzzerDatabase
               .GetAllCredits()
               .Select(item => new CreditViewModel(item, workspaceManager))
               .ToList()
            );

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
   }
}