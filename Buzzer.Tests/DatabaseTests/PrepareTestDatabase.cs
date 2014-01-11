using System.Data.SqlClient;
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
            execute(connection, clearDatabase());
            execute(connection, generateTestDataForEditTest());
            execute(connection, generateTestDataForSelectTest());
         }
      }

      [TearDown]
      public void TearDown()
      {
         using (var connection = new SqlConnection(TestSettings.ConnectionString))
         {
            connection.Open();
            execute(connection, clearDatabase());
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

      private static string clearDatabase()
      {
         return "DELETE FROM PhoneNumbers;" +
                "DELETE FROM Persons;" +
                "DELETE FROM Credits;";
      }

      private static string generateTestDataForEditTest()
      {
         return
            @"
DECLARE @ID AS BIGINT;

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CN1', 100000, '2013-12-31', 12, 0.36, 0.24, 45);
	
SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '11111111111111', 'Borrower', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	(@ID, '22222222222222', 'Guarantor 1', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	(@ID, '33333333333333', 'Guarantor 2', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	(@ID, '111111111'),
	(@ID, '222222222');
";
      }

      private static string generateTestDataForSelectTest()
      {
         return
            @"
DECLARE @ID AS BIGINT;

-- Credit without Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CN2', 200000, '2014-01-02', 24, 0.36, 0.12, 47.5);
	
SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '01234567890123', 'Borrower of CN2', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

-- Credit with Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CN3', 300000, '2014-01-02', 24, 0.36, NULL, NULL);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '12345678901234', 'Borrower of CN3', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	(@ID, '23456789012345', 'Guarantor 1 of CN3', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	(@ID, '34567890123456', 'Guarantor 2 of CN3', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	(@ID, '555666666'),
	(@ID, '777888888');
";
      }
   }
}