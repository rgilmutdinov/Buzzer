using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Buzzer.ViewModel.Common;
using Common;
using DataAccess.Model;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PersonInfoViewModel : ViewModelBase, IDataErrorInfo
   {
      private ICommand _addPhoneNumberCommand;
      private ICommand _removePhoneNumberCommand;
      private ICommand _copyRegistrationAddressCommand;

      public PersonInfoViewModel(PersonInfo original)
      {
         Check.NotNull(original, "original");

         Original = original;

         PhoneNumbers =
            new ObservableCollection<PhoneNumberViewModel>(
               Original.PhoneNumbers.Select(item => new PhoneNumberViewModel(item))
               );
      }

      #region Fields

      public PersonInfo Original { get; private set; }

      public string PersonName
      {
         get { return Original.PersonName; }
         set
         {
            if (Original.PersonName == value)
               return;

            Original.PersonName = value;
            propertyChanged("PersonName");
         }
      }

      public string RegistrationAddress
      {
         get { return Original.RegistrationAddress; }
         set
         {
            if (Original.RegistrationAddress == value)
               return;

            Original.RegistrationAddress = value;
            propertyChanged("RegistrationAddress");
         }
      }

      public string FactAddress
      {
         get { return Original.FactAddress; }
         set
         {
            if (Original.FactAddress == value)
               return;

            Original.FactAddress = value;
            propertyChanged("FactAddress");
         }
      }

      public string PassportNumber
      {
         get { return Original.PassportNumber; }
         set
         {
            if (Original.PassportNumber == value)
               return;

            Original.PassportNumber = value;
            propertyChanged("PassportNumber");
         }
      }

      public DateTime PassportIssueDate
      {
         get { return Original.PassportIssueDate; }
         set
         {
            if (Original.PassportIssueDate == value)
               return;

            Original.PassportIssueDate = value;
            propertyChanged("PassportIssueDate");
         }
      }

      public string PassportIssuer
      {
         get { return Original.PassportIssuer; }
         set
         {
            if (Original.PassportIssuer == value)
               return;

            Original.PassportIssuer = value;
            propertyChanged("PassportIssuer");
         }
      }

      public PhoneNumberViewModel SelectedPhoneNumber { get; set; }

      public ObservableCollection<PhoneNumberViewModel> PhoneNumbers { get; private set; }

      #endregion

      #region Commands

      public ICommand AddPhoneNumber
      {
         get
         {
            if (_addPhoneNumberCommand != null)
               return _addPhoneNumberCommand;

            _addPhoneNumberCommand = new CommandDelegate(addPhoneNumber);
            return _addPhoneNumberCommand;
         }
      }

      public ICommand RemovePhoneNumber
      {
         get
         {
            if (_removePhoneNumberCommand != null)
               return _removePhoneNumberCommand;

            _removePhoneNumberCommand = new CommandDelegate(removePhoneNumber, canRemovePhoneNumber);
            return _removePhoneNumberCommand;
         }
      }

      public ICommand CopyRegistrationAddress
      {
         get
         {
            if (_copyRegistrationAddressCommand != null)
               return _copyRegistrationAddressCommand;

            _copyRegistrationAddressCommand = new CommandDelegate(copyRegistrationAddress);
            return _copyRegistrationAddressCommand;
         }
      }

      #endregion
      
      #region IDataErrorInfo Members

      string IDataErrorInfo.this[string columnName]
      {
         get
         {
            string error = null;

            switch (columnName)
            {
               case "PersonName":
               case "RegistrationAddress":
               case "FactAddress":
               case "PassportNumber":
               case "PassportIssueDate":
               case "PassportIssuer":
               case "PhoneNumbers":
                  error = (Original as IDataErrorInfo)[columnName];
                  break;
            }

            CommandManager.InvalidateRequerySuggested();

            return error;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }

      #endregion

      private void addPhoneNumber()
      {
         var phoneNumberInfo = PhoneNumberInfo.CreateNew();
         Original.PhoneNumbers.Add(phoneNumberInfo);
         PhoneNumbers.Add(new PhoneNumberViewModel(phoneNumberInfo));

         if (PhoneNumbers.Count == 1)
            propertyChanged("PhoneNumbers");
      }

      private void removePhoneNumber()
      {
         var original = SelectedPhoneNumber.Original;
         PhoneNumbers.Remove(SelectedPhoneNumber);
         Original.PhoneNumbers.Remove(original);

         if (PhoneNumbers.Count == 0)
            propertyChanged("PhoneNumbers");
      }

      private bool canRemovePhoneNumber()
      {
         return SelectedPhoneNumber != null;
      }

      private void copyRegistrationAddress()
      {
         FactAddress = RegistrationAddress;
      }
   }
}