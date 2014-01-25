using System.Windows;
using System.Windows.Controls;
using Buzzer.DomainModel.Models;

namespace Buzzer.View
{
   public partial class PersonInfoView : UserControl
   {
      public static readonly DependencyProperty PersonInfoProperty =
         DependencyProperty.Register("PersonInfo", typeof (PersonInfo), typeof (PersonInfoView));
      
      public PersonInfoView()
      {
         InitializeComponent();
      }

      public PersonInfo PersonInfo
      {
         get { return (PersonInfo) GetValue(PersonInfoProperty); }
         set { SetValue(PersonInfoProperty, value); }
      }
   }
}
