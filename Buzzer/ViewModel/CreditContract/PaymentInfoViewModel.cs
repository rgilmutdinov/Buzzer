using System;
using Buzzer.ViewModel.Common;
using Common;
using DataAccess.Model;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PaymentInfoViewModel : ViewModelBase
   {
      public PaymentInfoViewModel(PaymentInfo paymentInfo, int number)
      {
         Check.NotNull(paymentInfo, "paymentInfo");

         Number = number;
         PaymentDate = paymentInfo.PaymentDate;

         var usd = paymentInfo.CurrencyPaymentAmount.HasValue;
         var paymentAmount = usd ? paymentInfo.CurrencyPaymentAmount.Value : paymentInfo.PaymentAmount;
         PaymentAmount = usd ? paymentAmount + " USD" : paymentAmount + " KGS";
      }

      public int Number { get; private set; }
      public DateTime PaymentDate { get; private set; }
      public string PaymentAmount { get; private set; }
   }
}