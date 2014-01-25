using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class CreditInfo : RepositoryItem
   {
      private decimal _creditAmount;
      private decimal _discountRate;
      private decimal? _effectiveDiscountRate;
      private decimal? _exchangeRate;
      private List<PersonInfo> _guarantors;

      private CreditInfo()
      {
      }

      public static CreditInfo CreatNew()
      {
         var newCredit = new CreditInfo
                            {
                               Id = NullValues.Id,
                               CreditIssueDate = DateTime.Today,
                               Borrower = PersonInfo.CreateNew(NullValues.Id),
                               _guarantors = new List<PersonInfo>()
                            };
         newCredit.Borrower.IsBorrower = true;
         return newCredit;
      }

      public static CreditInfo Create(
         int id,
         string creditNumber,
         decimal creditAmount,
         DateTime creditIssueDate,
         int monthsCount,
         decimal discountRate,
         decimal? effectiveDiscountRate,
         decimal? exchangeRate,
         PersonInfo borrower,
         IEnumerable<PersonInfo> guarantors
         )
      {
         return new CreditInfo
                   {
                      Id = id,
                      CreditNumber = creditNumber,
                      CreditAmount = creditAmount,
                      CreditIssueDate = creditIssueDate,
                      MonthsCount = monthsCount,
                      DiscountRate = discountRate,
                      EffectiveDiscountRate = effectiveDiscountRate,
                      ExchangeRate = exchangeRate,
                      Borrower = borrower,
                      _guarantors = guarantors.ToList()
                   };
      }

      // Номер кредитного договора.
      public string CreditNumber { get; set; }

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

      // Заемщик.
      public PersonInfo Borrower { get; private set; }

      // Поручители.
      public ReadOnlyCollection<PersonInfo> Guarantors
      {
         get { return _guarantors.AsReadOnly(); }
      }

      // График погашений платежей.
      public PaymentInfo[] PaymentsSchedule { get; set; }

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

      public override bool IsValid()
      {
         var isValid = base.IsValid() && Borrower.IsValid();

         foreach (var guarantor in _guarantors)
            isValid &= guarantor.IsValid();

         return isValid;
      }

      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "CreditNumber":
               return validateCreditNumber();

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

      private string validateCreditNumber()
      {
         if (CreditNumber.SafeGetLength() > 100)
            return string.Format(Resources.MaxLengthExceeded, 100);

         return string.IsNullOrEmpty(CreditNumber) ? Resources.FieldMustBeFilled : null;
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

      private string validateGuarantors()
      {
         return _guarantors.Count == 0 ? Resources.AtLeastOneGuarantorMustBeSpecified : null;
      }
   }
}
