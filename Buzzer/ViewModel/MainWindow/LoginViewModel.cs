using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Services;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.MainWindow
{
   public sealed class LoginViewModel : ViewModelBase
   {
      private readonly BuzzerDatabase _buzzerDatabase;
      private string _message;

      public LoginViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");
         _buzzerDatabase = buzzerDatabase;

         DisplayName = Resources.LoginViewModel_Caption;
      }

      public string Message
      {
         get { return _message; }
         private set
         {
            if (_message == value)
               return;

            _message = value;
            propertyChanged("Message");
         }
      }

      public bool CheckPassword(string password)
      {
         if (string.IsNullOrEmpty(password))
         {
            Message = "¬ведите пароль";
            return false;
         }

         bool result = _buzzerDatabase.CheckUser("Atai", CryptoService.GetHash(password));

         if (!result)
            Message = "¬веден неверный пароль";

         return result;
      }
   }
}