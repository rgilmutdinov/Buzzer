using System;
using System.Data;
using System.Data.Common;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   internal class SelectNotificationLogItemsCommand : CommandBase
   {
      public SelectNotificationLogItemsCommand(DbConnection connection, DbTransaction transaction)
         : base(connection, transaction)
      {
      }

      public NotificationLogItemInfo[] Execute()
      {
         const string query = "SELECT * FROM NotificationLogView";

         using (DbCommand command = createCommand(query))
         {
            using (var dataTable = new DataTable())
            {
               using (DbDataReader reader = command.ExecuteReader())
                  dataTable.Load(reader);

               var result = new NotificationLogItemInfo[dataTable.Rows.Count];

               for (int i = 0; i < dataTable.Rows.Count; i++)
               {
                  DataRow row = dataTable.Rows[i];

                  result[i] =
                     NotificationLogItemInfo.Create(
                        Convert.ToInt32(row["ID"]),
                        Convert.ToInt32(row["CreditID"]),
                        Convert.ToString(row["CreditNumber"]),
                        Convert.ToInt32(row["PersonID"]),
                        Convert.ToString(row["PersonName"]),
                        Convert.ToDateTime(row["NotificationDate"]),
                        get(row["Comment"], Convert.ToString)
                        );
               }

               return result;
            }
         }
      }
   }
}