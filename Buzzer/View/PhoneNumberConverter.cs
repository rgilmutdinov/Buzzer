using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Buzzer.ViewModel.CreditContract;

namespace Buzzer.View
{
   [ValueConversion(typeof (string), typeof (string))]
   [ValueConversion(typeof (PhoneNumberViewModel), typeof (string))]
   internal sealed class PhoneNumberConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         string phoneNumber =
            value is PhoneNumberViewModel
               ? ((PhoneNumberViewModel) value).PhoneNumber
               : (string) value;

         return FormatPhoneNumber(phoneNumber);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }

      private static object FormatPhoneNumber(string phoneNumber)
      {
         if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

         if (phoneNumber.Length != 9)
            return string.Empty;

         return string.Format("({0}) {1}-{2}-{3}",
                              phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 2),
                              phoneNumber.Substring(5, 2), phoneNumber.Substring(7));
      }
   }
}