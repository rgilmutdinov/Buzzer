using System;

namespace Notifier.Common
{
   public static class Check
   {
      internal static void NotNull(object value, string name)
      {
         if (value == null)
            throw new ArgumentNullException(name);
      }
   }
}
