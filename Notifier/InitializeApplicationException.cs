using System;

namespace Notifier
{
   public class InitializeApplicationException : Exception
   {
      public InitializeApplicationException(string messageFormat, params object[] args)
         : base(string.Format(messageFormat, args))
      {
         
      }

      public InitializeApplicationException(string message) : base(message)
      {
      }
   }
}