namespace Buzzer.Calculation
{
   public sealed class CreditPayment
   {
      // ����� ��������� �����.
      public decimal CreditSum { get; set; }

      // ����� ��������� ����� � �������� ���.
      public decimal? CurrencyCreditSum { get; set; }

      // �������� � ������.
      public decimal PercentSum { get; set; }

      // �������� ����� � ������.
      public decimal BaseSum { get; set; }

      // ����� � ������.
      public decimal TotalSum { get; set; }

      // �����.
      public decimal Tax { get; set; }

      // ����� � ������.
      public decimal PaymentAmount { get; set; }

      // ����� � ������ � �������� ���.
      public decimal? CurrencyPaymentAmount { get; set; }
   }
}