using System;

namespace Notifier.Database
{
   public sealed class PaymentLight
   {
      public int Id { get; set; }
      public DateTime PaymentDate { get; set; }
      public decimal PaymentAmount { get; set; }
      public bool IsNotified { get; set; }
   }
}