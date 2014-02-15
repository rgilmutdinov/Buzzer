using Common;

namespace Buzzer.DomainModel.Services
{
   public static class SmsSenderFactory
   {
      public static ISmsSender GetSmsSender(string phoneNumber)
      {
         Check.NotNull(phoneNumber, "phoneNumber");

         var result = PhoneNumberRegex.PhoneNumberMatcher.Match(phoneNumber);
         var phone = new PhoneNumber(result.Groups[1].Value, result.Groups[2].Value);

         switch (phone.Code)
         {
            // Beeline.
            case "770": case "771": case "772": case "773":
            case "775": case "777": case "778": case "779":
               return new BeelineSmsSender(phone);

            // Megacom.
            case "550": case "551": case "552": case "553":
            case "554": case "555": case "556": case "557":
            case "558": case "559":
               return new MegacomSmsSender(phone);

            // O.
            case "700": case "701": case "702": case "703":
            case "704": case "705": case "706": case "707":
            case "708": case "709":
               return new OSmsSender(phone);
         }

         throw new UnknownMobileProviderException();
      }
   }
}