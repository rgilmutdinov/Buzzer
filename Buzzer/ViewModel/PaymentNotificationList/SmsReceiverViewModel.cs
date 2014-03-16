using System;
using System.Collections.Generic;
using System.Linq;
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
   public sealed class SmsReceiverViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly PersonInfo _parent;
      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly string[] _phoneNumbers;

      public SmsReceiverViewModel(
         PersonInfo parent,
         IEnumerable<PersonInfo> children,
         BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(parent, "parent");
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _parent = parent;
         _buzzerDatabase = buzzerDatabase;
         _phoneNumbers = getPhoneNumbers();

         SelectedPhoneNumber = getSelectedPhoneNumber();
         Children = getChildren(children);
      }

      public string PersonName
      {
         get { return _parent.PersonName; }
      }

      public string SelectedPhoneNumber { get; set; }

      public IEnumerable<string> PhoneNumbers
      {
         get { return _phoneNumbers; }
      }

      public IEnumerable<SmsReceiverViewModel> Children { get; private set; }

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
            if (SelectedPhoneNumber == null)
               return;

            var smsSender = SmsSenderFactory.GetSmsSender(SelectedPhoneNumber);
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
                          notificationDate, _parent.PersonName, message);

         return NotificationLogItemInfo.CreateNew(_parent.CreditId, _parent.Id, notificationDate, comment);
      }

      private string[] getPhoneNumbers()
      {
         return _parent.PhoneNumbers.Select(item => item.PhoneNumber).ToArray();
      }

      private string getSelectedPhoneNumber()
      {
         return _phoneNumbers.Length > 0 ? _phoneNumbers[0] : null;
      }

      private IEnumerable<SmsReceiverViewModel> getChildren(IEnumerable<PersonInfo> children)
      {
         return
            children == null
               ? Enumerable.Empty<SmsReceiverViewModel>()
               : children.Select(item => new SmsReceiverViewModel(item, null, _buzzerDatabase)).ToArray();
      }
   }
}