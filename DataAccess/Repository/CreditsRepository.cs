using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Common;
using DataAccess.Helpers;
using DataAccess.Model;

namespace DataAccess.Repository
{
   internal sealed class CreditsRepository : RepositoryBase<CreditInfo>
   {
      private static readonly FieldInfo CreditNumber = new FieldInfo("CreditNumber", SqlDbType.NVarChar);
      private static readonly FieldInfo CreditAmount = new FieldInfo("CreditAmount", SqlDbType.Decimal, true);
      private static readonly FieldInfo CreditIssueDate = new FieldInfo("CreditIssueDate", SqlDbType.Date, true);
      private static readonly FieldInfo MonthsCount = new FieldInfo("MonthsCount", SqlDbType.Int, true);
      private static readonly FieldInfo DiscountRate = new FieldInfo("DiscountRate", SqlDbType.Decimal, true);
      private static readonly FieldInfo EffectiveDiscountRate = new FieldInfo("EffectiveDiscountRate", SqlDbType.Decimal, true);
      private static readonly FieldInfo ExchangeRate = new FieldInfo("ExchangeRate", SqlDbType.Decimal, true);

      internal CreditsRepository(string connectionString) : base(connectionString)
      {
      }

      protected override CreditInfo[] query(string whereClause, SqlConnection connection)
      {
         var selectCreditsQuery = string.Format("SELECT * FROM Credits {0}", whereClause);

         using (var command = connection.CreateCommand())
         {
            command.CommandText = selectCreditsQuery;

            using (var reader = command.ExecuteReader())
            {
               var result = new List<CreditInfo>();

               while (reader.Read())
               {
                  result.Add(
                     CreditInfo.Create(
                           Convert.ToInt32(reader[Id.Name]),
                           Convert.ToString(reader[CreditNumber.Name]),
                           Convert.ToDecimal(reader[CreditAmount.Name]),
                           Convert.ToDateTime(reader[CreditIssueDate.Name]),
                           Convert.ToInt32(reader[MonthsCount.Name]),
                           Convert.ToDecimal(reader[DiscountRate.Name]),
                           get(reader[EffectiveDiscountRate.Name], Convert.ToDecimal),
                           get(reader[ExchangeRate.Name], Convert.ToDecimal)
                        )
                     );
               }

               return result.ToArray();
            }
         }
      }

      protected override void insert(CreditInfo creditInfo, SqlConnection connection)
      {
         insertCredit(creditInfo, connection);
      }

      protected override void update(CreditInfo creditInfo, SqlConnection connection)
      {
         updateCredit(creditInfo, connection);
      }

      protected override void delete(CreditInfo creditInfo, SqlConnection connection)
      {
         deleteCredit(creditInfo.Id, connection);
      }

      private static void insertCredit(CreditInfo creditInfo, SqlConnection connection)
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

      private static void updateCredit(CreditInfo creditInfo, SqlConnection connection)
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

      private static void deleteCredit(int creditId, SqlConnection connection)
      {
         var deleteCreditQuery =
              string.Format(
                 "DELETE FROM Credits WHERE {0}={1};",
                 Id.Name, Id.ParameterName
                 );

         using (var command = connection.CreateCommand())
         {
            command.CommandText = deleteCreditQuery;
            command.AddParameter(creditId, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}
