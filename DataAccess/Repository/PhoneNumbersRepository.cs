using System;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Helpers;
using DataAccess.Model;

namespace DataAccess.Repository
{
   public sealed class PhoneNumbersRepository : RepositoryBase<PhoneNumberInfo>
   {
      private static readonly FieldInfo Id = new FieldInfo("ID", SqlDbType.Int);
      private static readonly FieldInfo PersonId = new FieldInfo("PersonID", SqlDbType.Int);
      private static readonly FieldInfo PhoneNumber = new FieldInfo("PhoneNumber", SqlDbType.NVarChar, true);

      public PhoneNumbersRepository(string connectionString) : base(connectionString)
      {
      }

      protected override void save(PhoneNumberInfo phoneNumberInfo, SqlConnection connection, SqlTransaction transaction)
      {
         if (phoneNumberInfo.IsNew)
            insertPhoneNumber(phoneNumberInfo, connection, transaction);
         else
            updatePhoneNumber(phoneNumberInfo, connection, transaction);
      }

      protected override void delete(PhoneNumberInfo item, SqlConnection connection, SqlTransaction transaction)
      {
         throw new NotImplementedException();
      }

      private static void insertPhoneNumber(
         PhoneNumberInfo phoneNumber,
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
            command.AddParameter(phoneNumber.PersonId, PersonId);
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            phoneNumber.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private static void updatePhoneNumber(
         PhoneNumberInfo phoneNumber,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var updatePhoneNumberQuery =
            string.Format(
               "UPDATE PhoneNumbers SET {0}={1} WHERE {2}={3};",
               PhoneNumber.Name, PhoneNumber.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = updatePhoneNumberQuery;
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            command.AddParameter(phoneNumber.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      private static void deletePhoneNumber(
         int phoneNumberId,
         SqlConnection connection,
         SqlTransaction transaction
         )
      {
         var deletePhoneNumberQuery =
            string.Format(
               "DELETE FROM PhoneNumbers WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.Transaction = transaction;
            command.CommandText = deletePhoneNumberQuery;
            command.AddParameter(phoneNumberId, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}