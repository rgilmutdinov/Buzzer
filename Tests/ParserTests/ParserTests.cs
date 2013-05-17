using System;
using NUnit.Framework;
using Notifier.Parser;

namespace Tests.ParserTests
{
   [TestFixture]
   public sealed class ParserTests
   {
      private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

      [Test]
      public void ParseContractTest()
      {
         var data = ContractParser.Parse(getFullPath(@"ParserTests\Contract.xls"));

         Assert.AreEqual("67-2010", data.ContractNumber);
         Assert.AreEqual("Фамилия Имя Отчество", data.BorrowerName);
         Assert.AreEqual(47.2618M, data.ExchangeRate);
         Assert.AreEqual(3, data.PaymentsData.Length);

         Assert.AreEqual(new DateTime(2010, 10, 9), data.PaymentsData[0].Date);
         Assert.AreEqual(295M, data.PaymentsData[0].Amount);
         Assert.AreEqual(true, data.PaymentsData[0].IsNotified);

         Assert.AreEqual(new DateTime(2010, 11, 9), data.PaymentsData[1].Date);
         Assert.AreEqual(294M, data.PaymentsData[1].Amount);
         Assert.AreEqual(true, data.PaymentsData[1].IsNotified);

         Assert.AreEqual(new DateTime(2010, 12, 9), data.PaymentsData[2].Date);
         Assert.AreEqual(294M, data.PaymentsData[2].Amount);
         Assert.AreEqual(true, data.PaymentsData[2].IsNotified);
      }

      [Test]
      public void ThrowExceptionIfDataIsNotSpecified(
         [Values(@"ParserTests\ContractNumberIsNotSpecified.xls",
                 @"ParserTests\ContractNumberIsNotFilled.xls",
                 @"ParserTests\BorrowerNameIsNotSpecified.xls",
                 @"ParserTests\ExchangeRateIsNotSpecified.xls",
                 @"ParserTests\PaymentDateIsNotSpecified.xls",
                 @"ParserTests\PaymentAmountIsNotSpecified.xls"
                 )]
         string filename
         )
      {
         Assert.Throws<ParseContractException>(() => ContractParser.Parse(getFullPath(filename)));
      }

      private static string getFullPath(string relativePath)
      {
         return BaseDirectory + "\\" + relativePath;
      }
   }
}
