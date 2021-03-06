using Common;
using WatiN.Core;

namespace Buzzer.DomainModel.Services
{
   public sealed class BeelineSmsSender : ISmsSender
   {
      private readonly PhoneNumber _phoneNumber;

      public BeelineSmsSender(PhoneNumber phoneNumber)
      {
         Check.NotNull(phoneNumber, "phoneNumber");
         _phoneNumber = phoneNumber;
      }

      public void Send(string message)
      {
         var browser = new FireFox("http://sms.beeline.kg/");
         browser.SelectList(Find.ByName("code")).Select(_phoneNumber.Code);
         browser.TextField(Find.ByName("phone")).TypeText(_phoneNumber.Phone);
         browser.TextField(Find.ByName("message2")).TypeText(message);
         browser.TextField(Find.ByName("keystring")).Focus();
      }
   }
}