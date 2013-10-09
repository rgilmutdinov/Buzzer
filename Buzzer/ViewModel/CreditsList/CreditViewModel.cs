using System;
using Buzzer.ViewModel.Common;
using Common;
using DataAccess.Model;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditViewModel : ViewModelBase
   {
      private readonly CreditInfo _creditInfo;

      public CreditViewModel(CreditInfo creditInfo)
      {
         Check.NotNull(creditInfo, "creditInfo");
         _creditInfo = creditInfo;
      }

      public CreditViewModel(
         string creditNumber,
         string borrowerName,
         decimal? creditAmount,
         DateTime? creditIssueDate,
         DateTime? creditEndDate,
         decimal? discountRate)
      {
         
      }
      
      public string CreditNumber
      {
         get { return _creditInfo.CreditNumber; }
      }

      public string BorrowerName
      {
         get { return _creditInfo.Borrower.PersonName; }
      }

      public decimal CreditAmount
      {
         get { return _creditInfo.CreditAmount; }
      }

      public DateTime CreditIssueDate
      {
         get { return _creditInfo.CreditIssueDate; }
      }

      public DateTime CreditEndDate
      {
         get { return _creditInfo.CreditIssueDate.AddMonths(_creditInfo.MonthsCount); }
      }

      public decimal DiscountRate
      {
         get { return _creditInfo.DiscountRate; }
      }
   }
}