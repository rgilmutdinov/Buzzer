using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

namespace Buzzer.DomainModel.Models
{
   public sealed class PhoneNumberInfo : DomainObject
   {
      private PhoneNumberInfo()
      {
      }

      internal static PhoneNumberInfo CreateNew(int personId)
      {
         return new PhoneNumberInfo
                   {
                      Id = NullValues.Id,
                      PersonId = personId
                   };
      }

      public static PhoneNumberInfo Create(int id, int personId, string phoneNumber)
      {
         return new PhoneNumberInfo
                   {
                      Id = id,
                      PersonId = personId,
                      PhoneNumber = phoneNumber
                   };
      }

      // Идентификатор владельца телефона.
      public int PersonId { get; set; }

      // Номер телефона.
      public string PhoneNumber { get; set; }

      protected override string getErrorInfo(string columnName)
      {
         if (columnName == "PhoneNumber")
            return validatePhoneNumber();

         throw new ArgumentException(columnName, "columnName");
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[] {"PhoneNumber"};
      }

      private string validatePhoneNumber()
      {
         if (PhoneNumber.SafeGetLength() > 100)
            return string.Format(Resources.MaxLengthExceeded, 100);

         return string.IsNullOrEmpty(PhoneNumber) ? Resources.FieldMustBeFilled : null;
      }
   }
}