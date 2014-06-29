using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof(CreditState), typeof(string))]
   internal sealed class CreditStateToStringConverterForRegistrationLog : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var creditState = (CreditState) value;

         switch (creditState)
         {
            case CreditState.Current:
            case CreditState.Repayed:
               return "¬€ƒ¿Õ";

            case CreditState.Consideration:
               return "–¿——ÃŒ“–≈Õ»≈ «¿ﬂ¬À≈Õ»ﬂ";

            case CreditState.Refused:
               return "Œ“ ¿«¿Õ";

            default:
               return "Õ≈¬≈–Õ€… —“¿“”—";
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}