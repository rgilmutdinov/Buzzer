namespace Notifier.Parser
{
   public sealed class ContractData
   {
      public ContractData(string contractNumber, string borrowerName, decimal exchangeRate, Payment[] payments)
      {
         ContractNumber = contractNumber;
         BorrowerName = borrowerName;
         ExchangeRate = exchangeRate;
         Payments = payments;
      }

      public string ContractNumber { get; private set; }
      public string BorrowerName { get; private set; }
      public decimal ExchangeRate { get; private set; }
      public Payment[] Payments { get; private set; }
   }
}