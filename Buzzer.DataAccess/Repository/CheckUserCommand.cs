using System;
using System.Data;
using System.Data.Common;
using Buzzer.DataAccess.Common;
using Buzzer.DataAccess.Helpers;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class CheckUserCommand : CommandBase
   {
      protected static readonly FieldInfo Login = new FieldInfo("Login", DbType.String);
      protected static readonly FieldInfo Password = new FieldInfo("Password", DbType.String);
      
      private readonly string _login;
      private readonly string _password;

      internal CheckUserCommand(DbConnection connection, DbTransaction transaction, string login, string password)
         : base(connection, transaction)
      {
         Check.NotNullAndEmpty(login, "login");
         Check.NotNullAndEmpty(password, "password");

         _login = login;
         _password = password;
      }

      public bool Execute()
      {
         string query =
            string.Format(
               "SELECT COUNT(*) FROM Users WHERE {0} = {1} AND {2} = {3}",
               Login.Name, Login.ParameterName, Password.Name, Password.ParameterName
               );

         using (DbCommand command = createCommand(query))
         {
            command.AddParameter(_login, Login);
            command.AddParameter(_password, Password);

            return Convert.ToInt32(command.ExecuteScalar()) == 1;
         }
      }
   }
}