using System;

namespace Buzzer.DataAccess.Common
{
   [Obsolete("", true)]
   public static class NullValues
   {
      public const int Id = -1;
      public static readonly DateTime DateTime = new DateTime(1900, 1, 1);
   }
}