namespace Buzzer.Calculation
{
   public static class CreditCalculator
   {
      private const decimal TaxRate = 0.02M;

      internal static CreditPayment[] Annuity(
         decimal creditSum,
         int months,
         decimal discountRate,
         decimal? currencyRate)
      {
         decimal monthlyRate = discountRate / 12;
         decimal power = pow(1 + monthlyRate, months);
         decimal factor = monthlyRate * power / (power - 1);
         decimal monthlySum = creditSum * factor;
         decimal rest = creditSum;
         CreditPayment[] result = new CreditPayment[months];

         for (var i = 0; i < months; i++)
         {
            decimal totalSum = monthlySum;
            decimal percent = rest * monthlyRate;
            decimal baseSum = totalSum - percent;
            decimal tax = percent * TaxRate;
            decimal paymentAmount = totalSum + tax;

            result[i] = new CreditPayment(round(rest),
                                          getCurrencyValue(rest, currencyRate),
                                          round(percent),
                                          round(baseSum),
                                          round(totalSum),
                                          round(tax),
                                          round(paymentAmount),
                                          getCurrencyValue(paymentAmount, currencyRate));
            rest -= baseSum;
         }

         return result;
      }

      private static decimal? getCurrencyValue(decimal value, decimal? currencyRate)
      {
         return currencyRate.HasValue ? round(value / currencyRate.Value) : (decimal?) null;
      }

      private static decimal pow(decimal value, int power)
      {
         decimal result = value;

         for (var i = 0; i < power - 1; i++)
            result *= value;

         return result;
      }

      private static decimal round(decimal value)
      {
         return decimal.Round(value);
      }
   }
}
