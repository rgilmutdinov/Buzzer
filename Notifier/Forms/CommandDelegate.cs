using System;
using System.Windows.Input;

namespace Notifier.Forms
{
   public class CommandDelegate : ICommand
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

      private bool _hasChange = false;

      public bool CanExecute(object parameter)
      {
         var newState = _predicate.Invoke();
         if (newState != _hasChange)
         {
            _hasChange = newState;
            if(CanExecuteChanged!= null)
               CanExecuteChanged(this, EventArgs.Empty);
         }
         return _hasChange;
      }

      public event EventHandler CanExecuteChanged;
   }
}