using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.CreditContract;
using DataAccess.Model;
using DataAccess.Repository;

namespace Buzzer.ViewModel.MainWindow
{
   public sealed class MainWindowViewModel
   {
      public MainWindowViewModel()
      {
         var creditRepository = new Repository("Server=localhost;Database=BuzzerDatabase;Trusted_Connection=True;");

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