using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Buzzer.Calculation;
using Buzzer.Common;
using Buzzer.DataAccess;
using Buzzer.Model;
using Buzzer.ViewModel.Common;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class CreditContractViewModel : WorkspaceViewModel, IDataErrorInfo
   {
      private readonly CreditRepository _creditRepository;
      private readonly CreditInfo _creditInfo;
      private readonly PersonInfo _borrower;
      private bool _isUsdRateEnabled;

      private ICommand _buildPaymentsScheduleCommand;
      private ICommand _addGuarantorCommand;
      private ICommand _removeGuarantorCommand;
      private ICommand _saveCommand;

      public CreditContractViewModel(CreditInfo creditInfo, CreditRepository creditRepository)
      {
         Check.NotNull(creditInfo, "creditInfo");
         Check.NotNull(creditRepository, "creditRepository");
         
         _creditInfo = creditInfo;
         _creditRepository = creditRepository;

         _borrower = _creditInfo.Borrower;
         Borrower = new PersonInfoViewModel(_borrower);
         _isUsdRateEnabled = _creditInfo.UsdRate.HasValue;

         Guarantors = 
            new ObservableCollection<PersonInfoViewModel>(
               _creditInfo.Guarantors.Select(item => new PersonInfoViewModel(item))
               );
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
         get { return _creditInfo.DiscountRate; }
         set
         {
            if (_creditInfo.DiscountRate == value)
               return;

            _creditInfo.DiscountRate = value;
            propertyChanged("DiscountRate");
         }
      }

      public decimal? EffectiveDiscountRate
      {
         get { return _creditInfo.EffectiveDiscountRate; }
         set
         {
            if (_creditInfo.EffectiveDiscountRate == value)
               return;

            _creditInfo.EffectiveDiscountRate = value;
            propertyChanged("EffectiveDiscountRate");
         }
      }

      public decimal UsdRate
      {
         get { return _creditInfo.UsdRate.HasValue ? _creditInfo.UsdRate.Value : decimal.Zero; }
         set
         {
            if (_creditInfo.UsdRate == value)
               return;

            _creditInfo.UsdRate = value;
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
               _creditInfo.UsdRate = null;

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

            _buildPaymentsScheduleCommand = new CommandDelegate(buildPaymentsSchedule, canbBuildPaymentsSchedule);
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
               case "CreditAmount":
               case "CreditIssueDate":
               case "MonthsCount":
               case "DiscountRate":
               case "EffectiveDiscountRate":
               case "UsdRate":
               case "Guarantors":
                  error = (_creditInfo as IDataErrorInfo)[columnName];
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
      
      private void buildPaymentsSchedule()
      {
         var payments = CreditCalculator.Annuity(CreditAmount, MonthsCount, DiscountRate, _creditInfo.UsdRate);
         _creditInfo.PaymentsSchedule = PaymentScheduleBuilder.Build(payments, CreditIssueDate, MonthsCount);

         PaymentsSchedule = new PaymentInfoViewModel[MonthsCount];

         for (var i = 0; i < MonthsCount; i++)
         {
            var number = i + 1;
            PaymentsSchedule[i] = new PaymentInfoViewModel(_creditInfo.PaymentsSchedule[i], number);
         }

         propertyChanged("PaymentsSchedule");
      }

      private bool canbBuildPaymentsSchedule()
      {
         var info = this as IDataErrorInfo;
         return
            info["CreditAmount"] == null &&
            info["MonthsCount"] == null &&
            info["DiscountRate"] == null &&
            info["UsdRate"] == null &&
            info["CreditIssueDate"] == null;
      }

      private void addGuarantor()
      {
         var personInfo = PersonInfo.CreateNew();
         _creditInfo.Guarantors.Add(personInfo);
         Guarantors.Add(new PersonInfoViewModel(personInfo));

         if (Guarantors.Count == 1)
            propertyChanged("Guarantors");
      }

      private void removeGuarantor()
      {
         var personInfo = SelectedGuarantor.Original;
         _creditInfo.Guarantors.Remove(personInfo);
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
         _creditRepository.InsertOrUpdate(_creditInfo);
      }

      private bool canSave()
      {
         return _creditInfo.CanSave();
      }
   }
}
