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

      public void SaveNotificationLogItem(NotificationLogItemInfo notificationLogItem)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var saveCommand = new SaveNotificationLogItemCommand(connection, transaction, notificationLogItem);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }

      public NotificationLogItemInfo[] GetNotificationLogItems()
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var selectCommand = new SelectNotificationLogItemsCommand(connection, transaction);
               NotificationLogItemInfo[] result = selectCommand.Execute();
               transaction.Commit();
               return result;
            }
         }
      }

      public bool CheckUser(string login, string password)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var checkUserCommand = new CheckUserCommand(connection, transaction, login, password);
               bool result = checkUserCommand.Execute();
               transaction.Commit();
               return result;
            }
         }
      }

      public void SaveTodoItem(TodoItem todoItem)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var command = new SaveTodoItemCommand(connection, transaction, todoItem);
               command.Execute();
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

      public DocumentType[] GetAllDocumentTypes()
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var select = new SelectDocumentTypesCommand(connection, transaction);
               DocumentType[] result = select.Execute();
               transaction.Commit();
               return result;
            }
         }
      }

      public void SaveDocumentType(DocumentType documentType)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var saveCommand = new SaveDocumentTypeCommand(connection, transaction, documentType);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }

      public CreditType[] GetAllCreditTypes()
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var selectCommand = new SelectCreditTypesCommand(connection, transaction);
               CreditType[] result = selectCommand.Execute();
               transaction.Commit();
               return result;
            }
         }
      }

      public void SaveCreditType(CreditType creditType)
      {
         using (DbConnection connection = createConnection())
         {
            using (DbTransaction transaction = createTransaction(connection))
            {
               var saveCommand = new SaveCreditTypeCommand(connection, transaction, creditType);
               saveCommand.Execute();
               transaction.Commit();
            }
         }
      }
   }
}