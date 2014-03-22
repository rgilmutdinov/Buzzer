using System;
using System.Collections.Generic;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.PaymentNotificationList
{
   public sealed class PaymentNotificationViewModel : ViewModelBase
   {
      private readonly CreditInfo _credit;
      private readonly PaymentInfo _payment;
      private readonly BuzzerDatabase _buzzerDatabase;

      public PaymentNotificationViewModel(CreditInfo credit, PaymentInfo payment, BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(credit, "credit");
         Check.NotNull(payment, "payment");
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _credit = credit;
         _payment = payment;
         _buzzerDatabase = buzzerDatabase;

         SmsReceivers = getSmsReceivers();
      }

      public PaymentInfo Orignal
      {
         get { return _payment; }
      }

      public bool IsChanged { get; set; }

      public bool IsNotified
      {
         get { return _payment.IsNotified; }
         set
         {
            if (_payment.IsNotified == value)
               return;

            _payment.IsNotified = value;
            propertyChanged("IsNotified");

            IsChanged = true;
         }
      }

      public string CreditNumber
      {
         get { return _credit.CreditNumber; }
      }

      public decimal? ExchangeRate
      {
         get { return _credit.ExchangeRate; }
      }

      public string PaymentAmount
      {
         get
         {
            string currency = _credit.ExchangeRate.HasValue ? "USD" : "KGS";
            return string.Format("{0} {1}", _payment.PaymentAmount, currency);
         }
      }

      public DateTime PaymentDate
      {
         get { return _payment.PaymentDate; }
      }

      public IEnumerable<SmsReceiverViewModel> SmsReceivers { get; private set; }

      private IEnumerable<SmsReceiverViewModel> getSmsReceivers()
      {
         return new[] {new SmsReceiverViewModel(_credit, _credit.Borrower, _credit.Guarantors, _buzzerDatabase)};
      }
   }
}