using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using DataAccess.Common;
using DataAccess.Helpers;
using DataAccess.Properties;

namespace DataAccess.Model
{
   public sealed class PersonInfo : RepositoryItem
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

      internal static PersonInfo Create(
         int id,
         int creditId,
         string personalNumber,
         string personName,
         string registrationAddress,
         string factAddress,
         string passportNumber,
         DateTime passportIssueDate,
         string passportIssuer,
         bool isBorrower
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
                      IsBorrower = isBorrower
                   };
      }

      // ������������� �������, � �������� ��������� �������/����������.
      public int CreditId { get; internal set; }

      // ���.
      public string PersonalNumber { get; set; }

      // ���.
      public string PersonName { get; set; }

      // ����� �� ��������.
      public string RegistrationAddress { get; set; }

      // ����������� ����� ����������.
      public string FactAddress { get; set; }

      // ����� ��������.
      public string PassportNumber { get; set; }

      // ���� ������ ��������.
      public DateTime PassportIssueDate { get; set; }

      // �����, �������� �������.
      public string PassportIssuer { get; set; }

      // ������ ���������.
      public ReadOnlyCollection<PhoneNumberInfo> PhoneNumbers
      {
         get
         {
            initializePhoneNumbers();
            return _phoneNumbers.AsReadOnly();
         }
      }
      
      // �������/����������.
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

      private void initializePhoneNumbers()
      {
         if (_phoneNumbers == null)
         {
            var phoneNumbers = Context.GetPhoneNumbers(Id);
            _phoneNumbers = new List<PhoneNumberInfo>();
            _phoneNumbers.AddRange(phoneNumbers);
         }
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

      [Todo("������ ������ ������ �������� � �������� ��������� �� ������������ �������.")]
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