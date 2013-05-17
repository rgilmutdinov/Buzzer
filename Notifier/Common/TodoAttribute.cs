using System;

namespace Notifier.Common
{
   [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
   public class TodoAttribute : Attribute
   {
      public TodoAttribute(string message)
      {
      }
   }
}