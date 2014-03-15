using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof(CreditState), typeof(Brush))]
   internal sealed class CreditStateToBrushConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var creditState = (CreditState) value;

         switch (creditState)
         {
            case CreditState.Current:
               return Brushes.GreenYellow;

            case CreditState.Repayed:
               return Brushes.Pink;

            default:
               throw new ArgumentException();
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}