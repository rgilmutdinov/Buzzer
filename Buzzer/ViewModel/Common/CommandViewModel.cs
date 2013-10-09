using System.Windows.Input;
using Common;

namespace Buzzer.ViewModel.Common
{
   public class CommandViewModel
   {
      public CommandViewModel(string displayName, ICommand command)
      {
         Check.NotNull(displayName, "displayName");
         Check.NotNull(command, "command");

         DisplayName = displayName;
         Command = command;
      }

      public string DisplayName { get; private set; }
      public ICommand Command { get; private set; }
   }
}