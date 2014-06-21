using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Buzzer.View
{
   [ValueConversion(typeof (DateTime), typeof (string))]
   [ValueConversion(typeof (DateTime?), typeof (string))]
   internal sealed class DateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value == null)
            return string.Empty;

         DateTime date = value is DateTime ? (DateTime) value : ((DateTime?) value).Value;
         return date.ToString("dd/MM/yyyy");
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}