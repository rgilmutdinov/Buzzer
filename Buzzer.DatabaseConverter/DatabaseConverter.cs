using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Buzzer.DatabaseConverter.Converters;
using Common;

namespace Buzzer.DatabaseConverter
{
   public sealed class DatabaseConverter
   {
      private readonly string _connectionString;
      private readonly DbProviderFactory _factory;
      
      public DatabaseConverter(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");

         _connectionString = connectionString;
         _factory = DbProviderFactories.GetFactory("System.Data.SQLite");
      }

      public void Convert()
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = connection.BeginTransaction())
            {
               var commandFactory = new CommandFactory(connection, transaction);
               int version = getDatabaseVersion(commandFactory);
               KeyValuePair<int, ConverterBase>[] converters = getConverters(version, commandFactory);

               if (converters.Length > 0)
               {
                  int newVersion = runConveters(converters);
                  updateDatabaseVersion(newVersion, commandFactory);
               }

               transaction.Commit();
            }
         }
      }

      private DbConnection createConnection()
      {
         DbConnection connection = _factory.CreateConnection();
         connection.ConnectionString = _connectionString;
         connection.Open();
         return connection;
      }

      private int getDatabaseVersion(CommandFactory commandFactory)
      {
         var versionProvider = new DatabaseVersionProvider(commandFactory);
         return versionProvider.GetVersion();
      }

      private KeyValuePair<int, ConverterBase>[] getConverters(int version, CommandFactory commandFactory)
      {
         var converters =
            new Dictionary<int, Func<ConverterBase>>
               {
                  {2, () => new AddNotificationLogConverter(commandFactory)},
                  {3, () => new AddCreditStateColumnToCreditsTableConverter(commandFactory)},
                  {4, () => new AddUsersTableConverter(commandFactory)},
                  {5, () => new AllowCreditsAndPersonsTablesAcceptNullValues(commandFactory)},
                  {6, () => new AddApplicationDateAndProtocolDateColumnsConverter(commandFactory)},
                  {7, () => new AddRefusalReasonColumnToCreditsTableConverter(commandFactory)},
                  {8, () => new AddRowStateColumnToCreditsTableConverter(commandFactory)},
                  {9, () => new AlterNotificationLogViewToFilterDeletedCreditsConverter(commandFactory)},
                  {10, () => new AddTodoItemsTableConverter(commandFactory)},
                  {11, () => new AddCreditTypesAndDocumentTypesTablesConverter(commandFactory)},
                  {12, () => new AddRequiredDocumentsTableConverter(commandFactory)},
                  {13, () => new AddPayoffsTableConverter(commandFactory)}
               };

         return
            converters
               .Where(item => item.Key > version)
               .Select(item => new KeyValuePair<int, ConverterBase>(item.Key, item.Value()))
               .OrderBy(item => item.Key)
               .ToArray();
      }

      private int runConveters(KeyValuePair<int, ConverterBase>[] converters)
      {
         foreach (ConverterBase converter in converters.Select(item => item.Value))
            converter.Execute();

         return converters.Last().Key;
      }

      private void updateDatabaseVersion(int version, CommandFactory commandFactory)
      {
         var versionUpdater = new DatabaseVersionUpdater(version, commandFactory);
         versionUpdater.UpdateDatabaseVersion();
      }
   }
}
