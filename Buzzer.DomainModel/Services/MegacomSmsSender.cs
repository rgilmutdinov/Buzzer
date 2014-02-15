using Common;
using WatiN.Core;

namespace Buzzer.DomainModel.Services
{
   public class MegacomSmsSender : ISmsSender
   {
      private readonly PhoneNumber _phoneNumber;

      public MegacomSmsSender(PhoneNumber phoneNumber)
      {
         Check.NotNull(phoneNumber, "phoneNumber");
         _phoneNumber = phoneNumber;
      }

      public void Send(string message)
      {
         var browser = new FireFox("http://megacom.kg/rus/freesms/");
         browser.SelectList(Find.ByName("prefix")).Select(string.Format("0{0}", _phoneNumber.Code));
         browser.TextField(Find.ByName("msisdn")).TypeText(_phoneNumber.Phone);
         browser.TextField(Find.ByName("messege")).TypeText(message);
         browser.TextField(Find.ByName("captcha")).Focus();
      }
   }
}
