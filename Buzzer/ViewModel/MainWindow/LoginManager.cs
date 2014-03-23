using Buzzer.DataAccess.Repository;
using Buzzer.View;
using Common;

namespace Buzzer.ViewModel.MainWindow
{
   internal sealed class LoginManager
   {
      private readonly BuzzerDatabase _database;
      private bool _isLoginNeeded;

      public LoginManager(BuzzerDatabase database)
      {
         Check.NotNull(database, "database");
         _database = database;
         _isLoginNeeded = true;
      }

      public bool Login()
      {
         if (!_isLoginNeeded)
            return true;

         var loginWindow = new LoginWindow(new LoginViewModel(_database));
         bool? result = loginWindow.ShowDialog();
         bool isSuccess = result.HasValue && result.Value;

         if (isSuccess)
            _isLoginNeeded = false;

         return isSuccess;
      }
   }
}