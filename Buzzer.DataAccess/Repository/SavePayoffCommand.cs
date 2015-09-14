using System;
using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SavePayoffCommand : CommandBase
   {
      private readonly PayoffInfo _payoff;

      public SavePayoffCommand(DbConnection connection, DbTransaction transaction, PayoffInfo payoff)
         : base(connection, transaction)
      {
         Check.NotNull(payoff, "payoff");
         _payoff = payoff;
      }

      public void Execute()
      {
         if (_payoff.CreditId == NullValues.Id)
            throw new InvalidOperationException();

         if (_payoff.IsNew)
         {
            insertPayoff();
         }
         else
         {
            updatePayoff();
         }
      }

      private void insertPayoff()
      {
         var insertPayoffQuery =
            string.Format(
               "INSERT INTO Payoffs ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",
               CreditId.Name, PayoffDate.Name,
               PayoffAmount.Name, Remarks.Name,
               CreditId.ParameterName, PayoffDate.ParameterName,
               PayoffAmount.ParameterName, Remarks.ParameterName);

         using (var command = createCommand(insertPayoffQuery))
         {
            command.AddParameter(_payoff.CreditId, CreditId);
            command.AddParameter(_payoff.PayoffDate, PayoffDate);
            command.AddParameter(_payoff.PayoffAmount, PayoffAmount);
            command.AddParameter(_payoff.Remarks, Remarks);

            _payoff.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updatePayoff()
      {
         var updatePayoffQuery =
            string.Format(
               "UPDATE Payoffs SET {0}={1}, {2}={3}, {4}={5} WHERE {6}={7};",
               PayoffDate.Name, PayoffDate.ParameterName,
               PayoffAmount.Name, PayoffAmount.ParameterName,
               Remarks.Name, Remarks.ParameterName,
               Id.Name, Id.ParameterName);

         using (var command = createCommand(updatePayoffQuery))
         {
            command.AddParameter(_payoff.PayoffDate, PayoffDate);
            command.AddParameter(_payoff.PayoffAmount, PayoffAmount);
            command.AddParameter(_payoff.Remarks, Remarks);
            command.AddParameter(_payoff.Id, Id);

            command.ExecuteNonQuery();
         }
      }
   }
}
