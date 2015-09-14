using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.DomainModel.Services;
using Buzzer.Properties;
using Buzzer.View;
using Buzzer.ViewModel.Common;
using Common;
using NLog;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class CreditContractViewModel : WorkspaceViewModel, IDataErrorInfo
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly CreditInfo _creditInfo;
      private readonly PersonInfo _borrower;
      private bool _isUsdRateEnabled;

      private readonly RequiredCreditDocuments[] _requiredCreditDocuments;

      private ICommand _buildPaymentsScheduleCommand;
      private ICommand _addGuarantorCommand;
      private ICommand _removeGuarantorCommand;
      private ICommand _addTodoItemCommand;
      private ICommand _removeTodoItemCommand;
      private ICommand _saveCommand;
      private ICommand _refuseCommand;
      private ICommand _addPayoffCommand;
      private ICommand _updatePayoffCommand;
      private ICommand _removePayoffCommand;

      public CreditContractViewModel(CreditInfo creditInfo, BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(creditInfo, "creditInfo");
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _creditInfo = creditInfo;
         _buzzerDatabase = buzzerDatabase;

         _borrower = _creditInfo.Borrower;
         Borrower = new PersonInfoViewModel(_borrower);
         _isUsdRateEnabled = _creditInfo.ExchangeRate.HasValue;

         Guarantors = getGuarantors();
         PaymentsSchedule = getPaymentsSchedule();

         Payoffs = getPayoffs();

         TodoList = getTodoList();
         CreditTypes = getCreditTypes();

         _requiredCreditDocuments = getRequiredCreditDocuments();

         SelectedPhoneNumber = Borrower.PhoneNumbers.FirstOrDefault();

         if (canBuildPaymentsSchedule())
            buildDetailedPayments();

         DisplayName = getDisplayName();
      }

      #region Fields

      public CreditState CreditState
      {
         get { return _creditInfo.CreditState; }
      }

      public string CreditNumber
      {
         get { return _creditInfo.CreditNumber; }
         set
         {
            if (_creditInfo.CreditNumber == value)
               return;

            _creditInfo.CreditNumber = value;
            propertyChanged("CreditNumber");

            DisplayName = _creditInfo.CreditNumber;
         }
      }

      public DateTime? ApplicationDate
      {
         get { return _creditInfo.ApplicationDate; }
         set
         {
            if (_creditInfo.ApplicationDate == value)
               return;

            _creditInfo.ApplicationDate = value;
            propertyChanged("ApplicationDate");
         }
      }

      public decimal CreditAmount
      {
         get { return _creditInfo.CreditAmount; }
         set
         {
            if (_creditInfo.CreditAmount == value)
               return;

            _creditInfo.CreditAmount = value;
            propertyChanged("CreditAmount");
         }
      }

      public DateTime CreditIssueDate
      {
         get { return _creditInfo.CreditIssueDate; }
         set
         {
            if (_creditInfo.CreditIssueDate == value)
               return;

            _creditInfo.CreditIssueDate = value.Date;
            propertyChanged("CreditIssueDate");
         }
      }

      public int MonthsCount
      {
         get { return _creditInfo.MonthsCount; }
         set
         {
            if (_creditInfo.MonthsCount == value)
               return;

            _creditInfo.MonthsCount = value;
            propertyChanged("MonthsCount");
         }
      }

      public decimal DiscountRate
      {
         get { return _creditInfo.DiscountRate*100; }
         set
         {
            if (_creditInfo.DiscountRate == value)
               return;

            _creditInfo.DiscountRate = value/100;
            propertyChanged("DiscountRate");
         }
      }

      public decimal? EffectiveDiscountRate
      {
         get { return _creditInfo.EffectiveDiscountRate*100; }
         set
         {
            if (_creditInfo.EffectiveDiscountRate == value)
               return;

            _creditInfo.EffectiveDiscountRate = value/100;
            propertyChanged("EffectiveDiscountRate");
         }
      }

      public decimal UsdRate
      {
         get { return _creditInfo.ExchangeRate.HasValue ? _creditInfo.ExchangeRate.Value : decimal.Zero; }
         set
         {
            if (_creditInfo.ExchangeRate == value)
               return;

            _creditInfo.ExchangeRate = value;
            propertyChanged("UsdRate");
         }
      }

      public PersonInfoViewModel Borrower { get; private set; }

      [Todo("Если value==true, то выставлять текущий курс валюты доллара США")]
      public bool IsUsdRateEnabled
      {
         get { return _isUsdRateEnabled; }
         set
         {
            if (_isUsdRateEnabled == value)
               return;

            if (!value)
               _creditInfo.ExchangeRate = null;

            _isUsdRateEnabled = value;

            propertyChanged("IsUsdRateEnabled");
            propertyChanged("UsdRate");
         }
      }

      public string NotificationDescription
      {
         get { return _creditInfo.NotificationDescription; }
         set
         {
            if (_creditInfo.NotificationDescription == value)
               return;

            _creditInfo.NotificationDescription = value;
            propertyChanged("NotificationDescription");
         }
      }

      public int NotificationCount
      {
         get
         {
            var notificationDate = _creditInfo.NotificationDate;

            if (notificationDate.HasValue && notificationDate.Value == DateTime.Today)
               return _creditInfo.NotificationCount;

            return 0;
         }
      }

      public PersonInfoViewModel SelectedGuarantor { get; set; }

      public ObservableCollection<PersonInfoViewModel> Guarantors { get; private set; }

      public PaymentInfoViewModel[] PaymentsSchedule { get; private set; }

      public ObservableCollection<PayoffViewModel> Payoffs { get; private set; }

      public ObservableCollection<PaymentAdvanceViewModel> PaymentsProgress { get; private set; } 

      public PayoffViewModel SelectedPayoff { get; set; }

      public TodoItemViewModel SelectedTodoItem { get; set; }

      public ObservableCollection<TodoItemViewModel> TodoList { get; private set; }

      public CreditType[] CreditTypes { get; private set; }

      public CreditType SelectedCreditType
      {
         get
         {
            var creditType = _creditInfo.CreditType;

            if (creditType == null)
               return null;

            return CreditTypes.Single(item => item.Id == creditType.Id);
         }
         set
         {
            var creditType = _creditInfo.CreditType;

            if (creditType != null && creditType.Id == value.Id)
               return;

            if (!userAcceptsChangingCreditType())
               return;

            _creditInfo.CreditType = value;

            removeOldDocuments();
            addNewDocuments();

            propertyChanged("SelectedCreditType");
            propertyChanged("RequiredDocuments");
         }
      }

      public IEnumerable<RequiredDocument> RequiredDocuments
      {
         get { return _creditInfo.RequiredDocuments; }
      }

      public PhoneNumberViewModel SelectedPhoneNumber { get; set; }

      public ObservableCollection<PhoneNumberViewModel> PhoneNumbers
      {
         get { return Borrower.PhoneNumbers; }
      }

      #endregion

      #region Commands

      public ICommand BuildPaymentsSchedule
      {
         get
         {
            if (_buildPaymentsScheduleCommand != null)
               return _buildPaymentsScheduleCommand;

            _buildPaymentsScheduleCommand = new CommandDelegate(buildPaymentsSchedule, canBuildPaymentsSchedule);
            return _buildPaymentsScheduleCommand;
         }
      }

      public ICommand AddGuarantor
      {
         get
         {
            if (_addGuarantorCommand != null)
               return _addGuarantorCommand;

            _addGuarantorCommand = new CommandDelegate(addGuarantor);
            return _addGuarantorCommand;
         }
      }

      public ICommand RemoveGuarantor
      {
         get
         {
            if (_removeGuarantorCommand != null)
               return _removeGuarantorCommand;

            _removeGuarantorCommand = new CommandDelegate(removeGuarantor, canRemoveGuarantor);
            return _removeGuarantorCommand;
         }
      }

      public ICommand AddTodoItem
      {
         get
         {
            if (_addTodoItemCommand != null)
               return _addTodoItemCommand;

            _addTodoItemCommand = new CommandDelegate(addTodoItem);
            return _addTodoItemCommand;
         }
      }

      public ICommand RemoveTodoItem
      {
         get
         {
            if (_removeTodoItemCommand != null)
               return _removeTodoItemCommand;

            _removeTodoItemCommand = new CommandDelegate(removeTodoItem, canRemoveTodoItem);
            return _removeTodoItemCommand;
         }
      }

      public ICommand Save
      {
         get
         {
            if (_saveCommand != null)
               return _saveCommand;

            _saveCommand = new CommandDelegate(save, canSave);
            return _saveCommand;
         }
      }

      public ICommand Refuse
      {
         get
         {
            if (_refuseCommand != null)
               return _refuseCommand;

            _refuseCommand = new CommandDelegate(refuse, canRefuse);
            return _refuseCommand;
         }
      }

      public ICommand AddPayoff
      {
         get
         {
            if (_addPayoffCommand != null)
               return _addPayoffCommand;

            _addPayoffCommand = new CommandDelegate(addPayoff);
            return _addPayoffCommand;
         }
      }

      public ICommand UpdatePayoff
      {
         get
         {
            if (_updatePayoffCommand != null)
               return _updatePayoffCommand;

            _updatePayoffCommand = new CommandDelegate(updatePayoff, canUpdatePayoff);
            return _updatePayoffCommand;
         }
      }

      public ICommand RemovePayoff
      {
         get
         {
            if (_removePayoffCommand != null)
               return _removePayoffCommand;

            _removePayoffCommand = new CommandDelegate(removePayoff, canRemovePayoff);
            return _removePayoffCommand;
         }
      }

      private ICommand _sendSmsCommand;

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

      #endregion

      #region IDataErrorInfo Members

      string IDataErrorInfo.this[string columnName]
      {
         get
         {
            string error = null;

            switch (columnName)
            {
               case "CreditNumber":
               case "ApplicationDate":
               case "CreditAmount":
               case "CreditIssueDate":
               case "MonthsCount":
               case "DiscountRate":
               case "EffectiveDiscountRate":
               case "Guarantors":
                  error = (_creditInfo as IDataErrorInfo)[columnName];
                  break;

               case "UsdRate":
                  error = (_creditInfo as IDataErrorInfo)["ExchangeRate"];
                  break;
            }

            CommandManager.InvalidateRequerySuggested();

            return error;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }

      #endregion

      private ObservableCollection<PersonInfoViewModel> getGuarantors()
      {
         return new ObservableCollection<PersonInfoViewModel>(
            _creditInfo.Guarantors.Select(item => new PersonInfoViewModel(item))
            );
      }

      private ObservableCollection<PayoffViewModel> getPayoffs()
      {
         return new ObservableCollection<PayoffViewModel>(
            _creditInfo.Payoffs.Select(payoff => new PayoffViewModel(payoff, IsUsdRateEnabled))
            );
      }

      private PaymentInfoViewModel[] getPaymentsSchedule()
      {
         if (_creditInfo.PaymentsSchedule.Length == 0)
            return new PaymentInfoViewModel[0];

         var result = new PaymentInfoViewModel[MonthsCount];

         for (var i = 0; i < MonthsCount; i++)
         {
            var number = i + 1;
            result[i] = new PaymentInfoViewModel(_creditInfo.PaymentsSchedule[i], number, IsUsdRateEnabled);
         }

         return result;
      }

      private PaymentAdvanceViewModel[] getPaymentsProgress()
      {
         if (_creditInfo.PaymentsProgress.Length == 0)
            return new PaymentAdvanceViewModel[0];

         var result = new PaymentAdvanceViewModel[MonthsCount];

         for (var i = 0; i < MonthsCount; i++)
         {
            result[i] = new PaymentAdvanceViewModel(_creditInfo.PaymentsProgress[i]);
         }

         return result;
      }

      private ObservableCollection<TodoItemViewModel> getTodoList()
      {
         return new ObservableCollection<TodoItemViewModel>(
            _creditInfo.TodoList.Select(item => new TodoItemViewModel(item, Borrower.PhoneNumbers, _buzzerDatabase))
            );
      }

      private CreditType[] getCreditTypes()
      {
         return _buzzerDatabase.GetAllCreditTypes();
      }

      private RequiredCreditDocuments[] getRequiredCreditDocuments()
      {
         return _buzzerDatabase.GetAllRequiredCreditDocuments();
      }

      private string getDisplayName()
      {
         return _creditInfo.IsNew ? Resources.NewCreditTabCaption : _creditInfo.CreditNumber;
      }

      private bool userAcceptsChangingCreditType()
      {
         var answer =
            MessageBox.Show(Resources.CreditContractViewModel_ChangeCreditTypeMessageBoxMessage,
               Resources.CreditContractViewModel_ChangeCreditTypeMessageBoxCaption,
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

         return answer == MessageBoxResult.Yes;
      }

      private void removeOldDocuments()
      {
         foreach (var requiredDocument in _creditInfo.RequiredDocuments.ToArray())
            _creditInfo.RemoveRequiredDocument(requiredDocument);
      }

      private void addNewDocuments()
      {
         var requiredCreditDocuments =
            _requiredCreditDocuments
               .Single(item => item.CreditType.Id == _creditInfo.CreditType.Id);

         foreach (var documentType in requiredCreditDocuments.DocumentTypes)
            _creditInfo.AddRequiredDocument(documentType);
      }

      private void buildPaymentsSchedule()
      {
         _creditInfo.BuildPaymentsSchedule();
         PaymentsSchedule = getPaymentsSchedule();
         propertyChanged("PaymentsSchedule");
      }

      private bool canBuildPaymentsSchedule()
      {
         return _creditInfo.CanBuildPaymentsSchedule();
      }

      private void buildDetailedPayments()
      {
         _creditInfo.BuildPaymentsProgress();
         PaymentsProgress = new ObservableCollection<PaymentAdvanceViewModel>(getPaymentsProgress());
         propertyChanged("PaymentsProgress");
      }

      private void addGuarantor()
      {
         var personInfo = _creditInfo.AddGuarantor();
         Guarantors.Add(new PersonInfoViewModel(personInfo));

         if (Guarantors.Count == 1)
            propertyChanged("Guarantors");
      }

      private void removeGuarantor()
      {
         var personInfo = SelectedGuarantor.Original;
         _creditInfo.RemoveGuarantor(personInfo);
         Guarantors.Remove(SelectedGuarantor);

         if (Guarantors.Count == 0)
            propertyChanged("Guarantors");
      }

      private bool canRemoveGuarantor()
      {
         return SelectedGuarantor != null;
      }

      private void addTodoItem()
      {
         var todoItem = _creditInfo.AddTodoItem();
         TodoList.Add(new TodoItemViewModel(todoItem, Borrower.PhoneNumbers, _buzzerDatabase));
      }

      private void removeTodoItem()
      {
         var todoItem = SelectedTodoItem.Original;
         _creditInfo.RemoveTodoItem(todoItem);
         TodoList.Remove(SelectedTodoItem);
      }

      private bool canRemoveTodoItem()
      {
         return SelectedTodoItem != null;
      }

      private void addPayoff()
      {
         var newPayoff = PayoffInfo.CreateNew(_creditInfo.Id);
         newPayoff.PayoffDate = DateTime.Now;

         var newPayoffViewModel = new PayoffViewModel(newPayoff, IsUsdRateEnabled);

         var payoffView = new PayoffWindow
         {
            ShowActivated = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            DataContext = newPayoffViewModel
         };

         var result = payoffView.ShowDialog();
         if (result.HasValue && result.Value)
         {
            _creditInfo.InsertPayoff(newPayoff);
            Payoffs.Add(newPayoffViewModel);

            buildDetailedPayments();
         }
      }

      private void updatePayoff()
      {
         var payoffCopy = PayoffInfo.CopyOf(SelectedPayoff.Original);
         var payoffViewModel = new PayoffViewModel(payoffCopy, IsUsdRateEnabled);

         var payoffView = new PayoffWindow
         {
            ShowActivated = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            DataContext = payoffViewModel
         };

         var result = payoffView.ShowDialog();
         if (result.HasValue && result.Value)
         {
            SelectedPayoff.CopyDataFrom(payoffViewModel);
            buildDetailedPayments();
            propertyChanged("SelectedPayoff");
         }
      }

      private bool canUpdatePayoff()
      {
         return SelectedPayoff != null;
      }

      private void removePayoff()
      {
         var payoff = SelectedPayoff.Original;
         _creditInfo.RemovePayoff(payoff);
         Payoffs.Remove(SelectedPayoff);

         buildDetailedPayments();
      }

      private bool canRemovePayoff()
      {
         return SelectedPayoff != null;
      }

      private void save()
      {
         _creditInfo.OnSave();
         propertyChanged("CreditState");

         saveChangesToDatabase();
      }

      private bool canSave()
      {
         return true;
      }

      private void refuse()
      {
         var answer =
            MessageBox.Show(Resources.CreditContractViewModel_RefuseCreditMessageBoxMessage,
               Resources.CreditContractViewModel_RefuseCreditMessageBoxCaption,
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

         if (answer == MessageBoxResult.Yes)
         {
            _creditInfo.Refuse();
            propertyChanged("CreditState");

            saveChangesToDatabase();
         }
      }

      private bool canRefuse()
      {
         return _creditInfo.CreditState != CreditState.Repayed &&
                _creditInfo.CreditState != CreditState.Refused;
      }

      private void saveChangesToDatabase()
      {
         try
         {
            _buzzerDatabase.SaveCredit(_creditInfo);
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

      private void sendSms()
      {
         try
         {
            const string message = "MKK Standart Kredit. Prosim Vas prinesti " +
                                   "dokumenty po predostavlennomu kreditu.";

            var smsSender = SmsSenderFactory.GetSmsSender(SelectedPhoneNumber.PhoneNumber);
            smsSender.Send(message);

            _creditInfo.Notified();
            _buzzerDatabase.SaveCreditNotificationInfo(_creditInfo);

            propertyChanged("NotificationCount");
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
                SelectedPhoneNumber["SelectedPhoneNumber"] == null &&
                _creditInfo.Id != NullValues.Id;
      }
   }
}
