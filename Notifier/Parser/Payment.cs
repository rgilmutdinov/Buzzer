using System;

namespace Notifier.Parser
{
   public sealed class Payment
   {
      public Payment(decimal amount, DateTime date)
      {
         Amount = amount;
         Date = date;
      }

      public decimal Amount { get; set; }
      public DateTime Date { get; set; }
   }
}