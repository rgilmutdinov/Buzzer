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
      
      private void onMouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         var row = (DataGridRow) sender;

         if (row.IsSelected)
         {
            var credit = (CreditViewModel) row.DataContext;
            credit.OpenCredit();
         }
      }
   }
}
