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

         updateFilter();
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

      private void updateFilter()
      {
         CreditsList.Filter =
            item =>
               {
                  var credit = (CreditViewModel) item;
                  return filterByCreditNumberOrBorrowerName(credit);
               };
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