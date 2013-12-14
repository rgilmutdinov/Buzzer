using System;
using System.Data.SqlClient;
using DataAccess.Model;
using DataAccess.Repository;
using NUnit.Framework;

namespace Tests.DatabaseTests
{
   [TestFixture]
   public sealed class DatabaseTests
   {
      private string _connectionString;

      [TestFixtureSetUp]
      public void SetUpTests()
      {
         _connectionString = "Server=localhost;Database=TestBuzzerDatabase;Trusted_Connection=True;";

         using (var connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
               command.CommandText =
                  "DELETE FROM PhoneNumbers;" +
                  "DELETE FROM Persons;" +
                  "DELETE FROM Credits;";
               command.ExecuteNonQuery();
            }
         }
      }

      [Test]
      public void InsertContractTest()
      {
         // Arrange.
         var database = new BuzzerDatabase(_connectionString);
         var credit = CreditInfo.CreatNew();

         credit.CreditNumber = "1";
         credit.CreditAmount = 2.0M;
         credit.CreditIssueDate = DateTime.Today;
         credit.MonthsCount = 3;
         credit.DiscountRate = 36.1234M;
         credit.EffectiveDiscountRate = 5;
         credit.ExchangeRate = null;

         credit.Borrower.PersonName = "Borrower";

         // Act.
         database.SaveCredit(credit);

         // Assert.
      }
   }
}
