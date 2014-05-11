using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Buzzer.View
{
   [ValueConversion(typeof (string), typeof (string))]
   internal sealed class PhoneNumberConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var phoneNumber = (string) value;

         if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

         if (phoneNumber.Length != 9)
            return string.Empty;

         return string.Format("({0}) {1}-{2}-{3}",
                              phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 2),
                              phoneNumber.Substring(5, 2), phoneNumber.Substring(7));
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}