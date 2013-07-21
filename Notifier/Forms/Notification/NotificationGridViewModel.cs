using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using NLog;
using Notifier.Common;
using Notifier.Database;

namespace Notifier.Forms.Notification
{
   public sealed class NotificationGridViewModel : INotifyPropertyChanged
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly ContractRepositoryDecorator _repository;
      private ListCollectionView _payments;

      private DateTime _dateFrom;
      private DateTime _dateTo;

      public NotificationGridViewModel(ContractRepositoryDecorator repository, DateTime dateFrom, DateTime dateTo)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;

         _dateFrom = dateFrom;
         _dateTo = dateTo;

         _payments =
            new ListCollectionView(new ObservableCollection<NotNotifiedPayment>(getPayments()))
               {
                  Filter = getFilter(_dateFrom, _dateTo)
               };
      }

      public ListCollectionView Payments
      {
         get { return _payments; }
      }

      public void SaveChangedPayments()
      {
         foreach (NotNotifiedPayment payment in _payments)
         {
            if (payment.IsChanged)
            {
               _repository.UpdatePhoneNumber(payment.Id, payment.PhoneNumber);
               Logger.Info("Номер телефона \"{0}\" контракта \"{1}\" сохранен.",
                           payment.PhoneNumber, payment.ContractNumber);
               _repository.UpdateIsNotified(payment.PaymentId, payment.IsNotified);
               payment.IsChanged = false;
            }
         }
      }

      public void RefreshPayments()
      {
         _payments =
            new ListCollectionView(new ObservableCollection<NotNotifiedPayment>(getPayments()))
               {
                  Filter = getFilter(_dateFrom, _dateTo)
               };
         propertyChanged("Payments");
      }

      public void SetPeriod(DateTime dateFrom, DateTime dateTo)
      {
         _dateFrom = dateFrom;
         _dateTo = dateTo;
         _payments.Filter = getFilter(_dateFrom, _dateTo);
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void propertyChanged(string name)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
      }

      private IEnumerable<NotNotifiedPayment> getPayments()
      {
         var contracts = _repository.GetContracts();
         return contracts.SelectMany(contract => NotNotifiedPayment.CreateFromContract(contract, _repository));
      }

      private static Predicate<object> getFilter(DateTime dateFrom, DateTime dateTo)
      {
         return item =>
                   {
                      var payment = (NotNotifiedPayment) item;
                      return dateFrom <= payment.PaymentDate && payment.PaymentDate <= dateTo;
                   };
      }
   }
}