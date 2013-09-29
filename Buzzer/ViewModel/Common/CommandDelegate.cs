using System;
using System.Windows.Input;

namespace Buzzer.ViewModel.Common
{
   public sealed class CommandDelegate : ICommand
   {
      private readonly Action _invoker;
      private readonly Func<bool> _predicate;

      public CommandDelegate(Action invoker)
      {
         _invoker = invoker;
         _predicate = () => true;
      }

      public CommandDelegate(Action invoker, Func<bool> predicate)
      {
         _invoker = invoker;
         _predicate = predicate;
      }

      public void Execute(object parameter)
      {
         _invoker.Invoke();
      }

      public bool CanExecute(object parameter)
      {
         return _predicate == null || _predicate();
      }

      public event EventHandler CanExecuteChanged
      {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }
   }
}