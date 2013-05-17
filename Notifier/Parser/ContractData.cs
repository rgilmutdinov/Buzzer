using Notifier.Common;

namespace Notifier.Parser
{
   public sealed class ContractData
   {
      public ContractData(string contractNumber, string borrowerName, decimal exchangeRate, PaymentData[] paymentsData)
      {
         Check.NotNull(contractNumber, "contractNumber");
         Check.NotNull(borrowerName, "borrowerName");
         Check.NotNull(exchangeRate, "exchangeRate");
         Check.NotNull(paymentsData, "payments");

         ContractNumber = contractNumber;
         BorrowerName = borrowerName;
         ExchangeRate = exchangeRate;
         PaymentsData = paymentsData;
      }

      public string ContractNumber { get; private set; }
      public string BorrowerName { get; private set; }
      public decimal ExchangeRate { get; private set; }
      public PaymentData[] PaymentsData { get; private set; }
   }
}