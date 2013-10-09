using System;

namespace DataAccess.Model
{
   public sealed class PaymentInfo
   {
      private PaymentInfo()
      {
      }

      public static PaymentInfo Create(
         decimal paymentAmount,
         decimal? currencyPaymentAmount,
         DateTime paymentDate
         )
      {
         return new PaymentInfo
                   {
                      PaymentAmount = paymentAmount,
                      CurrencyPaymentAmount = currencyPaymentAmount,
                      PaymentDate = paymentDate
                   };
      }

      // ����� � ������.
      public decimal PaymentAmount { get; set; }

      // ����� � ������ � �������� ���.
      public decimal? CurrencyPaymentAmount { get; set; }

      // ���� �������.
      public DateTime PaymentDate { get; set; }
   }
}