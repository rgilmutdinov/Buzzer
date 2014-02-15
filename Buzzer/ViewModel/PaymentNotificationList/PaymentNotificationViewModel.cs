using System;
using System.Windows;
using System.Windows.Input;
using Buzzer.DomainModel.Models;
using Buzzer.DomainModel.Services;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Buzzer.ViewModel.PaymentNotificationList
{
   public sealed class PaymentNotificationViewModel : ViewModelBase
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly CreditInfo _credit;
      private readonly PaymentInfo _payment;

      public PaymentNotificationViewModel(CreditInfo credit, PaymentInfo payment)
      {
         Check.NotNull(credit, "credit");
         Check.NotNull(payment, "payment");

         _credit = credit;
         _payment = payment;
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

      public string BorrowerName
      {
         get { return _credit.Borrower.PersonName; }
      }

      public string PhoneNumber
      {
         get { return _credit.Borrower.PhoneNumbers[0].PhoneNumber; }
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

      public ICommand SendSmsToBakai
      {
         get
         {
            const string message = "MKK Standart Kredit, 1242000270769677, Bakai 124001. " +
                                   "Prosim Vas proizvesti ezhemesyachnuyu vyplatu po kreditu.";
            return new CommandDelegate(
               () => sendSms(message)
               );
         }
      }

      public ICommand SendSmsToKicb
      {
         get
         {
            const string message = "MKK Standart Kredit, 1280011014164142, KICB 128001. " +
                                   "Prosim Vas proizvesti ezhemesyachnuyu vyplatu po kreditu.";
            return new CommandDelegate(
               () => sendSms(message)
               );
         }
      }

      private void sendSms(string message)
      {
         try
         {
            var smsSender = SmsSenderFactory.GetSmsSender(PhoneNumber);
            smsSender.Send(message);
         }
         catch (UnknownMobileProviderException)
         {
            MessageBox.Show(Resources.UnknownMobileProvider,
                            Resources.BuzzerErrorMessageBoxCaption,
                            MessageBoxButton.OK, MessageBoxImage.Error);
         }
         catch (Exception e)
         {
            MessageBox.Show(Resources.SendSmsError,
                            Resources.BuzzerErrorMessageBoxCaption,
                            MessageBoxButton.OK, MessageBoxImage.Error);
            Logger.Error(e);
         }
      }
   }
}