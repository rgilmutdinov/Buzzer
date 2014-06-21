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

      private void rowLoaded(object sender, RoutedEventArgs e)
      {
         var row = (DataGridRow) sender;
         var logItem = (RegistrationLogItemViewModel) row.DataContext;
         var gesture = new MouseGesture {MouseAction = MouseAction.LeftDoubleClick};
         row.InputBindings.Add(new MouseBinding(logItem.OpenCredit, gesture));
      }
   }
}
