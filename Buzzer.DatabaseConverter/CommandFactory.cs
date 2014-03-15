using System.Data.Common;
using Common;

namespace Buzzer.DatabaseConverter
{
   internal sealed class CommandFactory
   {
      private readonly DbConnection _connection;
      private readonly DbTransaction _transaction;

      public CommandFactory(DbConnection connection, DbTransaction transaction)
      {
         Check.NotNull(connection, "connection");
         Check.NotNull(transaction, "transaction");

         _connection = connection;
         _transaction = transaction;
      }

      public DbCommand CreateCommand(string query)
      {
         DbCommand command = _connection.CreateCommand();
         command.Transaction = _transaction;
         command.CommandText = query;
         return command;
      }
   }
}