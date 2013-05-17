using System.Threading;

namespace Notifier.Common
{
   public sealed class PeriodicTask
   {
      private readonly ManualResetEvent _stop;
      private RegisteredWaitHandle _registeredWait;
      
      private readonly IPeriodicAction _action;
      private readonly int _timeout;

      public PeriodicTask(IPeriodicAction action, int timeout)
      {
         Check.NotNull(action, "action");

         _action = action;
         _timeout = timeout;

         _stop = new ManualResetEvent(false);
      }

      public void Start()
      {
         _stop.Reset();
         _registeredWait = ThreadPool.RegisterWaitForSingleObject(_stop, callback, null, _timeout, false);
      }

      public void Stop()
      {
         _stop.Set();
         _stop.Dispose();
      }

      private void callback(object state, bool timeout)
      {
         if (timeout)
         {
            _action.Execute();
         }
         else
         {
            _registeredWait.Unregister(null);
         }
      }
   }
}