using Common;

namespace Buzzer.DomainModel.Services
{
   public sealed class PhoneNumber
   {
      public PhoneNumber(string code, string phone)
      {
         Check.NotNull(code, "code");
         Check.NotNull(phone, "phone");

         Code = code;
         Phone = phone;
      }

      public string Code { get; private set; }
      public string Phone { get; private set; }
   }
}