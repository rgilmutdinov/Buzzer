namespace Notifier.Database
{
   public sealed class Contract
   {
      public int Id { get; set; }
      public string ContractNumber { get; set; }
      public string BorrowerName { get; set; }
      public decimal ExchangeRate { get; set; }
      public string PhoneNumber { get; set; }
      public Payment[] Payments { get; set; }
   }
}