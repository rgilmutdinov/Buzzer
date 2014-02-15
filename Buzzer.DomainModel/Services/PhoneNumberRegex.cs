using System.Text.RegularExpressions;

namespace Buzzer.DomainModel.Services
{
   public static class PhoneNumberRegex
   {
      public static readonly Regex PhoneNumberMatcher = new Regex(@"^(\d{3})(\d{6})$", RegexOptions.Compiled);
   }
}