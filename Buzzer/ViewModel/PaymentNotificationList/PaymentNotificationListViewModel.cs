using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Buzzer.ViewModel.PaymentNotificationList
{
   public sealed class PaymentNotificationListViewModel : WorkspaceViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;
      private ListCollectionView _paymentNotificationList;

      private DateTime _fromDate;
      private DateTime _toDate;
      
      public PaymentNotificationListViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _buzzerDatabase = buzzerDatabase;
         _paymentNotificationList = getPayments();

         DisplayName = Resources.PaymentNotificationListViewModel_Caption;

         initFilterPeriod();
         updateFilter();
      }

      public DateTime FromDate
      {
         get { return _fromDate; }
         set
         {
            if (_fromDate == value)
               return;

            _fromDate = value;
            updateFilter();
         }
      }

      public DateTime ToDate
      {
         get { return _toDate; }
         set
         {
            if (_toDate == value)
               return;

            _toDate = value;
            updateFilter();
         }
      }

      public ListCollectionView PaymentNotificationList
      {
         get { return _paymentNotificationList; }
      }

      public ICommand UpdatePaymentNotificationList
      {
         get
         {
            return new CommandDelegate(updatePaymentNotificationList);
         }
      }

      public ICommand SavePaymentNotificationList
      {
         get { return new CommandDelegate(savePaymentNotificationList); }
      }

      private ListCollectionView getPayments()
      {
         var result = new List<PaymentNotificationViewModel>();

         foreach (CreditInfo credit in _buzzerDatabase.GetAllCredits())
         {
            foreach (PaymentInfo payment in credit.PaymentsSchedule)
            {
               result.Add(new PaymentNotificationViewModel(credit, payment, _buzzerDatabase));
            }
         }

         return new ListCollectionView(result);
      }

      private void initFilterPeriod()
      {
         _toDate = DateTime.Today;
         _fromDate = DateTime.Today.AddMonths(-1).AddDays(1);
      }

      private void updateFilter()
      {
         PaymentNotificationList.Filter =
            item =>
               {
                  var credit = (PaymentNotificationViewModel) item;
                  return filterByCreditIssueDate(credit);
               };
      }

      private bool filterByCreditIssueDate(PaymentNotificationViewModel payment)
      {
         DateTime date = payment.PaymentDate;
         return FromDate <= date && date <= ToDate;
      }

      private void updatePaymentNotificationList()
      {
         _paymentNotificationList = getPayments();
         updateFilter();
         propertyChanged("PaymentNotificationList");
      }

      private void savePaymentNotificationList()
      {
         try
         {
            foreach (PaymentNotificationViewModel item in _paymentNotificationList)
            {
               if (item.IsChanged)
               {
                  _buzzerDatabase.SavePayment(item.Orignal);
                  item.IsChanged = false;
               }
            }
         }
         catch (Exception e)
         {
            MessageBox.Show(Resources.ErrorWhileSavingInformationToDatabase,
                            Resources.BuzzerErrorMessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Logger.Error(e);
         }
      }
   }
}
