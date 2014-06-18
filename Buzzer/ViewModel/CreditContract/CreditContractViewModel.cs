using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Properties;
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

      private ICommand _buildPaymentsScheduleCommand;
      private ICommand _addGuarantorCommand;
      private ICommand _removeGuarantorCommand;
      private ICommand _saveCommand;

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

         DisplayName = getDisplayName();
      }

      #region Fields

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

            _creditInfo.CreditIssueDate = value;
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
         get { return _creditInfo.DiscountRate * 100; }
         set
         {
            if (_creditInfo.DiscountRate == value)
               return;

            _creditInfo.DiscountRate = value / 100;
            propertyChanged("DiscountRate");
         }
      }

      public decimal? EffectiveDiscountRate
      {
         get { return _creditInfo.EffectiveDiscountRate * 100; }
         set
         {
            if (_creditInfo.EffectiveDiscountRate == value)
               return;

            _creditInfo.EffectiveDiscountRate = value / 100;
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

      public PersonInfoViewModel SelectedGuarantor { get; set; }

      public ObservableCollection<PersonInfoViewModel> Guarantors { get; private set; }

      public PaymentInfoViewModel[] PaymentsSchedule { get; private set; }

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

      private string getDisplayName()
      {
         return _creditInfo.IsNew ? Resources.NewCreditTabCaption : _creditInfo.CreditNumber;
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

      private void save()
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

      private bool canSave()
      {
         return true;
      }
   }
}
