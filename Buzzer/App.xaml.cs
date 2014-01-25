using System.Windows;
using Buzzer.DataAccess.Repository;
using Buzzer.ViewModel.MainWindow;

namespace Buzzer
{
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         var connectionString = "Server=localhost;Database=BuzzerDatabase;Trusted_Connection=True;";
         var buzzerDatabase = new BuzzerDatabase(connectionString);

         var viewModel = new MainWindowViewModel(buzzerDatabase);
         var mainView = new MainWindow {DataContext = viewModel};
         mainView.Show();
      }
   }
}
