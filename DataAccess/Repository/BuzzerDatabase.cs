using System.Data.Common;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   public sealed class BuzzerDatabase
   {
      private readonly string _connectionString;
      private readonly DbProviderFactory _factory;

      public BuzzerDatabase(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");
         _connectionString = connectionString;

         _factory = DbProviderFactories.GetFactory("System.Data.SQLite");
      }

      public CreditInfo[] GetAllCredits()
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
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
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var saveCommand = new SaveCreditCommand(connection, transaction, credit);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }

      public void SavePayment(PaymentInfo payment)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var saveCommand = new SavePaymentCommand(connection, transaction, payment);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }

      private DbConnection createConnection()
      {
         DbConnection connection = _factory.CreateConnection();
         connection.ConnectionString = _connectionString;
         connection.Open();
         return connection;
      }

      private DbTransaction createTransaction(DbConnection connection)
      {
         return connection.BeginTransaction();
      }
   }
}