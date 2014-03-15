using System;
using System.Windows.Input;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditViewModel : ViewModelBase
   {
      private readonly CreditInfo _creditInfo;
      private readonly IWorkspaceManager _workspaceManager;

      private CreditState _creditState;

      private ICommand _openCreditCommand;

      public CreditViewModel(CreditInfo creditInfo, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(creditInfo, "creditInfo");
         Check.NotNull(workspaceManager, "workspaceManager");

         _creditInfo = creditInfo;
         _workspaceManager = workspaceManager;

         CreditNumber = _creditInfo.CreditNumber;
         BorrowerName = _creditInfo.Borrower.PersonName;
         CreditAmount = getCreditAmount();
         CreditIssueDate = _creditInfo.CreditIssueDate;
         CreditEndDate = getCreditEndDate();
         DiscountRate = getDiscountRate();
         CreditState = _creditInfo.CreditState;
      }

      public CreditInfo Original
      {
         get { return _creditInfo; }
      }

      public string CreditNumber { get; set; }

      public string BorrowerName { get; set; }

      public string CreditAmount { get; set; }

      public DateTime CreditIssueDate { get; set; }

      public DateTime CreditEndDate { get; set; }

      public decimal DiscountRate { get; set; }

      public CreditState CreditState
      {
         get { return _creditState; }
         set
         {
            if (_creditState == value)
               return;

            _creditState = value;
            propertyChanged("CreditState");
         }
      }

      public ICommand OpenCredit
      {
         get
         {
            if (_openCreditCommand != null)
               return _openCreditCommand;

            _openCreditCommand = new CommandDelegate(openCredit);
            return _openCreditCommand;
         }
      }

      private void openCredit()
      {
         _workspaceManager.ShowCreditInfo(_creditInfo);
      }

      private string getCreditAmount()
      {
         return string.Format("{0} KGS", _creditInfo.CreditAmount);
      }

      private decimal getDiscountRate()
      {
         return _creditInfo.DiscountRate * 100M;
      }

      private DateTime getCreditEndDate()
      {
         return _creditInfo.CreditIssueDate.AddMonths(_creditInfo.MonthsCount);
      }
   }
}