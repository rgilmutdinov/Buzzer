using System;

namespace Notifier.Database
{
   public sealed class Payment
   {
      public int Id { get; set; }
      public int ContractId { get; set; }
      public DateTime PaymentDate { get; set; }
      public decimal PaymentAmount { get; set; }
   }
}