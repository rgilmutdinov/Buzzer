using System.Data.SqlClient;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   public sealed class BuzzerDatabase
   {
      private readonly string _connectionString;

      public BuzzerDatabase(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");
         _connectionString = connectionString;
      }

      public CreditInfo[] GetAllCredits()
      {
         using (var connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
               var selectCommand = new SelectCreditsCommand(connection, transaction);
               CreditInfo[] creditInfos = selectCommand.Execute();
               transaction.Commit();
               return creditInfos;
            }
         }
      }

      public void SaveCredit(CreditInfo credit)
      {
         using (var connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
               var saveCommand = new SaveCreditCommand(connection, transaction, credit);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }

      public void SavePayment(PaymentInfo payment)
      {
         using (var connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
               var saveCommand = new SavePaymentCommand(connection, transaction, payment);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }
   }
}