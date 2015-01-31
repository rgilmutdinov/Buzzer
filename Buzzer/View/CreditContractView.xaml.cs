using System.Windows;
using System.Windows.Controls;

namespace Buzzer.View
{
   public partial class CreditContractView : UserControl
   {
      public CreditContractView()
      {
         InitializeComponent();
      }

      private void expanderExpanded(object sender, RoutedEventArgs e)
      {
         _scrollViewer.ScrollToBottom();
      }
   }
}
