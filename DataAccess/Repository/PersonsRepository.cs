using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Common;
using DataAccess.Helpers;
using DataAccess.Model;

namespace DataAccess.Repository
{
   internal sealed class PersonsRepository : RepositoryBase<PersonInfo>
   {
      internal static readonly FieldInfo CreditId = new FieldInfo("CreditId", SqlDbType.Int);
      private static readonly FieldInfo PersonalNumber = new FieldInfo("PersonalNumber", SqlDbType.NVarChar);
      private static readonly FieldInfo Name = new FieldInfo("Name", SqlDbType.NVarChar);
      private static readonly FieldInfo RegistrationAddress = new FieldInfo("RegistrationAddress", SqlDbType.NVarChar);
      private static readonly FieldInfo FactAddress = new FieldInfo("FactAddress", SqlDbType.NVarChar);
      private static readonly FieldInfo PassportNumber = new FieldInfo("PassportNumber", SqlDbType.NVarChar);
      private static readonly FieldInfo PassportIssueDate = new FieldInfo("PassportIssueDate", SqlDbType.Date);
      private static readonly FieldInfo PassportIssuer = new FieldInfo("PassportIssuer", SqlDbType.NVarChar);
      private static readonly FieldInfo IsBorrower = new FieldInfo("IsBorrower", SqlDbType.Bit);

      public PersonsRepository(string connectionString) : base(connectionString)
      {
      }

      protected override PersonInfo[] query(string whereClause, SqlConnection connection)
      {
         var selectPhoneNumbersQuery = string.Format("SELECT * FROM Persons {0}", whereClause);

         using (var command = connection.CreateCommand())
         {
            command.CommandText = selectPhoneNumbersQuery;

            using (var reader = command.ExecuteReader())
            {
               var result = new List<PersonInfo>();

               while (reader.Read())
               {
                  result.Add(
                     PersonInfo.Create(
                        Convert.ToInt32(reader[Id.Name]),
                        Convert.ToInt32(reader[CreditId.Name]),
                        Convert.ToString(reader[PersonalNumber.Name]),
                        Convert.ToString(reader[Name.Name]),
                        Convert.ToString(reader[RegistrationAddress.Name]),
                        Convert.ToString(reader[FactAddress.Name]),
                        Convert.ToString(reader[PassportNumber.Name]),
                        Convert.ToDateTime(reader[PassportIssueDate.Name]),
                        Convert.ToString(reader[PassportIssuer.Name]),
                        Convert.ToBoolean(reader[IsBorrower.Name])
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      protected override void insert(PersonInfo personInfo, SqlConnection connection)
      {
         insertPerson(personInfo, connection);
      }

      protected override void update(PersonInfo personInfo, SqlConnection connection)
      {
         updatePerson(personInfo, connection);
      }

      protected override void delete(PersonInfo personInfo, SqlConnection connection)
      {
         deletePerson(personInfo.Id, connection);
      }

      private static void insertPerson(PersonInfo personInfo, SqlConnection connection)
      {
         var insertPersonQuery =
            string.Format(
               "INSERT INTO Persons ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) VALUES ({9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17});" +
               "SELECT SCOPE_IDENTITY();",

               CreditId.Name, PersonalNumber.Name, Name.Name, RegistrationAddress.Name, FactAddress.Name,
               PassportNumber.Name, PassportIssueDate.Name, PassportIssuer.Name, IsBorrower.Name,

               CreditId.ParameterName, PersonalNumber.ParameterName, Name.ParameterName, RegistrationAddress.ParameterName, FactAddress.ParameterName,
               PassportNumber.ParameterName, PassportIssueDate.ParameterName, PassportIssuer.ParameterName, IsBorrower.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = insertPersonQuery;
            command.AddParameter(personInfo.CreditId, CreditId);
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, IsBorrower);
            personInfo.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private static void updatePerson(PersonInfo personInfo, SqlConnection connection)
      {
         var updatePersonQuery =
            string.Format(
               "UPDATE Persons SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13}, {14}={15} WHERE {16}={17};",
               PersonalNumber.Name, PersonalNumber.ParameterName,
               Name.Name, Name.ParameterName,
               RegistrationAddress.Name, RegistrationAddress.ParameterName,
               FactAddress.Name, FactAddress.ParameterName,
               PassportNumber.Name, PassportNumber.ParameterName,
               PassportIssueDate.Name, PassportIssueDate.ParameterName,
               PassportIssuer.Name, PassportIssuer.ParameterName,
               IsBorrower.Name, IsBorrower.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = updatePersonQuery;
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, IsBorrower);
            command.AddParameter(personInfo.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      private static void deletePerson(int personId, SqlConnection connection)
      {
         var deletePersonQuery =
            string.Format(
               "DELETE FROM Persons WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = deletePersonQuery;
            command.AddParameter(personId, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}