using System;
using System.Data;
using System.Data.Common;
using Buzzer.DataAccess.Common;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal abstract class CommandBase
   {
      protected static readonly FieldInfo Id = new FieldInfo("ID", DbType.Int32);

      protected static readonly FieldInfo CreditNumber = new FieldInfo("CreditNumber", DbType.String, true);
      protected static readonly FieldInfo ApplicationDate = new FieldInfo("ApplicationDate", DbType.Date, true);
      protected static readonly FieldInfo ProtocolDate = new FieldInfo("ProtocolDate", DbType.Date, true);
      protected static readonly FieldInfo CreditAmount = new FieldInfo("CreditAmount", DbType.Decimal);
      protected static readonly FieldInfo CreditIssueDate = new FieldInfo("CreditIssueDate", DbType.Date);
      protected static readonly FieldInfo MonthsCount = new FieldInfo("MonthsCount", DbType.Int32);
      protected static readonly FieldInfo DiscountRate = new FieldInfo("DiscountRate", DbType.Decimal);
      protected static readonly FieldInfo EffectiveDiscountRate = new FieldInfo("EffectiveDiscountRate", DbType.Decimal, true);
      protected static readonly FieldInfo ExchangeRate = new FieldInfo("ExchangeRate", DbType.Decimal, true);
      protected static readonly FieldInfo CreditState = new FieldInfo("CreditState", DbType.Int32);

      protected static readonly FieldInfo CreditId = new FieldInfo("CreditId", DbType.Int32);
      protected static readonly FieldInfo PersonalNumber = new FieldInfo("PersonalNumber", DbType.String, true);
      protected static readonly FieldInfo Name = new FieldInfo("Name", DbType.String, true);
      protected static readonly FieldInfo RegistrationAddress = new FieldInfo("RegistrationAddress", DbType.String, true);
      protected static readonly FieldInfo FactAddress = new FieldInfo("FactAddress", DbType.String, true);
      protected static readonly FieldInfo PassportNumber = new FieldInfo("PassportNumber", DbType.String, true);
      protected static readonly FieldInfo PassportIssueDate = new FieldInfo("PassportIssueDate", DbType.Date);
      protected static readonly FieldInfo PassportIssuer = new FieldInfo("PassportIssuer", DbType.String, true);
      protected static readonly FieldInfo PersonType = new FieldInfo("PersonType", DbType.Int32);
      
      protected static readonly FieldInfo PersonId = new FieldInfo("PersonID", DbType.Int32);
      protected static readonly FieldInfo PhoneNumber = new FieldInfo("PhoneNumber", DbType.String);

      protected static readonly FieldInfo PaymentAmount = new FieldInfo("PaymentAmount", DbType.Decimal);
      protected static readonly FieldInfo PaymentDate = new FieldInfo("PaymentDate", DbType.Date);
      protected static readonly FieldInfo IsNotified = new FieldInfo("IsNotified", DbType.Boolean);

      protected CommandBase(DbConnection connection, DbTransaction transaction)
      {
         Check.NotNull(connection, "connection");
         Check.NotNull(transaction, "transaction");

         Connection = connection;
         Transaction = transaction;
      }

      protected DbConnection Connection { get; private set; }

      protected DbTransaction Transaction { get; private set; }

      protected DbCommand createCommand(string query)
      {
         DbCommand command = Connection.CreateCommand();
         command.Transaction = Transaction;
         command.CommandText = query;
         return command;
      }

      protected static TValue? getNullable<TValue>(object value, Func<object, TValue> converter) where TValue : struct
      {
         return value == DBNull.Value ? (TValue?) null : converter(value);
      }

      protected static TValue get<TValue>(object value, Func<Object, TValue> converter) where TValue : class
      {
         return value == DBNull.Value ? null : converter(value);
      }
   }
}