using System;
using System.Globalization;
using System.Windows.Data;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof (RequiredDocumentState), typeof (bool))]
   internal sealed class RequiredDocumentStateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var state = (RequiredDocumentState) value;
         return state == RequiredDocumentState.Carried;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var isCarried = (bool) value;
         return isCarried ? RequiredDocumentState.Carried : RequiredDocumentState.None;
      }
   }
}