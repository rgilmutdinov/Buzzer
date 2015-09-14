using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class CreditInfo : DomainObject
   {
      private decimal _creditAmount;
      private decimal _discountRate;
      private decimal? _effectiveDiscountRate;
      private decimal? _exchangeRate;
      private List<PersonInfo> _guarantors;
      private List<TodoItem> _todoList;
      private List<RequiredDocument> _requiredDocuments;
      private List<PayoffInfo> _payoffs;

      private CreditInfo()
      {
      }

      public static CreditInfo CreatNew()
      {
         var newCredit = new CreditInfo
         {
            Id = NullValues.Id,
            ApplicationDate = DateTime.Today,
            ProtocolDate = null,
            CreditIssueDate = NullValues.DateTime,
            CreditState = CreditState.Consideration,
            Borrower = PersonInfo.CreateNew(NullValues.Id),
            _guarantors = new List<PersonInfo>(),
            PaymentsSchedule = new PaymentInfo[0],
            RowState = RowState.Modified,
            CreditType = null,
            NotificationDescription = string.Empty,
            NotificationDate = null,
            NotificationCount = 0,
            _todoList = new List<TodoItem>(),
            _requiredDocuments = new List<RequiredDocument>(),
            _payoffs = new List<PayoffInfo>()
         };
         newCredit.Borrower.IsBorrower = true;
         return newCredit;
      }

      public static CreditInfo Create(
         int id,
         string creditNumber,
         DateTime? applicationDate,
         DateTime? protocolDate,
         decimal creditAmount,
         DateTime creditIssueDate,
         int monthsCount,
         decimal discountRate,
         decimal? effectiveDiscountRate,
         decimal? exchangeRate,
         CreditState creditState,
         string refusalReason,
         RowState rowState,
         PersonInfo borrower,
         CreditType creditType,
         string notificationDescription,
         int notificationCount,
         DateTime? notificationDate,
         IEnumerable<PersonInfo> guarantors,
         IEnumerable<PaymentInfo> payments,
         IEnumerable<TodoItem> todoList,
         IEnumerable<RequiredDocument> requiredDocuments,
         IEnumerable<PayoffInfo> payoffs
         )
      {
         var paymentsSchedule = payments.OrderBy(item => item.PaymentDate).ToArray();

         return new CreditInfo
         {
            Id = id,
            CreditNumber = creditNumber,
            ApplicationDate = applicationDate,
            ProtocolDate = protocolDate,
            CreditAmount = creditAmount,
            CreditIssueDate = creditIssueDate,
            MonthsCount = monthsCount,
            DiscountRate = discountRate,
            EffectiveDiscountRate = effectiveDiscountRate,
            ExchangeRate = exchangeRate,
            CreditState = creditState,
            RefusalReason = refusalReason,
            RowState = rowState,
            Borrower = borrower,
            CreditType = creditType,
            NotificationDescription = notificationDescription,
            NotificationCount = notificationCount,
            NotificationDate = notificationDate,
            _guarantors = guarantors.ToList(),
            PaymentsSchedule = paymentsSchedule,
            _todoList = todoList.ToList(),
            _requiredDocuments = requiredDocuments.ToList(),
            _payoffs = payoffs.ToList()
         };
      }

      // Номер кредитного договора.
      public string CreditNumber { get; set; }

      // Дата подачи заявления.
      public DateTime? ApplicationDate { get; set; }

      // Дата протокола.
      public DateTime? ProtocolDate { get; set; }

      // Сумма кредита.
      public decimal CreditAmount
      {
         get { return _creditAmount; }
         set { _creditAmount = decimal.Round(value); }
      }

      // Дата выдачи кредита.
      public DateTime CreditIssueDate { get; set; }

      // Число месяцев.
      public int MonthsCount { get; set; }

      // Процентная ставка.
      public decimal DiscountRate
      {
         get { return _discountRate; }
         set { _discountRate = decimal.Round(value, 4); }
      }

      // Эффективная процентная ставка.
      public decimal? EffectiveDiscountRate
      {
         get { return _effectiveDiscountRate; }
         set { _effectiveDiscountRate = value.HasValue ? decimal.Round(value.Value, 4) : (decimal?) null; }
      }

      // Курс доллара США.
      public decimal? ExchangeRate
      {
         get { return _exchangeRate; }
         set { _exchangeRate = value.HasValue ? decimal.Round(value.Value, 4) : (decimal?) null; }
      }

      // Статус кредита.
      public CreditState CreditState { get; set; }

      // Причина отказа.
      public string RefusalReason { get; set; }

      // Статус.
      public RowState RowState { get; private set; }

      // Заемщик.
      public PersonInfo Borrower { get; private set; }

      // Поручители.
      public ReadOnlyCollection<PersonInfo> Guarantors
      {
         get { return _guarantors.AsReadOnly(); }
      }

      // График погашений платежей.
      public PaymentInfo[] PaymentsSchedule { get; private set; }

      public PaymentAdvance[] PaymentsProgress { get; private set; }

      // Список заданий/действий, которые нужно выполнить.
      public ReadOnlyCollection<TodoItem> TodoList
      {
         get { return _todoList.AsReadOnly(); }
      }

      // Список платежей по кредиту
      public ReadOnlyCollection<PayoffInfo> Payoffs
      {
         get { return _payoffs.AsReadOnly(); }
      }

      // Тип кредита.
      public CreditType CreditType { get; set; }

      // Примечание оповещения о требуемых документах.
      public string NotificationDescription { get; set; }

      // Количество оповещений.
      public int NotificationCount { get; set; }

      // Дата последнего оповещения.
      public DateTime? NotificationDate { get; set; }

      // Список требуемых документов.
      public ReadOnlyCollection<RequiredDocument> RequiredDocuments
      {
         get { return _requiredDocuments.AsReadOnly(); }
      }

      public PersonInfo AddGuarantor()
      {
         var newGuarantor = PersonInfo.CreateNew(Id);
         newGuarantor.IsBorrower = false;
         _guarantors.Add(newGuarantor);
         return newGuarantor;
      }

      public void RemoveGuarantor(PersonInfo guarantor)
      {
         _guarantors.Remove(guarantor);
      }

      public TodoItem AddTodoItem()
      {
         var todoItem = TodoItem.CreateNew(Id);
         _todoList.Add(todoItem);
         return todoItem;
      }

      public void RemoveTodoItem(TodoItem todoItem)
      {
         _todoList.Remove(todoItem);
      }

      public void InsertPayoff(PayoffInfo payoff)
      {
         _payoffs.Add(payoff);
      }

      public void RemovePayoff(PayoffInfo payoff)
      {
         _payoffs.Remove(payoff);
      }

      public RequiredDocument AddRequiredDocument(DocumentType documentType)
      {
         var requiredDocument = RequiredDocument.CreateNew(Id, documentType);
         _requiredDocuments.Add(requiredDocument);
         return requiredDocument;
      }

      public void RemoveRequiredDocument(RequiredDocument requiredDocument)
      {
         _requiredDocuments.Remove(requiredDocument);
      }

      public bool CanBuildPaymentsSchedule()
      {
         var info = this as IDataErrorInfo;
         return
            info["CreditAmount"] == null &&
            info["MonthsCount"] == null &&
            info["DiscountRate"] == null &&
            info["ExchangeRate"] == null &&
            info["CreditIssueDate"] == null;
      }

      public void BuildPaymentsSchedule()
      {
         var payments = CreditCalculator.Annuity(CreditAmount, MonthsCount, DiscountRate, ExchangeRate);
         PaymentsSchedule = PaymentScheduleBuilder.Build(payments, CreditIssueDate, MonthsCount,
            ExchangeRate.HasValue);
      }

      public void BuildPaymentsProgress()
      {
         if (!CanBuildPaymentsSchedule())
         {
            PaymentsProgress = new PaymentAdvance[0];
            return;
         }

         List<PayoffInfo> timelinePayoffs = Payoffs.OrderBy(payoff => payoff.PayoffDate).ToList();
         PaymentsProgress = AmortizationBuilder.Build(
            CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, ExchangeRate, timelinePayoffs);
      }

      public override bool IsValid()
      {
         var isValid = base.IsValid() && Borrower.IsValid();

         foreach (var guarantor in _guarantors)
            isValid &= guarantor.IsValid();

         return isValid;
      }

      public void OnSave()
      {
         if (CreditState != CreditState.Repayed && CreditState != CreditState.Refused)
         {
            CreditState = string.IsNullOrEmpty(CreditNumber)
               ? CreditState.Consideration
               : CreditState.Current;
         }
      }

      public void Refuse()
      {
         if (CreditState != CreditState.Repayed && CreditState != CreditState.Refused)
         {
            CreditState = CreditState.Refused;
         }
      }

      public void Delete()
      {
         RowState = RowState.Deleted;
      }

      public void Notified()
      {
         if (NotificationDate.HasValue && NotificationDate.Value == DateTime.Today)
         {
            NotificationCount++;
         }
         else
         {
            NotificationDate = DateTime.Today;
            NotificationCount = 1;
         }
      }

      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "CreditNumber":
               return validateCreditNumber();

            case "ApplicationDate":
               return validateApplicationDate();

            case "ProtocolDate":
               return validateProtocolDate();

            case "CreditAmount":
               return validateCreditAmount();

            case "CreditIssueDate":
               return validateCreditIssueDate();

            case "MonthsCount":
               return validateMonthsCount();

            case "DiscountRate":
               return validateDiscountRate();

            case "EffectiveDiscountRate":
               return validateEffectiveDiscountRate();

            case "ExchangeRate":
               return validateExchangeRate();

            case "Guarantors":
               return validateGuarantors();
         }

         throw new ArgumentException(columnName, "columnName");
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[]
         {
            "CreditNumber",
            "CreditAmount",
            "CreditIssueDate",
            "MonthsCount",
            "DiscountRate"
         };
      }

      #region Validation methods

      private string validateCreditNumber()
      {
         if (CreditNumber.SafeGetLength() > 100)
            return string.Format(Resources.MaxLengthExceeded, 100);

         return string.IsNullOrEmpty(CreditNumber) ? Resources.FieldMustBeFilled : null;
      }

      private string validateApplicationDate()
      {
         return ApplicationDate.HasValue ? null : Resources.FieldMustBeFilled;
      }

      private string validateProtocolDate()
      {
         return null;
      }

      private string validateCreditAmount()
      {
         return CreditAmount <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validateCreditIssueDate()
      {
         return CreditIssueDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      private string validateMonthsCount()
      {
         return MonthsCount <= 0 ? Resources.IncorrectValue : null;
      }

      private string validateDiscountRate()
      {
         return DiscountRate <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validateEffectiveDiscountRate()
      {
         if (!EffectiveDiscountRate.HasValue)
            return null;

         return EffectiveDiscountRate.Value <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validateExchangeRate()
      {
         if (!ExchangeRate.HasValue)
            return null;

         return ExchangeRate.Value <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validateCreditState()
      {
         return CreditState == CreditState.None ? Resources.IncorrectValue : null;
      }

      private string validateGuarantors()
      {
         return _guarantors.Count == 0 ? Resources.AtLeastOneGuarantorMustBeSpecified : null;
      }

      #endregion
   }
}
