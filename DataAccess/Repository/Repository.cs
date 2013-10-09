using System;
using System.Data;
using System.Data.SqlClient;
using Common;
using DataAccess.Helpers;
using DataAccess.Model;

namespace DataAccess.Repository
{
   public sealed class Repository : RepositoryBase<CreditInfo>
   {
      #region Query fields

      // Common.
      private static readonly FieldInfo Id = new FieldInfo("ID", SqlDbType.Int);
      private static readonly FieldInfo PersonId = new FieldInfo("PersonID", SqlDbType.Int);

      // Credit.
      private static readonly FieldInfo CreditNumber = new FieldInfo("CreditNumber", SqlDbType.NVarChar);
      private static readonly FieldInfo CreditAmount = new FieldInfo("CreditAmount", SqlDbType.Decimal, true);
      private static readonly FieldInfo CreditIssueDate = new FieldInfo("CreditIssueDate", SqlDbType.Date, true);
      private static readonly FieldInfo MonthsCount = new FieldInfo("MonthsCount", SqlDbType.Int, true);
      private static readonly FieldInfo DiscountRate = new FieldInfo("DiscountRate", SqlDbType.Decimal, true);
      private static readonly FieldInfo EffectiveDiscountRate = new FieldInfo("EffectiveDiscountRate", SqlDbType.Decimal, true);
      private static readonly FieldInfo ExchangeRate = new FieldInfo("ExchangeRate", SqlDbType.Decimal, true);

      // Person.
      private static readonly FieldInfo Name = new FieldInfo("Name", SqlDbType.NVarChar);
      private static readonly FieldInfo RegistrationAddress = new FieldInfo("RegistrationAddress", SqlDbType.NVarChar, true);
      private static readonly FieldInfo FactAddress = new FieldInfo("FactAddress", SqlDbType.NVarChar, true);
      private static readonly FieldInfo PassportNumber = new FieldInfo("PassportNumber", SqlDbType.NVarChar, true);
      private static readonly FieldInfo PassportIssueDate = new FieldInfo("PassportIssueDate", SqlDbType.Date, true);
      private static readonly FieldInfo PassportIssuer = new FieldInfo("PassportIssuer", SqlDbType.NVarChar, true);
      private static readonly FieldInfo IsValid = new FieldInfo("IsValid", SqlDbType.Bit);

      // PhoneNumber.
      private static readonly FieldInfo PhoneNumber = new FieldInfo("PhoneNumber", SqlDbType.NVarChar, true);

      // PersonToCredit.
      private static readonly FieldInfo CreditId = new FieldInfo("CreditId", SqlDbType.Int);
      private static readonly FieldInfo IsBorrower = new FieldInfo("IsBorrower", SqlDbType.Bit);

      #endregion

      public Repository(string connectionString) : base(connectionString)
      {
      }

      public CreditInfo[] GetAll()
      {
         /*var query =
            string.Format(
               "SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} FROM Credits " +
               "",
               Id
               );

         */
         return null;
      }

      public void SavePerson(PersonInfo personInfo)
      {
         executeInTransaction(
            (connection, transaction) => savePerson(personInfo, connection, transaction)
            );
      }

      public void Save(CreditInfo creditInfo)
      {
         executeInTransaction(
            (connection, transaction) =>
               {
                  if (creditInfo.Id == NullValues.Id)
                     insertCredit(creditInfo, connection, transaction);
                  else
                     updateCredit(creditInfo, connection, transaction);

                  savePerson(creditInfo.Borrower, connection, transaction);
                  foreach (var guarantor in creditInfo.Guarantors)
                     savePerson(guarantor, connection, transaction);

                  insertPersonToCredit(creditInfo.Borrower.Id, creditInfo.Id, true, connection, transaction);
                  foreach (var guarantor in creditInfo.Guarantors)
                     insertPersonToCredit(guarantor.Id, creditInfo.Id, false, connection, transaction);
               }
            );
      }

      protected override void save(CreditInfo item, SqlConnection connection, SqlTransaction transaction)
      {
         throw new NotImplementedException();
      }

      protected override void delete(CreditInfo item, SqlConnection connection, SqlTransaction transaction)
      {
         throw new NotImplementedException();
      }

      #region PhoneNumber operations

      private static void insertPhoneNumber(
         PhoneNumberInfo phoneNumber,
         int personId,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var insertPhoneNumberQuery =
            string.Format(
               "INSERT INTO PhoneNumbers ({0}, {1}) VALUES ({2}, {3});" +
               "SELECT SCOPE_IDENTITY();",
               PersonId.Name, PhoneNumber.Name,
               PersonId.ParameterName, PhoneNumber.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = insertPhoneNumberQuery;
            command.AddParameter(personId, PersonId);
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            phoneNumber.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      #endregion
      
      #region Person operations

      private static void savePerson(PersonInfo personInfo, SqlConnection connection, SqlTransaction transaction)
      {
         if (personInfo.Id == NullValues.Id)
         {
            insertPerson(personInfo, connection, transaction);
            foreach (var phoneNumber in personInfo.PhoneNumbers)
               insertPhoneNumber(phoneNumber, personInfo.Id, connection, transaction);
         }
         else
         {
            updatePerson(personInfo, connection, transaction);
         }
      }

      private static void insertPerson(
         PersonInfo personInfo,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var insertPersonQuery =
            string.Format(
               "INSERT INTO Persons ({0}, {1}, {2}, {3}, {4}, {5}, {6}) VALUES ({7}, {8}, {9}, {10}, {11}, {12}, {13});" +
               "SELECT SCOPE_IDENTITY();",

               Name.Name, RegistrationAddress.Name, FactAddress.Name, PassportNumber.Name,
               PassportIssueDate.Name, PassportIssuer.Name, IsValid.Name,

               Name.ParameterName, RegistrationAddress.ParameterName, FactAddress.ParameterName, PassportNumber.ParameterName,
               PassportIssueDate.ParameterName, PassportIssuer.ParameterName, IsValid.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = insertPersonQuery;

            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(true, IsValid);
            personInfo.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private static void updatePerson(PersonInfo personInfo, SqlConnection connection, SqlTransaction transaction)
      {
         var updatePersonQuery =
            string.Format(
               "UPDATE Persons SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13} WHERE {14}={15};",
               Name.Name, Name.ParameterName,
               RegistrationAddress.Name, RegistrationAddress.ParameterName,
               FactAddress.Name, FactAddress.ParameterName,
               PassportNumber.Name, PassportNumber.ParameterName,
               PassportIssueDate.Name, PassportIssueDate.ParameterName,
               PassportIssuer.Name, PassportIssuer.ParameterName,
               IsValid.Name, IsValid.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = updatePersonQuery;

            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(true, IsValid);
            command.AddParameter(personInfo.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region Credit operations

      private static void insertCredit(
         CreditInfo creditInfo,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var insertCreditQuery =
            string.Format(
               "INSERT INTO Credits ({0}, {1}, {2}, {3}, {4}, {5}, {6}) VALUES ({7}, {8}, {9}, {10}, {11}, {12}, {13});" +
               "SELECT SCOPE_IDENTITY();",

               CreditNumber.Name, CreditAmount.Name, CreditIssueDate.Name, MonthsCount.Name,
               DiscountRate.Name, EffectiveDiscountRate.Name, ExchangeRate.Name,

               CreditNumber.ParameterName, CreditAmount.ParameterName, CreditIssueDate.ParameterName,
               MonthsCount.ParameterName, DiscountRate.ParameterName, EffectiveDiscountRate.ParameterName,
               ExchangeRate.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = insertCreditQuery;
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            creditInfo.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private static void updateCredit(
         CreditInfo creditInfo,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var updateCreditQuery =
            string.Format(
               "UPDATE Credits SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13} WHERE {14}={15};",
               CreditNumber.Name, CreditNumber.ParameterName,
               CreditAmount.Name, CreditAmount.ParameterName,
               CreditIssueDate.Name, CreditIssueDate.ParameterName,
               MonthsCount.Name, MonthsCount.ParameterName,
               DiscountRate.Name, DiscountRate.ParameterName,
               EffectiveDiscountRate.Name, EffectiveDiscountRate.ParameterName,
               ExchangeRate.Name, ExchangeRate.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = updateCreditQuery;
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            command.AddParameter(creditInfo.Id, Id);
            command.ExecuteNonQuery();
         }
      }
      
      private static void insertPersonToCredit(
         int personId,
         int creditId,
         bool isBorrower,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var insertPersonToCreditQuery =
            string.Format(
               "INSERT INTO PersonsToCredits ({0}, {1}, {2}) VALUES ({3}, {4}, {5});",
               PersonId.Name, CreditId.Name, IsBorrower.Name,
               PersonId.ParameterName, CreditId.ParameterName, IsBorrower.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = insertPersonToCreditQuery;
            command.AddParameter(personId, PersonId);
            command.AddParameter(creditId, CreditId);
            command.AddParameter(isBorrower, IsBorrower);
            command.ExecuteNonQuery();
         }
      }

      #endregion
   }
}
