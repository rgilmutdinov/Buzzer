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

      private readonly NotificationGridViewModel _viewModel;
      private DateTimePicker _dateFrom;
      private DateTimePicker _dateTo;

      public NotificationForm(ContractRepositoryDecorator repository)
      {
         Check.NotNull(repository, "repository");

         InitializeComponent();
         addDateTimePickers();

         _viewModel = new NotificationGridViewModel(repository, _dateFrom.Value, _dateTo.Value);
         _elementHost.Child = new NotificationGridControl(_viewModel);
      }

      private void addDateTimePickers()
      {
         const string dateFormat = "dd/MM/yyyy";

         _toolStrip.Items.Add(new ToolStripLabel("С:"));
         _dateFrom = new DateTimePicker
                        {
                           Format = DateTimePickerFormat.Custom,
                           CustomFormat = dateFormat,
                           Width = 100,
                           Value = DateTime.Today.AddMonths(-1).AddDays(1),
                        };
         _dateFrom.ValueChanged += periodChanged;
         _toolStrip.Items.Add(new ToolStripControlHost(_dateFrom));

         _toolStrip.Items.Add(new ToolStripLabel("По:"));
         _dateTo = new DateTimePicker
                      {
                         Format = DateTimePickerFormat.Custom,
                         CustomFormat = dateFormat,
                         Width = 100,
                         Value = DateTime.Today
                      };
         _dateTo.ValueChanged += periodChanged;
         _toolStrip.Items.Add(new ToolStripControlHost(_dateTo));
      }

      private void periodChanged(object sender, EventArgs e)
      {
         if (_dateFrom.Value <= _dateTo.Value)
            _viewModel.SetPeriod(_dateFrom.Value, _dateTo.Value);
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
