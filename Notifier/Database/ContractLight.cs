using System.Collections.Generic;

namespace Notifier.Database
{
   public sealed class ContractLight
   {
      private readonly List<PaymentLight> _payments = new List<PaymentLight>(); 

      public int Id { get; set; }
      public string ContractNumber { get; set; }
      public string BorrowerName { get; set; }
      public decimal ExchangeRate { get; set; }
      public string PhoneNumber { get; set; }

      public IEnumerable<PaymentLight> Payments
      {
         get { return _payments; }
      }

      public void AddPayment(PaymentLight payment)
      {
         _payments.Add(payment);
      }
   }
}