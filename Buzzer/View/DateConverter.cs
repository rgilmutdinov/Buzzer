using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Buzzer.View
{
   [ValueConversion(typeof (DateTime), typeof (string))]
   internal sealed class DateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var date = (DateTime) value;
         return date.ToString("dd/MM/yyyy");
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}