using System;
using System.Data;
using System.Data.SqlClient;
using Buzzer.DataAccess.Common;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   [Obsolete("Delete", true)]
   internal abstract class RepositoryBase<T> where T : RepositoryItem
   {
      protected static readonly FieldInfo Id = new FieldInfo("ID", SqlDbType.Int);

      private readonly string _connectionString;

      protected RepositoryBase(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");
         _connectionString = connectionString;
      }

      public T Select(int id)
      {
         var condition = string.Format("{0}={1}", Id.Name, id);
         var result = Select(condition);
         return result.Length == 0 ? null : result[0];
      }

      public T[] SelectAll()
      {
         return execute(connection => query(string.Empty, connection));
      }

      public T[] Select(string condition)
      {
         var whereClause = string.Format("WHERE {0}", condition);
         return execute(connection => query(whereClause, connection));
      }

      public void Insert(T item)
      {
         execute(connection => insert(item, connection));
      }

      public void Update(T item)
      {
         execute(connection => update(item, connection));
      }

      public void Delete(T item)
      {
         execute(connection => delete(item, connection));
      }

      protected abstract T[] query(string whereClause, SqlConnection connection);
      protected abstract void insert(T item, SqlConnection connection);
      protected abstract void update(T item, SqlConnection connection);
      protected abstract void delete(T item, SqlConnection connection);

      protected TValue? get<TValue>(object value, Func<object, TValue> converter) where TValue : struct
      {
         return value == DBNull.Value ? (TValue?) null : converter(value);
      }

      private T[] execute(Func<SqlConnection, T[]> query)
      {
         using (var connection = new SqlConnection())
         {
            connection.ConnectionString = _connectionString;
            connection.Open();
            return query(connection);
         }
      }

      private void execute(Action<SqlConnection> query)
      {
         using (var connection = new SqlConnection())
         {
            connection.ConnectionString = _connectionString;
            connection.Open();
            query(connection);
         }
      }
   }
}