namespace Notifier.Forms.Notification
{
   public partial class NotificationGridControl
   {
      public NotificationGridControl(NotificationGridViewModel viewModel)
      {
         InitializeComponent();
         DataContext = viewModel;
      }
   }
}
