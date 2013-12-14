using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Buzzer.ViewModel.CreditContract;
using Buzzer.ViewModel.CreditsList;
using Common;
using DataAccess.Model;
using DataAccess.Repository;

namespace Buzzer.ViewModel.MainWindow
{
   public sealed class MainWindowViewModel : ViewModelBase
   {
      private readonly BuzzerDatabase _buzzerDatabase;
      private ObservableCollection<WorkspaceViewModel> _workspaces;
      private IEnumerable<CommandViewModel> _commands;

      public MainWindowViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         
         _buzzerDatabase = buzzerDatabase;
         DisplayName = Resources.MainWindowViewModel_BuzzerCaption;
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
                        Resources.MainWindowViewModel_GuaranteeCredit,
                        new CommandDelegate(createNewGuaranteeCredit)
                        ), 
                  };

            return _commands;
         }
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
         var workspace =
            Workspaces.FirstOrDefault(item => item is CreditsListViewModel) as CreditsListViewModel;
         if (workspace == null)
         {
            workspace = new CreditsListViewModel(_buzzerDatabase);
            Workspaces.Add(workspace);
         }
         setActiveWorkspace(workspace);
      }

      private void createNewGuaranteeCredit()
      {
         var credit = CreditInfo.CreatNew();
         var workspace = new CreditContractViewModel(credit, _buzzerDatabase);
         Workspaces.Add(workspace);
         setActiveWorkspace(workspace);
      }
   }
}