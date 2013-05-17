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

      [Test]
      public void GetNotNotifiedPaymentsTest()
      {
         // Arrange.
         var today = DateTime.Today;

         var repository = new Repository();
         var contractCreator = new ContractCreator(repository);
         
         var notNotifiedPayments1 = new[]
                                      {
                                         new Payment(today.AddDays(-1), 100M, false),
                                         new Payment(today, 100M, false),
                                         new Payment(today.AddDays(1), 100M, false)
                                      };
         var notNotifiedContract1 = contractCreator.Create("1", "Borrower", 1.1M, "0 555 111111", notNotifiedPayments1);

         var notNotifiedPayments2 = new[] {new Payment(today, 150M, false)};
         var notNotifiedContract2 = contractCreator.Create("1.5", "Borrower1.5", 1.6M, "0 555 151515", notNotifiedPayments2);

         var notifiedPayments = new[]
                                      {
                                         new Payment(today.AddDays(-1), 200M, true),
                                         new Payment(today, 200M, true),
                                         new Payment(today.AddDays(1), 200M, true)
                                      };
         var notifiedContract = contractCreator.Create("2", "Borrower2", 2.2M, "0 555 222222", notifiedPayments);

         repository.SaveContract(notNotifiedContract1);
         repository.SaveContract(notNotifiedContract2);
         repository.SaveContract(notifiedContract);

         // Act.
         var result = repository.GetNotNotifiedPayments();

         // Assert.
         Assert.AreEqual(2, result.Length);

         var expectedContracts =
            new[] {notNotifiedContract1, notNotifiedContract2}
               .OrderBy(item => item.Id).ToArray();
         var actualContracts = result.OrderBy(item => item.Id).ToArray();

         for (var i = 0; i < expectedContracts.Length; i++)
         {
            Assert.AreEqual(expectedContracts[i].Id, actualContracts[i].Id);
            Assert.AreEqual(expectedContracts[i].ContractNumber, actualContracts[i].ContractNumber);
            Assert.AreEqual(expectedContracts[i].BorrowerName, actualContracts[i].BorrowerName);
            Assert.AreEqual(expectedContracts[i].ExchangeRate, actualContracts[i].ExchangeRate);
            Assert.AreEqual(expectedContracts[i].PhoneNumber, actualContracts[i].PhoneNumber);

            if (actualContracts[i].Id == notNotifiedContract1.Id)
            {
               var expectedPayments = notNotifiedPayments1;
               var actualPayments = actualContracts[i].Payments.OrderBy(item => item.Id).ToArray();
               
               Assert.AreEqual(2, actualPayments.Length);

               Assert.AreEqual(expectedPayments[0].Id, actualPayments[0].Id);
               Assert.AreEqual(expectedPayments[0].PaymentAmount, actualPayments[0].PaymentAmount);
               Assert.AreEqual(expectedPayments[0].PaymentDate, actualPayments[0].PaymentDate);

               Assert.AreEqual(expectedPayments[1].Id, actualPayments[1].Id);
               Assert.AreEqual(expectedPayments[1].PaymentAmount, actualPayments[1].PaymentAmount);
               Assert.AreEqual(expectedPayments[1].PaymentDate, actualPayments[1].PaymentDate);
            }
            else if (actualContracts[i].Id == notNotifiedContract2.Id)
            {
               var expectedPayments = notNotifiedPayments2;
               var actualPayments = actualContracts[i].Payments.OrderBy(item => item.Id).ToArray();

               Assert.AreEqual(1, actualPayments.Length);

               Assert.AreEqual(expectedPayments[0].Id, actualPayments[0].Id);
               Assert.AreEqual(expectedPayments[0].PaymentAmount, actualPayments[0].PaymentAmount);
               Assert.AreEqual(expectedPayments[0].PaymentDate, actualPayments[0].PaymentDate);
            }
         }
      }
   }
}
