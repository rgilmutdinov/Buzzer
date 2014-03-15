using System.Data.Common;
using Common;

namespace Buzzer.DatabaseConverter
{
   internal sealed class DatabaseVersionUpdater
   {
      private readonly int _version;
      private readonly CommandFactory _commandFactory;

      public DatabaseVersionUpdater(int version, CommandFactory commandFactory)
      {
         Check.NotNull(commandFactory, "commandFactory");

         _version = version;
         _commandFactory = commandFactory;
      }

      public void UpdateDatabaseVersion()
      {
         string query =
            string.Format("UPDATE DatabaseVersion SET Version = {0}", _version);

         using (DbCommand command = _commandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}