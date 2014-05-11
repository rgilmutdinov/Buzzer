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
         var browser = new FireFox("http://o.kg/ru/chastnym_klientam/mobilnaja_svjaz/otpravka_sms_s_sajta");
         var frame = browser.Frame(Find.BySrc("http://portal.o.kg/SMS/index/index2"));
         frame.SelectList("MSISDN_PREFIX").Select(_phoneNumber.Code);
         frame.TextField("_MSISDN").TypeText(_phoneNumber.Phone);
         frame.TextField("TEXT_SMS").TypeText(message);
         frame.TextField("CAPTCHA_CODE").Focus();
      }
   }
}