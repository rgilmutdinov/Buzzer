using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Buzzer.DomainModel.Properties;
using Common;

namespace Buzzer.DomainModel.Models
{
   public sealed class PersonInfo : DomainObject
   {
      private List<PhoneNumberInfo> _phoneNumbers;

      private PersonInfo()
      {
      }

      internal static PersonInfo CreateNew(int creditId)
      {
         return new PersonInfo
                   {
                      Id = NullValues.Id,
                      CreditId = creditId,
                      PassportIssueDate = NullValues.DateTime,
                      _phoneNumbers = new List<PhoneNumberInfo>()
                   };
      }

      public static PersonInfo Create(
         int id,
         int creditId,
         string personalNumber,
         string personName,
         string registrationAddress,
         string factAddress,
         string passportNumber,
         DateTime passportIssueDate,
         string passportIssuer,
         bool isBorrower,
         IEnumerable<PhoneNumberInfo> phoneNumbers 
         )
      {
         return new PersonInfo
                   {
                      Id = id,
                      CreditId = creditId,
                      PersonalNumber = personalNumber,
                      PersonName = personName,
                      RegistrationAddress = registrationAddress,
                      FactAddress = factAddress,
                      PassportNumber = passportNumber,
                      PassportIssueDate = passportIssueDate,
                      PassportIssuer = passportIssuer,
                      IsBorrower = isBorrower,
                      _phoneNumbers = phoneNumbers.ToList()
                   };
      }

      // Идентификатор кредита, к которому относится заемщик/поручитель.
      public int CreditId { get; set; }

      // ИНН.
      public string PersonalNumber { get; set; }

      // ФИО.
      public string PersonName { get; set; }

      // Адрес по прописке.
      public string RegistrationAddress { get; set; }

      // Фактический адрес проживания.
      public string FactAddress { get; set; }

      // Номер паспорта.
      public string PassportNumber { get; set; }

      // Дата выдачи паспорта.
      public DateTime PassportIssueDate { get; set; }

      // Орган, выдавший паспорт.
      public string PassportIssuer { get; set; }

      // Номера телефонов.
      public ReadOnlyCollection<PhoneNumberInfo> PhoneNumbers
      {
         get { return _phoneNumbers.AsReadOnly(); }
      }
      
      // Заемщик/Поручитель.
      public bool IsBorrower { get; internal set; }

      public PhoneNumberInfo AddPhoneNumber()
      {
         var phoneNumber = PhoneNumberInfo.CreateNew(Id);
         _phoneNumbers.Add(phoneNumber);
         return phoneNumber;
      }

      public void RemovePhoneNumber(PhoneNumberInfo phoneNumber)
      {
         _phoneNumbers.Remove(phoneNumber);
      }

      public override bool IsValid()
      {
         var isValid = base.IsValid();
         
         foreach (var phone in PhoneNumbers)
            isValid &= phone.IsValid();

         return isValid;
      }
      
      protected override string getErrorInfo(string columnName)
      {
         switch (columnName)
         {
            case "PersonalNumber":
               return validatePersonalNumber();

            case "PersonName":
               return validatePersonName();

            case "RegistrationAddress":
               return validateRegistrationAddress();

            case "FactAddress":
               return validateFactAddress();

            case "PassportNumber":
               return validatePassportNumber();

            case "PassportIssueDate":
               return validatePassportIssueDate();

            case "PassportIssuer":
               return validatePassportIssuer();

            case "PhoneNumbers":
               return validatePhoneNumbers();
         }

         throw new ArgumentException(columnName, "columnName");
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[]
                   {
                      "PersonalNumber",
                      "PersonName",
                      "RegistrationAddress",
                      "FactAddress",
                      "PassportNumber",
                      "PassportIssueDate",
                      "PassportIssuer"
                   };
      }

      private string validatePersonalNumber()
      {
         if (string.IsNullOrEmpty(PersonalNumber))
            return Resources.FieldMustBeFilled;

         if (PersonalNumber.Length != 14)
            return Resources.IncorrectPersonalNumberLength;

         return null;
      }

      private string validatePersonName()
      {
         if (PersonName.SafeGetLength() > 255)
            return string.Format(Resources.MaxLengthExceeded, 255);

         return string.IsNullOrEmpty(PersonName) ? Resources.FieldMustBeFilled : null;
      }

      private string validateRegistrationAddress()
      {
         if (RegistrationAddress.SafeGetLength() > 255)
            return string.Format(Resources.MaxLengthExceeded, 255);

         return string.IsNullOrEmpty(RegistrationAddress) ? Resources.FieldMustBeFilled : null;
      }

      private string validateFactAddress()
      {
         if (FactAddress.SafeGetLength() > 255)
            return string.Format(Resources.MaxLengthExceeded, 255);

         return string.IsNullOrEmpty(FactAddress) ? Resources.FieldMustBeFilled : null;
      }

      [Todo("Узнать формат номера паспорта и добавить валидацию на соответствие формату.")]
      private string validatePassportNumber()
      {
         if (PassportNumber.SafeGetLength() > 100)
            return string.Format(Resources.MaxLengthExceeded, 100);

         return string.IsNullOrEmpty(PassportNumber) ? Resources.FieldMustBeFilled : null;
      }

      private string validatePassportIssueDate()
      {
         return PassportIssueDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      private string validatePassportIssuer()
      {
         if (PassportIssuer.SafeGetLength() > 100)
            return string.Format(Resources.MaxLengthExceeded, 100);

         return string.IsNullOrEmpty(PassportIssuer) ? Resources.FieldMustBeFilled : null;
      }

      private string validatePhoneNumbers()
      {
         return _phoneNumbers.Count == 0 ? Resources.AtLeastOnePhoneNumberMustBeSpecified : null;
      }
   }
}