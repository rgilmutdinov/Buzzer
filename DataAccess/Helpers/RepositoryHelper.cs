using System;
using System.Data.SqlClient;
using DataAccess.Repository;

namespace DataAccess.Helpers
{
   internal static class RepositoryHelper
   {
      internal static void AddParameter(this SqlCommand command, object value, FieldInfo fieldInfo)
      {
         var parameter = command.CreateParameter();
         parameter.ParameterName = fieldInfo.ParameterName;
         parameter.SqlDbType = fieldInfo.DbType;
         parameter.IsNullable = fieldInfo.IsNullable;
         parameter.SqlValue = fieldInfo.IsNullable && value == null ? DBNull.Value : value;
         command.Parameters.Add(parameter);
      }
   }
}