using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class PaymentInfo : RepositoryItem
   {
      private PaymentInfo()
      {
      }

      public static PaymentInfo CreateNew(
         decimal paymentAmount,
         DateTime paymentDate
         )
      {
         return new PaymentInfo
                   {
                      PaymentAmount = paymentAmount,
                      PaymentDate = paymentDate,
                      IsNotified = false
                   };
      }

      public static PaymentInfo Create(
         int id,
         decimal paymentAmount,
         DateTime paymentDate,
         bool isNotified
         )
      {
         return new PaymentInfo
                   {
                      Id = id,
                      PaymentAmount = paymentAmount,
                      PaymentDate = paymentDate,
                      IsNotified = isNotified
                   };
      }

      // ����� � ������.
      public decimal PaymentAmount { get; set; }
      
      // ���� �������.
      public DateTime PaymentDate { get; set; }

      // ����������, ���� �� ������� ���������� �� ������� �������.
      public bool IsNotified { get; set; }

      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "PaymentAmount":
               return validatePaymentAmount();

            case "PaymentDate":
               return validatePaymentDate();

            default:
               throw new ArgumentException(columnName, "columnName");
         }
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[]
                   {
                      "PaymentAmount",
                      "PaymentDate"
                   };
      }

      private string validatePaymentAmount()
      {
         return PaymentAmount <= decimal.Zero ? Resources.IncorrectValue : null;
      }

      private string validatePaymentDate()
      {
         return PaymentDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }
   }
}