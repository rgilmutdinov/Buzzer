using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class PayoffInfo : DomainObject
   {
      private PayoffInfo(
         int creditId,
         decimal payoffAmount,
         DateTime payoffDate)
      {
         CreditId = creditId;
         PayoffAmount = payoffAmount;
         PayoffDate = payoffDate;
      }

      public static PayoffInfo CreateNew(
         int creditId,
         decimal payoffAmount,
         DateTime payoffDate)
      {
         return new PayoffInfo(creditId, payoffAmount, payoffDate);
      }

      public static PayoffInfo CopyOf(PayoffInfo payoffInfo)
      {
         var copy = CreateNew(
            payoffInfo.CreditId, payoffInfo.PayoffAmount, payoffInfo.PayoffDate);
         copy.Id = payoffInfo.Id;
         copy.Remarks = payoffInfo.Remarks;

         return copy;
      }

      public static PayoffInfo CreateNew(int creditId)
      {
         return new PayoffInfo(creditId, 0, NullValues.DateTime);
      }

      public static PayoffInfo Create(
         int id,
         int creditId,
         decimal payoffAmount,
         DateTime payoffDate,
         string remarks)
      {
         return new PayoffInfo(creditId, payoffAmount, payoffDate)
         {
            Id = id,
            Remarks = remarks
         };
      }

      public int CreditId { get; set; }

      // Оплаченная сумма.
      public decimal PayoffAmount { get; set; }

      // Дата платежа.
      public DateTime PayoffDate { get; set; }

      // Примечание
      public string Remarks { get; set; }

      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "PayoffAmount":
               return validatePayoffAmount();

            case "PayoffDate":
               return validatePayoffDate();

            case "Remarks":
               return validateRemarks();

            default:
               throw new ArgumentException(columnName, "columnName");
         }
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[]
         {
            "PayoffAmount",
            "PayoffDate",
            "Remarks"
         };
      }

      private string validatePayoffAmount()
      {
         return PayoffAmount <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validatePayoffDate()
      {
         return PayoffDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      private string validateRemarks()
      {
         const int maxLength = 2000;
         if (Remarks.SafeGetLength() > maxLength)
            return string.Format(Resources.MaxLengthExceeded, maxLength);

         return null;
      }
   }
}
