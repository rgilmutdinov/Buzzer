using System;
using System.Windows.Input;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;
using DataAccess.Model;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditViewModel : ViewModelBase
   {
      private readonly CreditInfo _creditInfo;
      private readonly IWorkspaceManager _workspaceManager;

      public CreditViewModel(CreditInfo creditInfo, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(creditInfo, "creditInfo");
         _creditInfo = creditInfo;
         _workspaceManager = workspaceManager;
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

      public ICommand OpenCredit
      {
         get
         {
            return
               new CommandDelegate(
                  () => _workspaceManager.ShowCreditInfo(_creditInfo)
                  );
         }
      }
   }
}