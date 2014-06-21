using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof (CreditState), typeof (string))]
   internal sealed class CreditStateToStringConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var creditState = (CreditState)value;

         switch (creditState)
         {
            case CreditState.Current:
               return "ВЫДАН";

            case CreditState.Repayed:
               return "ПОГАШЕН";

            case CreditState.Consideration:
               return "РАССМОТРЕНИЕ ЗАЯВЛЕНИЯ";

            case CreditState.Refused:
               return "ОТКАЗАН";

            default:
               return "НЕВЕРНЫЙ СТАТУС";
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}