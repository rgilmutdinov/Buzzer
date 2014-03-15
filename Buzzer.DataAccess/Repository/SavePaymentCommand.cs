using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SavePaymentCommand : CommandBase
   {
      private readonly PaymentInfo _payment;

      public SavePaymentCommand(DbConnection connection, DbTransaction transaction, PaymentInfo payment)
         : base(connection, transaction)
      {
         Check.NotNull(payment, "payment");
         _payment = payment;
      }

      public void Execute()
      {
         string updatePaymentQuery =
            string.Format(
               "UPDATE PaymentsSchedule SET {0} = {1} WHERE {2} = {3};",
               IsNotified.Name, IsNotified.ParameterName, Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updatePaymentQuery))
         {
            command.AddParameter(_payment.IsNotified, IsNotified);
            command.AddParameter(_payment.Id, Id);
            command.ExecuteNonQuery();
         }
      }
   }
}