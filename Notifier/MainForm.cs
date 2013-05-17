using System;
using System.Windows.Forms;
using NLog;
using Notifier.Common;
using Notifier.Database;
using Notifier.Forms.Notification;
using Notifier.Properties;

namespace Notifier
{
   public partial class MainForm : Form
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly ContractRepositoryDecorator _repository;

      public MainForm(ContractRepositoryDecorator repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;

         InitializeComponent();
      }

      private void mainFormShown(object sender, EventArgs e)
      {
         showNotificationForm();
      }

      private void notificationToolStripMenuItemClick(object sender, EventArgs e)
      {
         showNotificationForm();
      }

      private void contractsToolStripMenuItemClick(object sender, EventArgs e)
      {

      }

      private void showNotificationForm()
      {
         try
         {
            showForm(new NotificationForm(_repository) {WindowState = FormWindowState.Maximized});
         }
         catch (Exception e)
         {
            MessageBox.Show(Resources.ShowNotificationFormError, Resources.Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Error(e);
         }
      }

      private void showForm(Form form)
      {
         form.MdiParent = this;
         form.Show();
      }
   }
}
