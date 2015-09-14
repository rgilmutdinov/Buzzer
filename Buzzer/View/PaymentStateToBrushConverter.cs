using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   [ValueConversion(typeof(PaymentAdvanceState), typeof(Brush))]
   internal sealed class PaymentStateToBrushConverter : IValueConverter
   {
      private static readonly Brush PastPaidBrush   = new SolidColorBrush(Color.FromArgb(100, 144, 238, 144));
      private static readonly Brush PastUnpaidBrush = new SolidColorBrush(Color.FromArgb(100, 221, 160, 221));

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var paymentState = (PaymentAdvanceState) value;

         switch (paymentState)
         {
            case PaymentAdvanceState.PastPaid:
               return PastPaidBrush;

            case PaymentAdvanceState.PastUnpaid:
               return PastUnpaidBrush;

            case PaymentAdvanceState.Current:
               return Brushes.SkyBlue;

            case PaymentAdvanceState.Oncoming:
               return Brushes.White;

            default:
               return Brushes.Transparent;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
