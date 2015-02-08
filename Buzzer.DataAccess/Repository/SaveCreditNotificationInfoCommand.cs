using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveCreditNotificationInfoCommand : CommandBase
   {
      private readonly CreditInfo _creditInfo;

      public SaveCreditNotificationInfoCommand(DbConnection connection, DbTransaction transaction, CreditInfo creditInfo)
         : base(connection, transaction)
      {
         Check.NotNull(creditInfo, "creditInfo");
         _creditInfo = creditInfo;
      }

      public void Execute()
      {
         string updateNotificationInfoQuery =
            string.Format(
               "UPDATE Credits SET {0}={1}, {2}={3} WHERE {4}={5};",
               RequiredDocumentNotificationCount.Name, RequiredDocumentNotificationCount.ParameterName,
               RequiredDocumentNotificationDate.Name, RequiredDocumentNotificationDate.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updateNotificationInfoQuery))
         {
            command.AddParameter(_creditInfo.NotificationCount, RequiredDocumentNotificationCount);
            command.AddParameter(_creditInfo.NotificationDate, RequiredDocumentNotificationDate);
            command.AddParameter(_creditInfo.Id, Id);

            command.ExecuteNonQuery();
         }
      }
   }
}