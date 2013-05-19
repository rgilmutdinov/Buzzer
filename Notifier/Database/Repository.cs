using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace Notifier.Database
{
   public sealed class Repository
   {
      private const string Id = "ID";
      private const string ContractNumber = "ContractNumber";
      private const string BorrowerName = "BorrowerName";
      private const string ExchangeRate = "ExchangeRate";
      private const string PhoneNumber = "PhoneNumber";
      private const string ContractId = "ContractID";
      private const string PaymentDate = "PaymentDate";
      private const string PaymentAmount = "PaymentAmount";
      private const string IsNotified = "IsNotified";

      private const string IdParameter = "@id";
      private const string ContractNumberParameter = "@contractNumber";
      private const string BorrowerNameParameter = "@borrowerName";
      private const string ExchangeRateParameter = "@exchangeRate";
      private const string PhoneNumberParameter = "@phoneNumber";
      private const string ContractIdParameter = "@contractID";
      private const string PaymentDateParameter = "@paymentDate";
      private const string PaymentAmountParameter = "@paymentAmount";
      private const string IsNotifiedParameter = "@isNotified";
      
      private readonly DbProviderFactory _factory;
      private readonly string _connectionString;

      public Repository()
      {
         _factory = DbProviderFactories.GetFactory("System.Data.SQLite");
         _connectionString = ConfigurationManager.ConnectionStrings["LocalDatabaseConnectionString"].ConnectionString;
      }

      public Contract[] GetContracts()
      {
         return getContracts(query => query, command => new DbParameter[0]);
      }

      public Contract GetContract(int id)
      {
         var where = string.Format(" WHERE {0} = {1}", Id, IdParameter);
         Func<DbCommand, DbParameter[]> patchParameters =
            command => new[] {createParameter(command, IdParameter, id, DbType.Int32)};
         return getContracts(query => query + where, patchParameters).FirstOrDefault();
      }

      public Payment[] GetPayments(int contractId)
      {
         var selectPaymentsQuery =
            string.Format(
               "SELECT {0}, {1}, {2}, {3} FROM Payments WHERE {4} = {5}",
               Id, PaymentDate, PaymentAmount, IsNotified, ContractId, ContractIdParameter
               );

         return
            execute(
               connection =>
                  {
                     using (var command = connection.CreateCommand())
                     {
                        command.CommandText = selectPaymentsQuery;
                        command.Parameters.Add(createParameter(command, ContractIdParameter, contractId, DbType.Int32));

                        using (var reader = command.ExecuteReader())
                        {
                           var payments = new List<Payment>();

                           while (reader.Read())
                           {
                              var id = reader.GetInt32(0);
                              var paymentDate = reader.GetDateTime(1);
                              var paymentAmount = reader.GetDecimal(2);
                              var isNotified = reader.GetBoolean(3);
                              payments.Add(new Payment(id, contractId, paymentDate, paymentAmount, isNotified));
                           }

                           return payments.ToArray();
                        }
                     }
                  }
               );
      }

      public void SaveContract(Contract contract)
      {
         var insertContractQuery =
            string.Format(
               "INSERT INTO Contracts ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",
               ContractNumber, BorrowerName, ExchangeRate, PhoneNumber,
               ContractNumberParameter, BorrowerNameParameter, ExchangeRateParameter, PhoneNumberParameter
               );

         var insertPaymentQuery =
            string.Format(
               "INSERT INTO Payments ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",
               ContractId, PaymentDate, PaymentAmount, IsNotified,
               ContractIdParameter, PaymentDateParameter, PaymentAmountParameter, IsNotifiedParameter
               );

         executeInTransaction(
            connection =>
               {
                  int id;

                  using (var command = connection.CreateCommand())
                  {
                     command.CommandText = insertContractQuery;

                     command.Parameters.Add(createParameter(command, ContractNumberParameter, contract.ContractNumber, DbType.String));
                     command.Parameters.Add(createParameter(command, BorrowerNameParameter, contract.BorrowerName, DbType.String));
                     command.Parameters.Add(createParameter(command, ExchangeRateParameter, contract.ExchangeRate, DbType.Decimal));
                     command.Parameters.Add(createParameter(command, PhoneNumberParameter, contract.PhoneNumber, DbType.String));

                     try
                     {
                        id = Convert.ToInt32(command.ExecuteScalar());
                        contract.SetId(id);
                     }
                     catch (SQLiteException e)
                     {
                        if (e.ReturnCode == SQLiteErrorCode.Constraint)
                        {
                           throw new ContractNumberIsNotUniqueException(contract.ContractNumber);
                        }

                        throw;
                     }
                  }

                  foreach (var payment in contract.Payments)
                  {
                     using (var command = connection.CreateCommand())
                     {
                        command.CommandText = insertPaymentQuery;

                        command.Parameters.Add(createParameter(command, ContractIdParameter, id, DbType.Int32));
                        command.Parameters.Add(createParameter(command, PaymentDateParameter, payment.PaymentDate, DbType.Date));
                        command.Parameters.Add(createParameter(command, PaymentAmountParameter, payment.PaymentAmount, DbType.Decimal));
                        command.Parameters.Add(createParameter(command, IsNotifiedParameter, payment.IsNotified, DbType.Boolean));

                        var paymentId = Convert.ToInt32(command.ExecuteScalar());
                        payment.SetId(paymentId);
                        payment.SetContractId(id);
                     }
                  }
               }
            );
      }

      public void UpdatePhoneNumber(int contractId, string phoneNumber)
      {
         var updateQuery =
            string.Format(
               "UPDATE Contracts SET {0} = {1} WHERE {2} = {3};",
               PhoneNumber, PhoneNumberParameter, Id, IdParameter
               );

         executeInTransaction(
            connection =>
               {
                  using (var command = connection.CreateCommand())
                  {
                     command.CommandText = updateQuery;
                     command.Parameters.Add(createParameter(command, PhoneNumberParameter, phoneNumber, DbType.String));
                     command.Parameters.Add(createParameter(command, IdParameter, contractId, DbType.Int32));
                     command.ExecuteNonQuery();
                  }
               }
            );
      }

      public void UpdateIsNotified(int paymentId, bool isNotified)
      {
         var updateQuery =
            string.Format(
               "UPDATE Payments SET {0} = {1} WHERE {2} = {3}",
               IsNotified, IsNotifiedParameter, Id, IdParameter
               );

         executeInTransaction(
            connection =>
               {
                  using (var command = connection.CreateCommand())
                  {
                     command.CommandText = updateQuery;
                     command.Parameters.Add(createParameter(command, IsNotifiedParameter, isNotified, DbType.Boolean));
                     command.Parameters.Add(createParameter(command, IdParameter, paymentId, DbType.Int32));
                     command.ExecuteNonQuery();
                  }
               }
            );
      }

      private Contract[] getContracts(
         Func<string, string> patchQuery,
         Func<DbCommand, DbParameter[]> parametersGetter)
      {
         var selectContractQuery =
            string.Format(
               "SELECT {0}, {1}, {2}, {3}, {4} FROM Contracts",
               Id, ContractNumber, BorrowerName, ExchangeRate, PhoneNumber
               );

         return
            execute(
               connection =>
                  {
                     using (var command = connection.CreateCommand())
                     {
                        command.CommandText = patchQuery(selectContractQuery);
                        command.Parameters.AddRange(parametersGetter(command));

                        using (var reader = command.ExecuteReader())
                        {
                           var result = new List<Contract>();

                           while (reader.Read())
                           {
                              var id = reader.GetInt32(0);
                              var contractNumber = reader.GetString(1);
                              var borrowerName = reader.GetString(2);
                              var exchangeRate = reader.GetDecimal(3);
                              var phoneNumber = reader[PhoneNumber] == DBNull.Value ? null : reader.GetString(4);

                              result.Add(new Contract(this, id, contractNumber, borrowerName, exchangeRate, phoneNumber));
                           }

                           return result.ToArray();
                        }
                     }
                  }
               );
      }

      private void execute(Action<DbConnection> query)
      {
         using (var connection = _factory.CreateConnection())
         {
            connection.ConnectionString = _connectionString;
            connection.Open();
            query(connection);
         }
      }

      private T execute<T>(Func<DbConnection, T> query)
      {
         using (var connection = _factory.CreateConnection())
         {
            connection.ConnectionString = _connectionString;
            connection.Open();
            return query(connection);
         }
      }

      private void executeInTransaction(Action<DbConnection> query)
      {
         execute(
            connection =>
               {
                  var transaction = connection.BeginTransaction();
                  
                  try
                  {
                     query(connection);
                     transaction.Commit();
                  }
                  catch (Exception)
                  {
                     transaction.Rollback();
                     throw;
                  }
                  finally
                  {
                     transaction.Dispose();
                  }
               }
            );
      }

      private static DbParameter createParameter(DbCommand command, string name, object value, DbType type)
      {
         var parameter = command.CreateParameter();
         parameter.ParameterName = name;
         parameter.Value = value;
         parameter.DbType = type;
         return parameter;
      }
   }
}