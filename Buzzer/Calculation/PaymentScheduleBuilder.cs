using System;
using Buzzer.DomainModel.Models;

namespace Buzzer.Calculation
{
   public static class PaymentScheduleBuilder
   {
      public static PaymentInfo[] Build(CreditPayment[] payments, DateTime start, int monthsCount)
      {
         var result = new PaymentInfo[monthsCount];

         for (var i = 0; i < monthsCount; i++)
         {
            result[i] =
               PaymentInfo.Create(
               payments[i].PaymentAmount,
               payments[i].CurrencyPaymentAmount,
               start.AddMonths(i + 1)
               );
         }

         return result;
      }
   }
}