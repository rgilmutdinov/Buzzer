using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NLog;
using Notifier.Common;
using Notifier.Database;

namespace Notifier.Forms.Notification
{
   public sealed class NotificationGridViewModel : INotifyPropertyChanged
   {
      private const int TwoWeeksDiff = -13;

      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly ContractRepositoryDecorator _repository;
      private ObservableCollection<NotNotifiedPayment> _payments;

      public NotificationGridViewModel(ContractRepositoryDecorator repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;

         _payments = new ObservableCollection<NotNotifiedPayment>(getPayments());
      }

      public ObservableCollection<NotNotifiedPayment> Payments
      {
         get { return _payments; }
      }

      public void SaveChangedPayments()
      {
         foreach (var payment in _payments)
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
         _payments = new ObservableCollection<NotNotifiedPayment>(getPayments());
         propertyChanged("Payments");
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void propertyChanged(string name)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
      }

      private IEnumerable<NotNotifiedPayment> getPayments()
      {
         var today = DateTime.Today;
         var contracts = _repository.GetContracts(today.AddDays(TwoWeeksDiff), today);
         return contracts.SelectMany(contract => NotNotifiedPayment.CreateFromContract(contract, _repository));
      }
   }
}