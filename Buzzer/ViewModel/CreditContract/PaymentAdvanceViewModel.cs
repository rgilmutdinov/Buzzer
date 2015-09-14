using System;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PaymentAdvanceViewModel : ViewModelBase
   {
      public PaymentAdvanceViewModel(PaymentAdvance paymentAdvance)
      {
         Check.NotNull(paymentAdvance, "paymentAdvance");

         Original = paymentAdvance;
      }

      public PaymentAdvance Original { get; private set; }

      public DateTime DueDate
      {
         get { return Original.DueDate; }
      }

      // Сумма выданного займа.
      public decimal CreditSum
      {
         get { return Original.Payment.CreditSum; }
      }

      // Проценты к оплате.
      public decimal PercentSum
      {
         get { return Original.Payment.PercentSum; }
      }

      // Основная сумма к оплате.
      public decimal BaseSum
      {
         get { return Original.Payment.BaseSum; }
      }

      // Итого к оплате.
      public decimal TotalSum
      {
         get { return Original.Payment.TotalSum; }
      }

      // Налог.
      public decimal Tax
      {
         get { return Original.Payment.Tax; }
      }

      // Всего к оплате.
      public decimal PaymentAmount
      {
         get { return Original.Payment.PaymentAmount; }
      }

      public decimal? Balance
      {
         get { return Original.Balance; }
      }

      public decimal? Penalty
      {
         get { return Original.Penalty; }
      }

      public PaymentAdvanceState State
      {
         get { return Original.State; }
      }
   }
}
