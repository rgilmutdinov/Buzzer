using System;
using System.Globalization;
using System.Windows.Data;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof (TodoItemStateConverter), typeof (bool))]
   internal sealed class TodoItemStateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var state = (TodoItemState) value;
         return state == TodoItemState.Done;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var isDone = (bool) value;
         return isDone ? TodoItemState.Done : TodoItemState.None;
      }
   }
}