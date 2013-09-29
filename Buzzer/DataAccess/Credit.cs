using System.ComponentModel;
using System.Linq;
using Buzzer.Common;
using Buzzer.Properties;

namespace Buzzer.DataAccess
{
   public partial class Credit : IDataErrorInfo
   {
      public Person Borrower
      {
         get { return PersonsToCredits.Single(item => item.IsBorrower).Person; }
      }

      public Person[] Guarantors
      {
         get
         {
            return
               PersonsToCredits
                  .Where(item => !item.IsBorrower)
                  .Select(item => item.Person)
                  .ToArray();
         }
      }

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

               case "UsdRate":
                  return validateUsdRate();

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

      private string validateCreditNumber()
      {
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

      private string validateUsdRate()
      {
         if (!ExchangeRate.HasValue)
            return null;

         return ExchangeRate.Value <= decimal.Zero ? Resources.IncorrectValue : null;
      }
   }
}