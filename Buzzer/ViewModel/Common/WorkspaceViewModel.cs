using System;
using System.Windows.Input;

namespace Buzzer.ViewModel.Common
{
   public abstract class WorkspaceViewModel : ViewModelBase
   {
      private ICommand _closeCommand;

      public ICommand CloseCommand
      {
         get
         {
            if (_closeCommand != null)
               return _closeCommand;

            _closeCommand = new CommandDelegate(onRequestClose);

            return _closeCommand;
         }
      }

      public event EventHandler RequestClose;

      private void onRequestClose()
      {
         if (RequestClose != null)
            RequestClose(this, EventArgs.Empty);
      }
   }
}