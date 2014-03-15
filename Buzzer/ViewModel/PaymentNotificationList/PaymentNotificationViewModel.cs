using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.DomainModel.Services;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;
using NLog;

namespace Buzzer.ViewModel.PaymentNotificationList
{
   public sealed class PaymentNotificationViewModel : ViewModelBase
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
         get
         {
            ReadOnlyCollection<PhoneNumberInfo> phoneNumbers = _credit.Borrower.PhoneNumbers;

            if (phoneNumbers.Count > 0)
               return phoneNumbers[0].PhoneNumber;

            return string.Empty;
         }
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

            var logItem = createNotificationLogItemInfo(message);
            _buzzerDatabase.SaveNotificationLogItem(logItem);
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

      private NotificationLogItemInfo createNotificationLogItemInfo(string message)
      {
         DateTime notificationDate = DateTime.Now;
         string comment =
            string.Format("{0:dd/MM/yyyy HH:mm}. Получатель: {1}. Отправлено СМС-сообщение: \"{2}\".",
                          notificationDate, _credit.Borrower.PersonName, message);

         return NotificationLogItemInfo.CreateNew(_credit.Id, _credit.Borrower.Id,
                                                  notificationDate, comment);
      }
   }
}