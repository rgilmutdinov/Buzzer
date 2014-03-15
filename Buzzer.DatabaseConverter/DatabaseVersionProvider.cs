using System;
using System.Data.Common;
using Common;

namespace Buzzer.DatabaseConverter
{
   internal sealed class DatabaseVersionProvider
   {
      private readonly CommandFactory _commandFactory;

      public DatabaseVersionProvider(CommandFactory commandFactory)
      {
         Check.NotNull(commandFactory, "commandFactory");
         _commandFactory = commandFactory;
      }

      public int GetVersion()
      {
         createVersionTableIfNotExists();
         return readDatabaseVersion();
      }

      private void createVersionTableIfNotExists()
      {
         const string query =
            "CREATE TABLE IF NOT EXISTS DatabaseVersion (Version INTEGER);";

         using (DbCommand command = _commandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }

      private int readDatabaseVersion()
      {
         const int noVersion = 0;
         const int firstVersion = 1;

         const string selectVersionQuery =
            "SELECT Version FROM DatabaseVersion LIMIT 1";
         
         int version;

         using (DbCommand command = _commandFactory.CreateCommand(selectVersionQuery))
         {
            object value = command.ExecuteScalar();
            version = value == null ? noVersion : Convert.ToInt32(value);
         }

         if (version == noVersion)
         {
            string insertFirstVersionQuery =
               string.Format("INSERT INTO DatabaseVersion VALUES ({0})", firstVersion);

            using (DbCommand command = _commandFactory.CreateCommand(insertFirstVersionQuery))
               command.ExecuteNonQuery();

            version = firstVersion;
         }

         return version;
      }
   }
}