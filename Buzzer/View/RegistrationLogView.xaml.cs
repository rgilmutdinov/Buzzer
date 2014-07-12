using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Buzzer.ViewModel.RegistrationLog;

namespace Buzzer.View
{
   public partial class RegistrationLogView : UserControl
   {
      public RegistrationLogView()
      {
         InitializeComponent();
      }

      private void MenuItemClicked(object sender, RoutedEventArgs e)
      {
         var menuItem = (MenuItem) sender;
         _refusalReasonColumn.Visibility = menuItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
      }

      private void onMouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         var row = (DataGridRow) sender;

         if (row.IsSelected)
         {
            var logItem = (RegistrationLogItemViewModel) row.DataContext;
            logItem.OpenCredit();
         }
      }
   }
}
