using System.ComponentModel;
using Common;
using DataAccess.Properties;

namespace DataAccess.Model
{
   public sealed class PhoneNumberInfo : RepositoryItem, IDataErrorInfo
   {
      private PhoneNumberInfo()
      {
      }

      public static PhoneNumberInfo CreateNew()
      {
         return new PhoneNumberInfo {Id = NullValues.Id};
      }

      // ����� ��������.
      public string PhoneNumber { get; set; }

      private string validatePhoneNumber()
      {
         return string.IsNullOrEmpty(PhoneNumber) ? Resources.FieldMustBeFilled : null;
      }

      // ������������� ��������� ��������.
      public int PersonId { get; set; }

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