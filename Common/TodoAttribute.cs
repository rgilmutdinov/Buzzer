using System;

namespace Common
{
   [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
   public class TodoAttribute : Attribute
   {
      public TodoAttribute()
      {
      }

      public TodoAttribute(string message)
      {
      }
   }
}