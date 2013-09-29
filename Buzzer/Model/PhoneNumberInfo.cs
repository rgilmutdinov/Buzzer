using System.ComponentModel;
using Buzzer.Common;
using Buzzer.Properties;

namespace Buzzer.Model
{
   public sealed class PhoneNumberInfo : IDataErrorInfo
   {
      private PhoneNumberInfo()
      {
      }

      public static PhoneNumberInfo CreateNew()
      {
         return new PhoneNumberInfo {Id = NullValues.Id};
      }

      public int Id { get; set; }

      // Номер телефона.
      public string PhoneNumber { get; set; }

      private string validatePhoneNumber()
      {
         return string.IsNullOrEmpty(PhoneNumber) ? Resources.FieldMustBeFilled : null;
      }

      string IDataErrorInfo.this[string columnName]
      {
         get
         { 
            if (columnName == "PhoneNumber")
               return validatePhoneNumber();

            return null;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }
   }
}