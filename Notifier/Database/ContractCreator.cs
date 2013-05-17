using Notifier.Common;

namespace Notifier.Database
{
   public sealed class ContractCreator
   {
      private readonly Repository _repository;

      public ContractCreator(Repository repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;
      }

      public Contract Create(string contractNumber, string borrowerName, decimal exchangeRate, string phoneNumber, Payment[] payments)
      {
         return new Contract(_repository, contractNumber, borrowerName, exchangeRate, phoneNumber, payments);
      }
   }
}