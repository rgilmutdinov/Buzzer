using System.Data;
using Common;

namespace Buzzer.DataAccess.Common
{
   internal sealed class FieldInfo
   {
      public FieldInfo(string name, SqlDbType dbType, bool isNullable = false)
      {
         Check.NotNullAndEmpty(name, "name");
         
         Name = name;
         ParameterName = "@" + name;
         DbType = dbType;
         IsNullable = isNullable;
      }

      public string Name { get; private set; }
      public string ParameterName { get; private set; }
      public SqlDbType DbType { get; private set; }
      public bool IsNullable { get; private set; }
   }
}