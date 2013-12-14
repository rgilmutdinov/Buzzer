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

      // ����� ��������� �����.
      public decimal CreditSum { get; private set; }

      // ����� ��������� ����� � �������� ���.
      public decimal? CurrencyCreditSum { get; private set; }

      // �������� � ������.
      public decimal PercentSum { get; private set; }

      // �������� ����� � ������.
      public decimal BaseSum { get; private set; }

      // ����� � ������.
      public decimal TotalSum { get; private set; }

      // �����.
      public decimal Tax { get; private set; }

      // ����� � ������.
      public decimal PaymentAmount { get; private set; }

      // ����� � ������ � �������� ���.
      public decimal? CurrencyPaymentAmount { get; private set; }
   }
}