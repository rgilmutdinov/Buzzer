using System;
using System.Diagnostics;

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
         decimal monthlySum = round(creditSum * factor);
         decimal rest = creditSum;
         CreditPayment[] result = new CreditPayment[months];

         for (var i = 0; i < months; i++)
         {
            decimal totalSum = monthlySum;
            decimal percent = round(rest * monthlyRate);
            decimal baseSum = round(totalSum - percent);
            decimal tax = round(percent * TaxRate);
            decimal paymentAmount = round(totalSum + tax);

            result[i] = new CreditPayment
                           {
                              CreditSum = rest,
                              CurrencyCreditSum = getCurrencyValue(rest, currencyRate),
                              PercentSum = percent,
                              BaseSum = baseSum,
                              TotalSum = totalSum,
                              Tax = tax,
                              PaymentAmount = paymentAmount,
                              CurrencyPaymentAmount = getCurrencyValue(paymentAmount, currencyRate)
                           };

            rest = round(rest - baseSum);

            Debug.Assert(totalSum == percent + baseSum);
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
         return Math.Round(value);
      }
   }
}
