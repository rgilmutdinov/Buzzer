using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Common;

namespace Buzzer.DomainModel.Models
{
   public static class AmortizationBuilder
   {
      public static PaymentAdvance[] Build(
         decimal creditSum,
         DateTime creditIssueDate,
         int months,
         decimal discountRate,
         decimal? currencyRate,
         List<PayoffInfo> payoffs)
      {
         Check.NotNull(payoffs, "payoffs");

         DateTime currentDate = DateTime.Now;

         var payments = CreditCalculator.Annuity(creditSum, months, discountRate, currencyRate);

         var paymentsProgress = new PaymentAdvance[months];
         var payoffIndex = 0;
         decimal balance = 0M;
         for (var i = 0; i < months; i++)
         {
            CreditPayment payment = payments[i];

            DateTime startDate = creditIssueDate.AddMonths(i);
            DateTime dueDate = startDate.AddMonths(1);

            PaymentAdvance paymentAdvance = new PaymentAdvance(payments[i], dueDate);

            if (dueDate <= currentDate)
            {
               balance -= payment.TotalSum;
               for (int j = payoffIndex; j < payoffs.Count; j++)
               {
                  PayoffInfo currentPayoff = payoffs[j];
                  DateTime payoffDate = currentPayoff.PayoffDate;

                  if (payoffDate > startDate && payoffDate <= dueDate)
                  {
                     balance += currentPayoff.PayoffAmount;
                  }
                  else
                  {
                     payoffIndex = j;
                     break;
                  }
               }

               decimal penalty = getPenalty(startDate, currentDate, -balance, discountRate);
               balance -= penalty;

               paymentAdvance.Balance = balance;
               paymentAdvance.Penalty = penalty;

               paymentAdvance.State = balance >= 0 
                  ? PaymentAdvanceState.PastPaid 
                  : PaymentAdvanceState.PastUnpaid;
            }
            else if (dueDate > currentDate && currentDate >= startDate)
            {
               paymentAdvance.State = PaymentAdvanceState.Current;
            }
            else
            {
               paymentAdvance.State = PaymentAdvanceState.Oncoming;
            }
            
            paymentsProgress[i] = paymentAdvance;
         }

         return paymentsProgress;
      }

      private static decimal getPenalty(DateTime startDate, DateTime endDate, decimal underpaid, decimal discountRate)
      {
         if (underpaid <= 0)
            return 0M;

         decimal dayPenalty = decimal.Round(underpaid * discountRate /  360.0M);
         return endDate.Subtract(startDate).Days * dayPenalty;
      }
   }
}
