using Buzzer.DataAccess;
using Buzzer.Model;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.CreditContract;
using Buzzer.ViewModel.CreditsList;

namespace Buzzer.ViewModel.MainWindow
{
   public sealed class MainWindowViewModel
   {
      public MainWindowViewModel()
      {
         var creditRepository = new CreditRepository();

         var creditInfo = CreditInfo.CreatNew();
         var creditContractViewModel = new CreditContractViewModel(creditInfo, creditRepository);
         CurrentViewModel = creditContractViewModel;
//         CurrentViewModel = new CreditsListViewModel(creditRepository);

         Commands = new[]
                       {
                          new CommandViewModel("Список кредитов", new CommandDelegate(() => {})), 
                          new CommandViewModel("Новый кредит", new CommandDelegate(() => {})), 
                       };
      }

      public WorkspaceViewModel CurrentViewModel { get; private set; }

      public CommandViewModel[] Commands { get; private set; }
   }
}