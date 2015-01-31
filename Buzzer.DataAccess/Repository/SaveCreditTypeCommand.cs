using System;
using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveCreditTypeCommand : CommandBase
   {
      private readonly CreditType _creditType;

      public SaveCreditTypeCommand(DbConnection connection, DbTransaction transaction, CreditType creditType)
         : base(connection, transaction)
      {
         Check.NotNull(creditType, "creditType");
         _creditType = creditType;
      }

      public void Execute()
      {
         if (_creditType.IsNew)
            insertCreditType();
         else
            updateCreditType();
      }
      
      private void insertCreditType()
      {
         string insertCreditTypeQuery =
            string.Format(
               "INSERT INTO CreditTypes ({0}) VALUES ({1});" +
               "SELECT last_insert_rowid();",
               CreditTypeName.Name, CreditTypeName.ParameterName
               );

         using (DbCommand command = createCommand(insertCreditTypeQuery))
         {
            command.AddParameter(_creditType.Name, CreditTypeName);
            _creditType.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateCreditType()
      {
         string updateCreditTypeQuery =
            string.Format(
               "UPDATE CreditTypes SET {0}={1} WHERE {2}={3}",
               CreditTypeName.Name, CreditTypeName.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updateCreditTypeQuery))
         {
            command.AddParameter(_creditType.Name, CreditTypeName);
            command.AddParameter(_creditType.Id, Id);

            command.ExecuteNonQuery();
         }
      }
   }
}