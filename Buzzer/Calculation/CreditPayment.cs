namespace Buzzer.Calculation
{
   public sealed class CreditPayment
   {
      // Сумма выданного займа.
      public decimal CreditSum { get; set; }

      // Сумма выданного займа в долларах США.
      public decimal? CurrencyCreditSum { get; set; }

      // Проценты к оплате.
      public decimal PercentSum { get; set; }

      // Основная сумма к оплате.
      public decimal BaseSum { get; set; }

      // Итого к оплате.
      public decimal TotalSum { get; set; }

      // Налог.
      public decimal Tax { get; set; }

      // Всего к оплате.
      public decimal PaymentAmount { get; set; }

      // Всего к оплате в долларах США.
      public decimal? CurrencyPaymentAmount { get; set; }
   }
}