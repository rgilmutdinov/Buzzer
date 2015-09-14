using System;
using Common;

namespace Buzzer.DomainModel.Models
{
   public enum PaymentAdvanceState
   {
      PastPaid,
      PastUnpaid,
      Current,
      Oncoming
   }


   public sealed class PaymentAdvance
   {
      public PaymentAdvance(CreditPayment creditPayment, DateTime dueDate)
      {
         Check.NotNull(creditPayment, "creditPayment");

         Payment = creditPayment;
         DueDate = dueDate;
      }

      public CreditPayment Payment { get; private set; }
      public DateTime DueDate { get; private set; }

      public decimal? Balance { get; set; }
      public decimal? Penalty { get; set; }

      public PaymentAdvanceState State { get; set; }
   }
}
