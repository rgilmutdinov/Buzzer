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
         DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

         using (DbConnection connection = factory.CreateConnection())
         {
            connection.ConnectionString = TestSettings.ConnectionString;
            connection.Open();

            execute(connection, Resources.ClearDatabase);
            execute(connection, Resources.GenerateTestDataForEditTest);
            execute(connection, Resources.GenerateTestDataForSelectTest);
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