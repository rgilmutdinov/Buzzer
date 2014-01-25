using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Buzzer.DataAccess.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   [Obsolete("Delete", true)]
   internal sealed class PhoneNumbersRepository : RepositoryBase<PhoneNumberInfo>
   {
      internal static readonly FieldInfo PersonId = new FieldInfo("PersonID", SqlDbType.Int);
      private static readonly FieldInfo PhoneNumber = new FieldInfo("PhoneNumber", SqlDbType.NVarChar, true);

      public PhoneNumbersRepository(string connectionString) : base(connectionString)
      {
      }

      protected override PhoneNumberInfo[] query(string whereClause, SqlConnection connection)
      {
         var selectPhoneNumbersQuery = string.Format("SELECT * FROM PhoneNumbers {0}", whereClause);

         using (var command = connection.CreateCommand())
         {
            command.CommandText = selectPhoneNumbersQuery;

            using (var reader = command.ExecuteReader())
            {
               var result = new List<PhoneNumberInfo>();

               while (reader.Read())
               {
                  result.Add(
                     PhoneNumberInfo.Create(
                        Convert.ToInt32(reader[Id.Name]),
                        Convert.ToInt32(reader[PersonId.Name]),
                        Convert.ToString(reader[PhoneNumber.Name])
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      protected override void insert(PhoneNumberInfo phoneNumberInfo, SqlConnection connection)
      {
         insertPhoneNumber(phoneNumberInfo, connection);
      }

      protected override void update(PhoneNumberInfo phoneNumberInfo, SqlConnection connection)
      {
         updatePhoneNumber(phoneNumberInfo, connection);
      }

      protected override void delete(PhoneNumberInfo item, SqlConnection connection)
      {
         deletePhoneNumber(item.Id, connection);
      }

      private static void insertPhoneNumber(PhoneNumberInfo phoneNumber, SqlConnection connection)
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
            command.CommandText = insertPhoneNumberQuery;
            command.AddParameter(phoneNumber.PersonId, PersonId);
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            phoneNumber.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private static void updatePhoneNumber(PhoneNumberInfo phoneNumber, SqlConnection connection)
      {
         var updatePhoneNumberQuery =
            string.Format(
               "UPDATE PhoneNumbers SET {0}={1} WHERE {2}={3};",
               PhoneNumber.Name, PhoneNumber.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = updatePhoneNumberQuery;
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            command.AddParameter(phoneNumber.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      private static void deletePhoneNumber(int phoneNumberId, SqlConnection connection)
      {
         var deletePhoneNumberQuery =
            string.Format(
               "DELETE FROM PhoneNumbers WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = deletePhoneNumberQuery;
            command.AddParameter(phoneNumberId, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}