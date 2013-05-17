using System;

namespace Notifier.Parser
{
   public sealed class PaymentData
   {
      public PaymentData(decimal amount, DateTime date, bool isNotified)
      {
         Amount = amount;
         Date = date;
         IsNotified = isNotified;
      }

      public decimal Amount { get; private set; }
      public DateTime Date { get; private set; }
      public bool IsNotified { get; private set; }
   }
}