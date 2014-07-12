using System;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.MainWindow;
using Common;

namespace Buzzer.ViewModel.RegistrationLog
{
   public sealed class RegistrationLogItemViewModel : ViewModelBase
   {
      private readonly CreditInfo _creditInfo;
      private readonly IWorkspaceManager _workspaceManager;

      public RegistrationLogItemViewModel(CreditInfo creditInfo, IWorkspaceManager workspaceManager)
      {
         Check.NotNull(creditInfo, "creditInfo");
         Check.NotNull(workspaceManager, "workspaceManager");

         _creditInfo = creditInfo;
         _workspaceManager = workspaceManager;
      }

      public CreditInfo Original
      {
         get { return _creditInfo; }
      }

      public bool IsChanged { get; set; }

      public DateTime? ApplicationDate
      {
         get { return _creditInfo.ApplicationDate; }
      }

      public string BorrowerName
      {
         get { return _creditInfo.Borrower.PersonName; }
      }

      public CreditState CreditState
      {
         get { return _creditInfo.CreditState; }
      }

      public DateTime? ProtocolDate
      {
         get { return _creditInfo.ProtocolDate; }
         set
         {
            if (_creditInfo.ProtocolDate == value)
               return;

            _creditInfo.ProtocolDate = value;
            propertyChanged("ProtocolDate");

            IsChanged = true;
         }
      }

      public string RefusalReason
      {
         get { return _creditInfo.RefusalReason; }
         set
         {
            if (_creditInfo.RefusalReason == value)
               return;

            _creditInfo.RefusalReason = value;
            propertyChanged("RefusalReason");

            IsChanged = true;
         }
      }

      public void OpenCredit()
      {
         _workspaceManager.ShowCreditInfo(_creditInfo);
      }
   }
}