using System.Collections.ObjectModel;
using System.Linq;
using Buzzer.ViewModel.Common;
using Common;
using DataAccess.Repository;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditsListViewModel : WorkspaceViewModel
   {
      private readonly BuzzerDatabase _buzzerDatabse;

      public CreditsListViewModel(BuzzerDatabase buzzerDatabse)
      {
         Check.NotNull(buzzerDatabse, "buzzerDatabse");
         _buzzerDatabse = buzzerDatabse;

         CreditsList =
            new ObservableCollection<CreditViewModel>(
               _buzzerDatabse.GetAllCredits().Select(item => new CreditViewModel(item))
               );
      }

      public ObservableCollection<CreditViewModel> CreditsList { get; private set; }
   }
}