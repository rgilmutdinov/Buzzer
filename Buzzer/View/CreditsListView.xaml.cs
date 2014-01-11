using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Buzzer.ViewModel.CreditsList;

namespace Buzzer.View
{
   public partial class CreditsListView : UserControl
   {
      public CreditsListView()
      {
         InitializeComponent();
      }

      private void rowLoaded(object sender, RoutedEventArgs e)
      {
         var row = (DataGridRow) sender;
         var credit = (CreditViewModel) row.DataContext;
         var gesture = new MouseGesture {MouseAction = MouseAction.LeftDoubleClick};
         row.InputBindings.Add(new MouseBinding(credit.OpenCredit, gesture));
      }
   }
}
