using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Buzzer.View
{
   [ValueConversion(typeof (DateTime), typeof (string))]
   internal sealed class DateTimeConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var date = (DateTime) value;
         return date.ToString("dd/MM/yyyy HH:mm");
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}