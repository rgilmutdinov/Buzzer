using System;
using System.Collections.Generic;
using DataAccess.Common;
using DataAccess.Helpers;
using DataAccess.Properties;

namespace DataAccess.Model
{
   public sealed class PhoneNumberInfo : RepositoryItem
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

      internal static PhoneNumberInfo Create(int id, int personId, string phoneNumber)
      {
         return new PhoneNumberInfo
                   {
                      Id = id,
                      PersonId = personId,
                      PhoneNumber = phoneNumber
                   };
      }

      // Идентификатор владельца телефона.
      public int PersonId { get; internal set; }

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