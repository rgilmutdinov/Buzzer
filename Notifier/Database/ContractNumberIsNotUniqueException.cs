using System;
using Notifier.Common;

namespace Notifier.Database
{
   public sealed class ContractNumberIsNotUniqueException : Exception
   {
      public ContractNumberIsNotUniqueException(string contractNumber)
      {
         Check.NotNull(contractNumber, "contractNumber");
         ContractNumber = contractNumber;
      }

      public string ContractNumber { get; private set; }
   }
}