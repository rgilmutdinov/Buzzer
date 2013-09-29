using System;
using System.Collections.Generic;
using System.ComponentModel;
using Buzzer.Common;
using Buzzer.Properties;

namespace Buzzer.Model
{
   public sealed class PersonInfo : IDataErrorInfo
   {
      private PersonInfo()
      {
      }

      public static PersonInfo CreateNew()
      {
         return new PersonInfo
                   {
                      Id = NullValues.Id,
                      PassportIssueDate = NullValues.DateTime,
                      PhoneNumbers = new List<PhoneNumberInfo>()
                   };
      }

      public int Id { get; set; }

      // ФИО.
      public string PersonName { get; set; }

      private string validatePersonName()
      {
         return string.IsNullOrEmpty(PersonName) ? Resources.FieldMustBeFilled : null;
      }

      // Адрес по прописке.
      public string RegistrationAddress { get; set; }

      private string validateRegistrationAddress()
      {
         return string.IsNullOrEmpty(RegistrationAddress) ? Resources.FieldMustBeFilled : null;
      }

      // Фактический адрес проживания.
      public string FactAddress { get; set; }

      private string validateFactAddress()
      {
         return string.IsNullOrEmpty(FactAddress) ? Resources.FieldMustBeFilled : null;
      }

      // Номер паспорта.
      public string PassportNumber { get; set; }

      private string validatePassportNumber()
      {
         return string.IsNullOrEmpty(PassportNumber) ? Resources.FieldMustBeFilled : null;
      }

      // Дата выдачи паспорта.
      public DateTime PassportIssueDate { get; set; }

      private string validatePassportIssueDate()
      {
         return PassportIssueDate <= NullValues.DateTime ? Resources.IncorrectValue : null;
      }

      // Орган, выдавший паспорт.
      public string PassportIssuer { get; set; }

      private string validatePassportIssuer()
      {
         return string.IsNullOrEmpty(PassportIssuer) ? Resources.FieldMustBeFilled : null;
      }
      
      // Номера телефонов.
      public List<PhoneNumberInfo> PhoneNumbers { get; private set; }

      private string validatePhoneNumbers()
      {
         return PhoneNumbers.Count == 0 ? Resources.AtLeastOnePhoneNumberMustBeSpecified : null;
      }

      public bool CanSave()
      {
         return validatePersonName() == null;
      }

      string IDataErrorInfo.this[string columnName]
      {
         get
         {
            switch (columnName)
            {
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

            return null;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }
   }
}