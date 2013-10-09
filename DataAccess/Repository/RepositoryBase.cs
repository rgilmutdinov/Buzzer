using System;
using System.Data.SqlClient;
using Common;
using DataAccess.Model;

namespace DataAccess.Repository
{
   public abstract class RepositoryBase<T> where T : RepositoryItem
   {
      private readonly string _connectionString;

      protected RepositoryBase(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");
         _connectionString = connectionString;
      }

      public void Save(T item)
      {
         executeInTransaction(
            (connection, transaction) => save(item, connection, transaction)
            );
      }

      public void Delete(T item)
      {
         executeInTransaction(
            (connection, transaction) => delete(item, connection, transaction)
            );
      }

      protected abstract void save(T item, SqlConnection connection, SqlTransaction transaction);
      protected abstract void delete(T item, SqlConnection connection, SqlTransaction transaction);
      
      protected void execute(Action<SqlConnection> query)
      {
         using (var connection = new SqlConnection())
         {
            connection.ConnectionString = _connectionString;
            connection.Open();
            query(connection);
         }
      }

      protected void executeInTransaction(Action<SqlConnection, SqlTransaction> query)
      {
         execute(
            connection =>
               {
                  var transaction = connection.BeginTransaction();

                  try
                  {
                     query(connection, transaction);
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
   }
}