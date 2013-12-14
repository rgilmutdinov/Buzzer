using System.Windows.Input;
using Common;

namespace Buzzer.ViewModel.Common
{
   public class CommandViewModel : ViewModelBase
   {
      public CommandViewModel(string displayName, ICommand command)
      {
         Check.NotNull(displayName, "displayName");
         Check.NotNull(command, "command");

         DisplayName = displayName;
         Command = command;
      }

      public ICommand Command { get; private set; }
   }
}