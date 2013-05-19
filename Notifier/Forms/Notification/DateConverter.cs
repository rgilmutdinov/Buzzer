using System;
using System.Globalization;
using System.Windows.Data;

namespace Notifier.Forms.Notification
{
   [ValueConversion(typeof (DateTime), typeof (String))]
   internal sealed class DateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var date = (DateTime) value;
         return date.ToString("dd/MM/yyyy");
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}