using System;

namespace Buzzer.Model
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

      // Всего к оплате.
      public decimal PaymentAmount { get; set; }

      // Всего к оплате в долларах США.
      public decimal? CurrencyPaymentAmount { get; set; }

      // Дата платежа.
      public DateTime PaymentDate { get; set; }
   }
}