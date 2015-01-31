using System.Data.Common;
using Buzzer.Tests.Properties;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [SetUpFixture]
   public class PrepareTestDatabase
   {
      [SetUp]
      public void SetUp()
      {
         convertDatabase();
         generateTestData();
      }

      private static void convertDatabase()
      {
         var converter = new DatabaseConverter.DatabaseConverter(TestSettings.ConnectionString);
         converter.Convert();
      }

      private static void generateTestData()
      {
         DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

         using (DbConnection connection = factory.CreateConnection())
         {
            connection.ConnectionString = TestSettings.ConnectionString;
            connection.Open();

            execute(connection, Resources.ClearDatabase);
            execute(connection, Resources.GenerateTestDataForSaveCreditsTest);
            execute(connection, Resources.GenerateTestDataForSelectCreditsTest);
            execute(connection, Resources.GenerateTestDataForSaveNotificationLogItemsTest);
            execute(connection, Resources.GenerateTestDataForSelectNotificationLogItemsTest);
            execute(connection, Resources.GenerateTestDataForCheckUserTest);
            execute(connection, Resources.GenerateTestDataForSaveNotifiedTodoItemTest);
         }
      }

      [TearDown]
      public void TearDown()
      {
         DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

         using (DbConnection connection = factory.CreateConnection())
         {
            connection.ConnectionString = TestSettings.ConnectionString;
            connection.Open();
            execute(connection, Resources.ClearDatabase);
         }
      }

      private static void execute(DbConnection connection, string commandText)
      {
         using (DbTransaction transaction = connection.BeginTransaction())
         {
            using (DbCommand command = connection.CreateCommand())
            {
               command.CommandText = commandText;
               command.Transaction = transaction;
               command.ExecuteNonQuery();
               transaction.Commit();
            }
         }
      }
   }
}