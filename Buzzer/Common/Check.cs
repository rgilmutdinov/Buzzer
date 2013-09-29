using System;

namespace Buzzer.Common
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
