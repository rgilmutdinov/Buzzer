using System;
using System.Collections.Generic;
using System.Linq;
using Notifier.Common;

namespace Notifier.Database
{
   public sealed class ContractRepositoryDecorator
   {
      private readonly Repository _repository;

      public ContractRepositoryDecorator(Repository repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;
      }

      public ContractLight[] GetContracts(DateTime begin, DateTime end)
      {
         var contracts = _repository.GetContracts();
         var result = new List<ContractLight>();

         foreach (var contract in contracts)
         {
            var contractLight = new ContractLight
                                   {
                                      Id = contract.Id,
                                      ContractNumber = contract.ContractNumber,
                                      BorrowerName = contract.BorrowerName,
                                      ExchangeRate = contract.ExchangeRate,
                                      PhoneNumber = contract.PhoneNumber
                                   };

            foreach (var payment in contract.Payments)
            {
               if (begin <= payment.PaymentDate && payment.PaymentDate <= end)
               {
                  contractLight.AddPayment(
                     new PaymentLight
                        {
                           Id = payment.Id,
                           PaymentAmount = payment.PaymentAmount,
                           PaymentDate = payment.PaymentDate,
                           IsNotified = payment.IsNotified
                        }
                     );
               }
            }

            if (contractLight.Payments.Any())
               result.Add(contractLight);
         }

         return result.ToArray();
      }

      public void UpdatePhoneNumber(int contractId, string phoneNumber)
      {
         _repository.UpdatePhoneNumber(contractId, phoneNumber);
      }

      public void UpdateIsNotified(int paymentId, bool isNotified)
      {
         _repository.UpdateIsNotified(paymentId, isNotified);
      }
   }
}