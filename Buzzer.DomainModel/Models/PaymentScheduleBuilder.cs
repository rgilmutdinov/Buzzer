using System;

namespace Buzzer.DomainModel.Models
{
   public static class PaymentScheduleBuilder
   {
      public static PaymentInfo[] Build(
         CreditPayment[] payments, DateTime start,
         int monthsCount, bool isUsd)
      {
         var result = new PaymentInfo[monthsCount];

         for (var i = 0; i < monthsCount; i++)
         {
            decimal paymentAmount =
               isUsd
                  ? payments[i].CurrencyPaymentAmount.Value
                  : payments[i].PaymentAmount;
            result[i] = PaymentInfo.CreateNew(paymentAmount, start.AddMonths(i + 1));
         }

         return result;
      }
   }
}