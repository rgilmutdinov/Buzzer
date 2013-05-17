using System.Text.RegularExpressions;

namespace Notifier.Common
{
   public static class PhoneNumberRegex
   {
      public static readonly Regex PhoneNumberMatcher = new Regex(@"^(\d{3})(\d{6})$", RegexOptions.Compiled);
   }
}