using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class TodoItemViewModel : ViewModelBase, IDataErrorInfo
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly TodoItem _todoItem;
      private readonly ObservableCollection<PhoneNumberViewModel> _phoneNumbers;
      private readonly BuzzerDatabase _buzzerDatabase;
      private ICommand _sendSmsCommand;

      public TodoItemViewModel(TodoItem todoItem, ObservableCollection<PhoneNumberViewModel> phoneNumbers, BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(todoItem, "todoItem");
         Check.NotNull(phoneNumbers, "phoneNumbers");
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _todoItem = todoItem;
         _phoneNumbers = phoneNumbers;
         _buzzerDatabase = buzzerDatabase;

         SelectedPhoneNumber = _phoneNumbers.FirstOrDefault();
      }
      
      public TodoItem Original
      {
         get { return _todoItem; }
      }

      public string Description
      {
         get { return _todoItem.Description; }
         set
         {
            if (_todoItem.Description == value)
               return;

            _todoItem.Description = value;
            propertyChanged("Description");
         }
      }

      public TodoItemState State
      {
         get { return _todoItem.State; }
         set
         {
            if (_todoItem.State == value)
               return;

            _todoItem.State = value;
            propertyChanged("State");
         }
      }

      public int NotificationCount
      {
         get
         {
            DateTime? notificationDate = _todoItem.NotificationDate;

            if (notificationDate.HasValue && notificationDate.Value == DateTime.Today)
               return _todoItem.NotificationCount;

            return 0;
         }
      }

      public PhoneNumberViewModel SelectedPhoneNumber { get; set; }

      public ObservableCollection<PhoneNumberViewModel> PhoneNumbers
      {
         get { return _phoneNumbers; }
      }

      public ICommand SendSms
      {
         get
         {
            if (_sendSmsCommand != null)
               return _sendSmsCommand;

            _sendSmsCommand = new CommandDelegate(sendSms, canSendSms);
            return _sendSmsCommand;
         }
      }

      public string this[string columnName]
      {
         get
         {
            if (columnName == "Description")
               return ((IDataErrorInfo) _todoItem)[columnName];

            return null;
         }
      }

      public string Error
      {
         get { return null; }
      }

      [Todo("Уточнить у Атая, каким образом формировать сообщения для заемщика")]
      private void sendSms()
      {
         try
         {
            ISmsSender smsSender = SmsSenderFactory.GetSmsSender(SelectedPhoneNumber.PhoneNumber);
            smsSender.Send(Description);

            _todoItem.Notified();
            _buzzerDatabase.SaveTodoItem(_todoItem);

            notificationCountChanged();
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

      private bool canSendSms()
      {
         return SelectedPhoneNumber != null &&
                SelectedPhoneNumber["PhoneNumber"] == null &&
                !string.IsNullOrEmpty(Description) &&
                _todoItem.CreditId != NullValues.Id;
      }

      private void notificationCountChanged()
      {
         propertyChanged("NotificationCount");
      }
   }
}