using System;
using System.Windows.Forms;
using NLog;
using Notifier.Common;
using Notifier.Database;
using Notifier.Properties;

namespace Notifier.Forms.Notification
{
   public partial class NotificationForm : Form
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly ContractRepositoryDecorator _repository;
      private readonly NotificationGridViewModel _viewModel;

      public NotificationForm(ContractRepositoryDecorator repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;

         InitializeComponent();

         _viewModel = new NotificationGridViewModel(repository);
         _elementHost.Child = new NotificationGridControl(_viewModel);
      }

      private void saveButtonClick(object sender, EventArgs e)
      {
         try
         {
            _viewModel.SaveChangedPayments();
         }
         catch (Exception exc)
         {
            MessageBox.Show(Resources.SavePhoneNumbersError, Resources.Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Error(exc);
         }
      }

      private void refreshButtonClick(object sender, EventArgs e)
      {
         try
         {
            _viewModel.RefreshPayments();
         }

         catch (Exception exc)
         {
            MessageBox.Show(Resources.RefreshNotificationListError, Resources.Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Error(exc);
         }
      }
   }
}
