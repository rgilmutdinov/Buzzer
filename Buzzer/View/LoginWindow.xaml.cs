using System.Windows;
using System.Windows.Input;
using Buzzer.ViewModel.MainWindow;

namespace Buzzer.View
{
   public partial class LoginWindow : Window
   {
      private readonly LoginViewModel _loginViewModel;

      public LoginWindow(LoginViewModel loginViewModel)
      {
         InitializeComponent();

         _loginViewModel = loginViewModel;
         DataContext = _loginViewModel;

         _passwordBox.Focus();
      }

      private void passwordBoxKeyUp(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Enter)
         {
            bool result = _loginViewModel.CheckPassword(_passwordBox.Password);
            
            if (result)
            {
               DialogResult = true;
               Close();
            }
         }
         else if (e.Key == Key.Escape)
         {
            DialogResult = false;
            Close();
         }
      }
   }
}
