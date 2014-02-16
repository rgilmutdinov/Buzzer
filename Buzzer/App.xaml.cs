using System;
using System.Configuration;
using System.Windows;
using Buzzer.DataAccess.Repository;
using Buzzer.ViewModel.MainWindow;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Buzzer
{
   public partial class App : Application
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         DispatcherUnhandledException += onUnhandledException;

         ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["BuzzerDatabase"];
         var buzzerDatabase = new BuzzerDatabase(connectionString.ConnectionString);

         var viewModel = new MainWindowViewModel(buzzerDatabase);
         var mainView = new MainWindow {DataContext = viewModel};
         mainView.Show();
      }

      private void onUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
      {
         Logger.Error(e.Exception);
         MessageBox.Show("Атай, свяжись со мной!!! Упала критическая ошибка :(", "Ахтунг",
                         MessageBoxButton.OK, MessageBoxImage.Error);
      }
   }
}
