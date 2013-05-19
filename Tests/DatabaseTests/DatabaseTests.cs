using System;
using System.Linq;
using NUnit.Framework;
using Notifier.Database;

namespace Tests.DatabaseTests
{
   [TestFixture]
   public sealed class DatabaseTests
   {
      [Test]
      public void AddContractTest()
      {
         // Arrange.
         var payments = new[]
                           {
                              new Payment(new DateTime(2010, 1, 1), 100.0001M, true), 
                              new Payment(new DateTime(2010, 2, 1), 200.0002M, false)
                           };
         var repository = new Repository();
         var contractCreator = new ContractCreator(repository);
         var contract = contractCreator.Create("67-2010", "Имя заемщика", 45.0123M, "0 555 123456", payments);

         // Act.
         repository.SaveContract(contract);

         // Assert.
         Assert.AreNotEqual(0, contract.Id);
         Assert.AreNotEqual(0, payments[0].Id);
         Assert.AreNotEqual(0, payments[1].Id);

         Assert.AreEqual(contract.Id, payments[0].ContractId);
         Assert.AreEqual(contract.Id, payments[1].ContractId);

         var contractFromDb = repository.GetContract(contract.Id);

         Assert.IsNotNull(contractFromDb);
         Assert.AreEqual(contract.Id, contractFromDb.Id);
         Assert.AreEqual(contract.ContractNumber, contractFromDb.ContractNumber);
         Assert.AreEqual(contract.BorrowerName, contractFromDb.BorrowerName);
         Assert.AreEqual(contract.ExchangeRate, contractFromDb.ExchangeRate);
         Assert.AreEqual(contract.PhoneNumber, contractFromDb.PhoneNumber);
         Assert.AreEqual(contract.Payments.Length, contractFromDb.Payments.Length);

         var orderPayments = contract.Payments.OrderBy(p => p.Id).ToArray();
         var orderPaymentsFromDb = contractFromDb.Payments.OrderBy(p => p.Id).ToArray();

         for (var i = 0; i < orderPayments.Length; i++)
         {
            Assert.AreEqual(orderPayments[i].Id, orderPaymentsFromDb[i].Id);
            Assert.AreEqual(orderPayments[i].ContractId, orderPaymentsFromDb[i].ContractId);
            Assert.AreEqual(orderPayments[i].PaymentDate, orderPaymentsFromDb[i].PaymentDate);
            Assert.AreEqual(orderPayments[i].PaymentAmount, orderPaymentsFromDb[i].PaymentAmount);
            Assert.AreEqual(orderPayments[i].IsNotified, orderPaymentsFromDb[i].IsNotified);
         }
      }
   }
}
