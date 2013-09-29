using System.Windows;
using Buzzer.ViewModel;
using Buzzer.ViewModel.MainWindow;

namespace Buzzer
{
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         var viewModel = new MainWindowViewModel();
         var mainView = new MainWindow {DataContext = viewModel};
         mainView.Show();
      }
   }
}
