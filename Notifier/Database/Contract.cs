using Notifier.Common;

namespace Notifier.Database
{
   public sealed class Contract
   {
      private readonly Repository _repository;
      private Payment[] _payments;

      public Contract(Repository repository, int id, string contractNumber, string borrowerName, decimal exchangeRate, string phoneNumber)
      {
         Check.NotNull(repository, "repository");
         Check.NotNull(contractNumber, "contractNumber");
         Check.NotNull(borrowerName, "borrowerName");

         _repository = repository;

         Id = id;
         ContractNumber = contractNumber;
         BorrowerName = borrowerName;
         ExchangeRate = exchangeRate;
         PhoneNumber = phoneNumber;
      }

      public Contract(Repository repository, string contractNumber, string borrowerName, decimal exchangeRate, string phoneNumber, Payment[] payments)
         : this(repository, 0, contractNumber, borrowerName, exchangeRate, phoneNumber)
      {
         Check.NotNull(payments, "payments");
         _payments = payments;
      }

      public int Id { get; private set; }
      public string ContractNumber { get; private set; }
      public string BorrowerName { get; private set; }
      public decimal ExchangeRate { get; private set; }
      public string PhoneNumber { get; private set; }

      public Payment[] Payments
      {
         get { return _payments ?? (_payments = _repository.GetPayments(Id)); }
      }

      public void SetId(int id)
      {
         Id = id;
      }
   }
}