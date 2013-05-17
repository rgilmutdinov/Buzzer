using System;

namespace Notifier.Database
{
   public sealed class Payment
   {
      public Payment(int id, int contractId, DateTime paymentDate, decimal paymentAmount, bool isNotified)
      {
         Id = id;
         ContractId = contractId;
         PaymentDate = paymentDate;
         PaymentAmount = paymentAmount;
         IsNotified = isNotified;
      }

      public Payment(DateTime paymentDate, decimal paymentAmount, bool isNotified)
         : this(0, 0, paymentDate, paymentAmount, isNotified)
      {
      }

      public int Id { get; private set; }
      public int ContractId { get; private set; }
      public DateTime PaymentDate { get; private set; }
      public decimal PaymentAmount { get; private set; }
      public bool IsNotified { get; private set; }

      public void SetId(int id)
      {
         Id = id;
      }

      public void SetContractId(int id)
      {
         ContractId = id;
      }
   }
}