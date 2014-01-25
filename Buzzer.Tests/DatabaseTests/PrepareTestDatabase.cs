using System.Data.SqlClient;
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
         using (var connection = new SqlConnection(TestSettings.ConnectionString))
         {
            connection.Open();
            execute(connection, Resources.ClearDatabase);
            execute(connection, Resources.GenerateTestDataForEditTest);
            execute(connection, Resources.GenerateTestDataForSelectTest);
         }
      }

      [TearDown]
      public void TearDown()
      {
         using (var connection = new SqlConnection(TestSettings.ConnectionString))
         {
            connection.Open();
            execute(connection, Resources.ClearDatabase);
         }
      }

      private static void execute(SqlConnection connection, string commandText)
      {
         using (var transaction = connection.BeginTransaction())
         {
            using (var command = new SqlCommand(commandText, connection, transaction))
            {
               command.ExecuteNonQuery();
               transaction.Commit();
            }
         }
      }
   }
}