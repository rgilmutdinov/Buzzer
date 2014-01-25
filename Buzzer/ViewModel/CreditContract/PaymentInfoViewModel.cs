using System;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PaymentInfoViewModel : ViewModelBase
   {
      public PaymentInfoViewModel(PaymentInfo paymentInfo, int number, bool isUsd)
      {
         Check.NotNull(paymentInfo, "paymentInfo");

         Number = number;
         PaymentDate = paymentInfo.PaymentDate;

         PaymentAmount =
            isUsd
               ? paymentInfo.PaymentAmount + " USD"
               : paymentInfo.PaymentAmount + " KGS";
      }

      public int Number { get; private set; }
      public DateTime PaymentDate { get; private set; }
      public string PaymentAmount { get; private set; }
   }
}