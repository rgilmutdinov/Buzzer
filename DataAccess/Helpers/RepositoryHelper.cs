using System;
using System.Data.Common;
using Buzzer.DataAccess.Common;

namespace Buzzer.DataAccess.Helpers
{
   internal static class RepositoryHelper
   {
      internal static void AddParameter(this DbCommand command, object value, FieldInfo fieldInfo)
      {
         var parameter = command.CreateParameter();
         parameter.ParameterName = fieldInfo.ParameterName;
         parameter.DbType = fieldInfo.DbType;
         parameter.IsNullable = fieldInfo.IsNullable;
         parameter.Value = fieldInfo.IsNullable && value == null ? DBNull.Value : value;
         command.Parameters.Add(parameter);
      }
   }
}