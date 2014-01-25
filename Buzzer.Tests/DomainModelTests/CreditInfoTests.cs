using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DomainModelTests
{
   [TestFixture]
   public class CreditInfoTests
   {
      [Test]
      [TestCaseSource("TestCases")]
      public void BuildPaymentsScheduleTest(
         decimal creditAmount,
         DateTime creditIssueDate,
         int monthsCount,
         decimal discountRate,
         decimal? exchangeRate,
         LightPaymentInfo[] expectedPayments
         )
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();
         credit.CreditAmount = creditAmount;
         credit.CreditIssueDate = creditIssueDate;
         credit.MonthsCount = monthsCount;
         credit.DiscountRate = discountRate;
         credit.ExchangeRate = exchangeRate;

         // Act.
         credit.BuildPaymentsSchedule();
         
         // Assert.
         PaymentInfo[] actualPayments = credit.PaymentsSchedule;
         
         Assert.IsNotNull(actualPayments);
         Assert.AreEqual(expectedPayments.Length, actualPayments.Length);

         for (int i = 0; i < expectedPayments.Length; i++)
         {
            Assert.AreEqual(expectedPayments[i].PaymentAmount, actualPayments[i].PaymentAmount);
            Assert.AreEqual(expectedPayments[i].PaymentDate, actualPayments[i].PaymentDate);
         }
      }

      public TestCaseData[] TestCases
      {
         get
         {
            var testCases = new List<TestCaseData>();

            {
               const decimal creditAmount = 100000M;
               var creditIssueDate = new DateTime(2013, 5, 22);
               const int monthsCount = 24;
               const decimal discountRate = 0.36M;
               decimal? exchangeRate = null;
               var payments =
                  new[]
                     {
                        new LightPaymentInfo(5965, new DateTime(2013, 6, 22)),
                        new LightPaymentInfo(5963, new DateTime(2013, 7, 22)),
                        new LightPaymentInfo(5961, new DateTime(2013, 8, 22)),
                        new LightPaymentInfo(5959, new DateTime(2013, 9, 22)),
                        new LightPaymentInfo(5957, new DateTime(2013, 10, 22)),
                        new LightPaymentInfo(5955, new DateTime(2013, 11, 22)),
                        new LightPaymentInfo(5953, new DateTime(2013, 12, 22)),
                        new LightPaymentInfo(5951, new DateTime(2014, 1, 22)),
                        new LightPaymentInfo(5949, new DateTime(2014, 2, 22)),
                        new LightPaymentInfo(5947, new DateTime(2014, 3, 22)),
                        new LightPaymentInfo(5945, new DateTime(2014, 4, 22)),
                        new LightPaymentInfo(5942, new DateTime(2014, 5, 22)),
                        new LightPaymentInfo(5940, new DateTime(2014, 6, 22)),
                        new LightPaymentInfo(5938, new DateTime(2014, 7, 22)),
                        new LightPaymentInfo(5935, new DateTime(2014, 8, 22)),
                        new LightPaymentInfo(5932, new DateTime(2014, 9, 22)),
                        new LightPaymentInfo(5930, new DateTime(2014, 10, 22)),
                        new LightPaymentInfo(5927, new DateTime(2014, 11, 22)),
                        new LightPaymentInfo(5924, new DateTime(2014, 12, 22)),
                        new LightPaymentInfo(5921, new DateTime(2015, 1, 22)),
                        new LightPaymentInfo(5918, new DateTime(2015, 2, 22)),
                        new LightPaymentInfo(5915, new DateTime(2015, 3, 22)),
                        new LightPaymentInfo(5912, new DateTime(2015, 4, 22)),
                        new LightPaymentInfo(5908, new DateTime(2015, 5, 22))
                     };

               var testCaseData = new TestCaseData(creditAmount, creditIssueDate, monthsCount,
                                                   discountRate, exchangeRate, payments);
               testCaseData.SetName("BuildPaymentsScheduleWithoutExchangeRateSpecifiedTest");
               testCases.Add(testCaseData);
            }

            {
               const decimal creditAmount = 300000M;
               var creditIssueDate = new DateTime(2010, 9, 9);
               const int monthsCount = 36;
               const decimal discountRate = 0.36M;
               decimal? exchangeRate = 47.2618M;
               var payments =
                  new[]
                     {
                        new LightPaymentInfo(295, new DateTime(2010, 10, 9)),
                        new LightPaymentInfo(294, new DateTime(2010, 11, 9)),
                        new LightPaymentInfo(294, new DateTime(2010, 12, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 1, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 2, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 3, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 4, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 5, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 6, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 7, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 8, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 9, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 10, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 11, 9)),
                        new LightPaymentInfo(294, new DateTime(2011, 12, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 1, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 2, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 3, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 4, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 5, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 6, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 7, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 8, 9)),
                        new LightPaymentInfo(293, new DateTime(2012, 9, 9)),
                        new LightPaymentInfo(292, new DateTime(2012, 10, 9)),
                        new LightPaymentInfo(292, new DateTime(2012, 11, 9)),
                        new LightPaymentInfo(292, new DateTime(2012, 12, 9)),
                        new LightPaymentInfo(292, new DateTime(2013, 1, 9)),
                        new LightPaymentInfo(292, new DateTime(2013, 2, 9)),
                        new LightPaymentInfo(292, new DateTime(2013, 3, 9)),
                        new LightPaymentInfo(292, new DateTime(2013, 4, 9)),
                        new LightPaymentInfo(292, new DateTime(2013, 5, 9)),
                        new LightPaymentInfo(291, new DateTime(2013, 6, 9)),
                        new LightPaymentInfo(291, new DateTime(2013, 7, 9)),
                        new LightPaymentInfo(291, new DateTime(2013, 8, 9)),
                        new LightPaymentInfo(291, new DateTime(2013, 9, 9))
                     };

               var testCaseData = new TestCaseData(creditAmount, creditIssueDate, monthsCount,
                                                   discountRate, exchangeRate, payments);
               testCaseData.SetName("BuildPaymentsScheduleWithExchangeRateSpecifiedTest");
               testCases.Add(testCaseData);
            }

            return testCases.ToArray();
         }
      }

      public class LightPaymentInfo
      {
         public LightPaymentInfo(decimal paymentAmount, DateTime paymentDate)
         {
            PaymentAmount = paymentAmount;
            PaymentDate = paymentDate;
         }

         public decimal PaymentAmount { get; private set; }
         public DateTime PaymentDate { get; private set; }
      }
   }
}
