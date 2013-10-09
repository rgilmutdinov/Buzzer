using System.ComponentModel;
using Buzzer.ViewModel.Common;
using Common;
using DataAccess.Model;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PhoneNumberViewModel : ViewModelBase, IDataErrorInfo
   {
      public PhoneNumberViewModel(PhoneNumberInfo phoneNumber)
      {
         Check.NotNull(phoneNumber, "phoneNumber");
         Original = phoneNumber;
      }

      public PhoneNumberInfo Original { get; private set; }

      public string PhoneNumber
      {
         get { return Original.PhoneNumber; }
         set
         {
            if (Original.PhoneNumber == value)
               return;

            Original.PhoneNumber = value;
            propertyChanged("PhoneNumber");
         }
      }

      public string this[string columnName]
      {
         get
         {
            if (columnName == "PhoneNumber")
               return (Original as IDataErrorInfo)[columnName];

            return null;
         }
      }

      public string Error
      {
         get { return null; }
      }
   }
}