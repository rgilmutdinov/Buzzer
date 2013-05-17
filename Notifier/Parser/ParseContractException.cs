using System;

namespace Notifier.Parser
{
   public sealed class ParseContractException : Exception
   {
      public ParseContractException(string message)
         : base(message)
      {
      }
   }
}