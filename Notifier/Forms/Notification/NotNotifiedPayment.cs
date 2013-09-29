using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using NLog;
using Notifier.Common;
using Notifier.Database;
using Notifier.Properties;
using Notifier.Sms;

namespace Notifier.Forms.Notification
{
   public sealed class NotNotifiedPayment : IDataErrorInfo, INotifyPropertyChanged
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      public static NotNotifiedPayment[] CreateFromContract(ContractLight contract, ContractRepositoryDecorator repository)
      {
         Check.NotNull(contract, "contract");
         Check.NotNull(repository, "repository");

         var payments = contract.Payments.ToArray();
         var result = new NotNotifiedPayment[payments.Length];

         for (var i = 0; i < result.Length; i++)
         {
            result[i] = new NotNotifiedPayment(contract, payments[i], repository);
         }

         return result;
      }

      private readonly ContractLight _contract;
      private readonly PaymentLight _payment;
      private readonly ContractRepositoryDecorator _repository;

      private NotNotifiedPayment(ContractLight contract, PaymentLight payment, ContractRepositoryDecorator repository)
      {
         Check.NotNull(contract, "contract");
         Check.NotNull(payment, "payment");
         Check.NotNull(repository, "repository");

         _contract = contract;
         _payment = payment;
         _repository = repository;
      }

      public int Id
      {
         get { return _contract.Id; }
      }

      public string ContractNumber
      {
         get { return _contract.ContractNumber; }
      }

      public string BorrowerName
      {
         get { return _contract.BorrowerName; }
      }

      public string PhoneNumber
      {
         get { return _contract.PhoneNumber; }
         set
         {
            if (_contract.PhoneNumber == value)
               return;

            _contract.PhoneNumber = value;
            IsChanged = true;

            propertyChanged("IsPhoneNumberCorrect");
         }
      }

      public decimal ExchangeRate
      {
         get { return _contract.ExchangeRate; }
      }

      public int PaymentId
      {
         get { return _payment.Id; }
      }

      public decimal PaymentAmount
      {
         get { return _payment.PaymentAmount; }
      }

      public DateTime PaymentDate
      {
         get { return _payment.PaymentDate; }
      }

      public bool IsNotified
      {
         get { return _payment.IsNotified; }
         set
         {
            if (_payment.IsNotified == value)
               return;

            _payment.IsNotified = value;
            IsChanged = true;

            propertyChanged("IsNotified");
         }
      }

      public ICommand SendSms
      {
         get
         {
            return new CommandDelegate(
               () =>
                  {
                     try
                     {
                        var smsSender = SmsSenderFactory.GetSmsSender(PhoneNumber);
                        smsSender.Send("MKK Standart Kredit, 1242000270769677, Bakai 124001. Prosim Vas proizvesti ezhemesyachnuyu vyplatu po kreditu.");

//                        _repository.UpdateIsNotified(_payment.Id, true);
//                        _payment.IsNotified = true;
//                        propertyChanged("IsNotified");
                     }
                     catch (UnknownMobileProviderException)
                     {
                        MessageBox.Show(Resources.UnknownMobileProvider, Resources.Error,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                     }
                     catch (Exception e)
                     {
                        MessageBox.Show(Resources.SendSmsError, Resources.Error,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logger.Error(e);
                     }
                  }
               );
         }
      }
      
      public bool IsChanged { get; set; }

      public bool IsPhoneNumberCorrect
      {
         get { return PhoneNumber != null && PhoneNumberRegex.PhoneNumberMatcher.IsMatch(PhoneNumber); }
      }
      
      [Todo("Сделать валидацию номера телефона")]
      public string this[string columnName]
      {
         get { return null; }
      }

      public string Error
      {
         get { return null; }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void propertyChanged(string name)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
   }
}