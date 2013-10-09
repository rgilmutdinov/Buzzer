using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common;
using DataAccess.Properties;

namespace DataAccess.Model
{
   public sealed class CreditInfo : RepositoryItem, IDataErrorInfo
   {
      private CreditInfo()
      {
      }

      public static CreditInfo CreatNew()
      {
         var newCredit = new CreditInfo
                            {
                               Id = NullValues.Id,
                               CreditIssueDate = DateTime.Today,
                               Borrower = PersonInfo.CreateNew(),
                               Guarantors = new List<PersonInfo>()
                            };
         return newCredit;
      }

      // Номер кредитного договора.
      public string CreditNumber { get; set; }

      private string validateCreditNumber()
      {
         return string.IsNullOrEmpty(CreditNumber) ? Resources.FieldMustBeFilled : null;
      }

      // Сумма кредита.
      public decimal CreditAmount { get; set; }

      private string validateCreditAmount()
      {
         return CreditAmount <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      // Дата выдачи кредита.
      public DateTime CreditIssueDate { get; set; }

      private string validateCreditIssueDate()
      {
         return CreditIssueDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      // Число месяцев.
      public int MonthsCount { get; set; }

      private string validateMonthsCount()
      {
         return MonthsCount <= 0 ? Resources.IncorrectValue : null;
      }

      // Процентная ставка.
      public decimal DiscountRate { get; set; }

      private string validateDiscountRate()
      {
         return DiscountRate <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      // Эффективная процентная ставка.
      public decimal? EffectiveDiscountRate { get; set; }

      private string validateEffectiveDiscountRate()
      {
         if (!EffectiveDiscountRate.HasValue)
            return null;

         return EffectiveDiscountRate.Value <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      // Курс доллара США.
      public decimal? ExchangeRate { get; set; }

      private string validateExchangeRate()
      {
         if (!ExchangeRate.HasValue)
            return null;

         return ExchangeRate.Value <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      // Заемщик.
      public PersonInfo Borrower { get; private set; }

      // Поручители.
      public List<PersonInfo> Guarantors { get; private set; }

      private string validateGuarantors()
      {
         return Guarantors.Count == 0 ? Resources.AtLeastOneGuarantorMustBeSpecified : null;
      }

      // График погашений платежей.
      public PaymentInfo[] PaymentsSchedule { get; set; }

      // Проверяет, можно ли сохранить объект.
      public bool CanSave()
      {
         return validateCreditNumber() == null &&
                Borrower.CanSave() &&
                Guarantors.All(person => person.CanSave());
      }

      string IDataErrorInfo.this[string columnName]
      {
         get
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

            return null;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }
   }
}
