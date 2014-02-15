using Common;
using WatiN.Core;

namespace Buzzer.DomainModel.Services
{
   public sealed class OSmsSender : ISmsSender
   {
      private readonly PhoneNumber _phoneNumber;

      public OSmsSender(PhoneNumber phoneNumber)
      {
         Check.NotNull(phoneNumber, "phoneNumber");
         _phoneNumber = phoneNumber;
      }

      public void Send(string message)
      {
         var browser = new FireFox("http://www.o.kg/pages/show/?id=306");
         var frame = browser.Frame(Find.BySrc("http://portal.o.kg/SMS/?lang=ru"));
         frame.SelectList("MSISDN_PREFIX").Select(_phoneNumber.Code);
         frame.TextField("_MSISDN").TypeText(_phoneNumber.Phone);
         frame.TextField("TEXT_SMS").TypeText(message);
         frame.TextField("CAPTCHA_CODE").Focus();
      }
   }
}