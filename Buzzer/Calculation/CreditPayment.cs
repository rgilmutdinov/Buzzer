namespace Buzzer.Calculation
{
   public sealed class CreditPayment
   {
      public CreditPayment(
         decimal creditSum,
         decimal? currencyCreditSum,
         decimal percentSum,
         decimal baseSum,
         decimal totalSum,
         decimal tax,
         decimal paymentAmount,
         decimal? currencyPaymentAmount
         )
      {
         CreditSum = creditSum;
         CurrencyCreditSum = currencyCreditSum;
         PercentSum = percentSum;
         BaseSum = baseSum;
         TotalSum = totalSum;
         Tax = tax;
         PaymentAmount = paymentAmount;
         CurrencyPaymentAmount = currencyPaymentAmount;
      }

      // Сумма выданного займа.
      public decimal CreditSum { get; private set; }

      // Сумма выданного займа в долларах США.
      public decimal? CurrencyCreditSum { get; private set; }

      // Проценты к оплате.
      public decimal PercentSum { get; private set; }

      // Основная сумма к оплате.
      public decimal BaseSum { get; private set; }

      // Итого к оплате.
      public decimal TotalSum { get; private set; }

      // Налог.
      public decimal Tax { get; private set; }

      // Всего к оплате.
      public decimal PaymentAmount { get; private set; }

      // Всего к оплате в долларах США.
      public decimal? CurrencyPaymentAmount { get; private set; }
   }
}