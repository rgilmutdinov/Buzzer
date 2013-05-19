using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Notifier.Forms.Notification
{
   [ValueConversion(typeof (bool), typeof (Brush))]
   internal sealed class BooleanToBrushConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var flag = (bool) value;
         return flag ? Brushes.GreenYellow : Brushes.Pink;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}