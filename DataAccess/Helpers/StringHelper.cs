using System;

namespace Buzzer.DataAccess.Helpers
{
   [Obsolete("", true)]
   internal static class StringHelper
   {
      internal static int SafeGetLength(this string text)
      {
         return text == null ? 0 : text.Length;
      }
   }
}