using System;
using System.Data;
using System.Data.Common;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   internal class SelectCreditTypesCommand : CommandBase
   {
      public SelectCreditTypesCommand(DbConnection connection, DbTransaction transaction)
         : base(connection, transaction)
      {
      }

      public CreditType[] Execute()
      {
         using (DataTable creditTypesTable = selectCreditTypes())
            return createCreditTypes(creditTypesTable);
      }

      private DataTable selectCreditTypes()
      {
         const string selectCreditTypesQuery = "SELECT * FROM CreditTypes;";

         using (DbCommand command = createCommand(selectCreditTypesQuery))
         using (DbDataReader reader = command.ExecuteReader())
         {
            var creditTypesTable = new DataTable();
            creditTypesTable.Load(reader);
            return creditTypesTable;
         }
      }

      private CreditType[] createCreditTypes(DataTable creditTypesTable)
      {
         DataRowCollection rows = creditTypesTable.Rows;
         var creditTypes = new CreditType[rows.Count];

         for (int i = 0; i < creditTypes.Length; i++)
         {
            DataRow row = rows[i];

            creditTypes[i] =
               CreditType.Create(
                  Convert.ToInt32(row[Id.Name]),
                  Convert.ToString(row[CreditTypeName.Name])
                  );
         }

         return creditTypes;
      }
   }
}