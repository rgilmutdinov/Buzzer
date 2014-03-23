using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.CreditContract;
using Buzzer.ViewModel.CreditsList;
using Buzzer.ViewModel.NotificationLog;
using Buzzer.ViewModel.PaymentNotificationList;
using Common;

namespace Buzzer.ViewModel.MainWindow
{
   public sealed class MainWindowViewModel : ViewModelBase, IWorkspaceManager
   {
      private readonly BuzzerDatabase _buzzerDatabase;
      private readonly LoginManager _loginManager;

      private ObservableCollection<WorkspaceViewModel> _workspaces;
      private IEnumerable<CommandViewModel> _commands;

      public MainWindowViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         
         _buzzerDatabase = buzzerDatabase;
         DisplayName = Resources.MainWindowViewModel_BuzzerCaption;

         _loginManager = new LoginManager(buzzerDatabase);
      }

      public ObservableCollection<WorkspaceViewModel> Workspaces
      {
         get
         {
            if (_workspaces != null)
               return _workspaces;

            _workspaces = new ObservableCollection<WorkspaceViewModel>();
            _workspaces.CollectionChanged += onWorkspacesChanged;

            return _workspaces;
         }
      }

      public IEnumerable<CommandViewModel> Commands
      {
         get
         {
            if (_commands != null)
               return _commands;

            _commands =
               new[]
                  {
                     new CommandViewModel(
                        Resources.MainWindowViewModel_CreditsList,
                        new CommandDelegate(showCreditsList)
                        ),

                     new CommandViewModel(
                        Resources.MainWindowViewModel_PaymentNotificationList,
                        new CommandDelegate(showPaymentNotificationList)),
                        
                     new CommandViewModel(
                        Resources.MainWindowViewModel_NotificationLog,
                        new CommandDelegate(showNotificationLog)
                        ),

                     new CommandViewModel(
                        Resources.MainWindowViewModel_GuaranteeCredit,
                        new CommandDelegate(createNewGuaranteeCredit)
                        ),
                  };

            return _commands;
         }
      }

      public void ShowCreditInfo(CreditInfo credit)
      {
         var workspace = new CreditContractViewModel(credit, _buzzerDatabase);
         addWorkspace(workspace);
      }

      private void addWorkspace(WorkspaceViewModel workspace)
      {
         Workspaces.Add(workspace);
         setActiveWorkspace(workspace);
      }

      private void onWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.NewItems != null && e.NewItems.Count != 0)
            foreach (WorkspaceViewModel workspace in e.NewItems)
               workspace.RequestClose += onWorkspaceRequestClose;

         if (e.OldItems != null && e.OldItems.Count != 0)
            foreach (WorkspaceViewModel workspace in e.OldItems)
               workspace.RequestClose -= onWorkspaceRequestClose;
      }

      private void onWorkspaceRequestClose(object sender, EventArgs e)
      {
         var workspace = sender as WorkspaceViewModel;
         workspace.Dispose();
         Workspaces.Remove(workspace);
      }

      private void setActiveWorkspace(WorkspaceViewModel workspace)
      {
         var collectionView = CollectionViewSource.GetDefaultView(Workspaces);
         if (collectionView != null)
            collectionView.MoveCurrentTo(workspace);
      }

      private void showCreditsList()
      {
         var workspace = Workspaces.FirstOrDefault(item => item is CreditsListViewModel);
         
         if (workspace == null)
            addWorkspace(new CreditsListViewModel(_buzzerDatabase, this));
         else
            setActiveWorkspace(workspace);
      }

      private void showPaymentNotificationList()
      {
         var workspace = Workspaces.FirstOrDefault(item => item is PaymentNotificationListViewModel);

         if (workspace == null)
            addWorkspace(new PaymentNotificationListViewModel(_buzzerDatabase));
         else
            setActiveWorkspace(workspace);
      }

      private void showNotificationLog()
      {
         var workspace = Workspaces.FirstOrDefault(item => item is NotificationLogViewModel);

         if (workspace == null)
         {
            if (_loginManager.Login())
               addWorkspace(new NotificationLogViewModel(_buzzerDatabase));
         }
         else
            setActiveWorkspace(workspace);
      }

      private void createNewGuaranteeCredit()
      {
         var credit = CreditInfo.CreatNew();
         var workspace = new CreditContractViewModel(credit, _buzzerDatabase);
         addWorkspace(workspace);
      }
   }
}