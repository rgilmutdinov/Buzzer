namespace Buzzer.DomainModel.Models
{
   internal static class StringHelper
   {
      internal static int SafeGetLength(this string text)
      {
         return text == null ? 0 : text.Length;
      }
   }
}