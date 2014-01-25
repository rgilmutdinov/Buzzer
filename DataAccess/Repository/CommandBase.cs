using System;
using System.Data;
using System.Data.SqlClient;
using Buzzer.DataAccess.Common;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal abstract class CommandBase
   {
      protected static readonly FieldInfo Id = new FieldInfo("ID", SqlDbType.Int);

      protected static readonly FieldInfo CreditNumber = new FieldInfo("CreditNumber", SqlDbType.NVarChar);
      protected static readonly FieldInfo CreditAmount = new FieldInfo("CreditAmount", SqlDbType.Decimal);
      protected static readonly FieldInfo CreditIssueDate = new FieldInfo("CreditIssueDate", SqlDbType.Date);
      protected static readonly FieldInfo MonthsCount = new FieldInfo("MonthsCount", SqlDbType.Int);
      protected static readonly FieldInfo DiscountRate = new FieldInfo("DiscountRate", SqlDbType.Decimal);
      protected static readonly FieldInfo EffectiveDiscountRate = new FieldInfo("EffectiveDiscountRate", SqlDbType.Decimal, true);
      protected static readonly FieldInfo ExchangeRate = new FieldInfo("ExchangeRate", SqlDbType.Decimal, true);
      
      protected static readonly FieldInfo CreditId = new FieldInfo("CreditId", SqlDbType.Int);
      protected static readonly FieldInfo PersonalNumber = new FieldInfo("PersonalNumber", SqlDbType.NVarChar);
      protected static readonly FieldInfo Name = new FieldInfo("Name", SqlDbType.NVarChar);
      protected static readonly FieldInfo RegistrationAddress = new FieldInfo("RegistrationAddress", SqlDbType.NVarChar);
      protected static readonly FieldInfo FactAddress = new FieldInfo("FactAddress", SqlDbType.NVarChar);
      protected static readonly FieldInfo PassportNumber = new FieldInfo("PassportNumber", SqlDbType.NVarChar);
      protected static readonly FieldInfo PassportIssueDate = new FieldInfo("PassportIssueDate", SqlDbType.Date);
      protected static readonly FieldInfo PassportIssuer = new FieldInfo("PassportIssuer", SqlDbType.NVarChar);
      protected static readonly FieldInfo IsBorrower = new FieldInfo("IsBorrower", SqlDbType.Bit);
      
      protected static readonly FieldInfo PersonId = new FieldInfo("PersonID", SqlDbType.Int);
      protected static readonly FieldInfo PhoneNumber = new FieldInfo("PhoneNumber", SqlDbType.NVarChar);

      protected static readonly FieldInfo PaymentAmount = new FieldInfo("PaymentAmount", SqlDbType.Decimal);
      protected static readonly FieldInfo PaymentDate = new FieldInfo("PaymentDate", SqlDbType.Date);
      protected static readonly FieldInfo IsNotified = new FieldInfo("IsNotified", SqlDbType.Bit);

      protected CommandBase(SqlConnection connection, SqlTransaction transaction)
      {
         Check.NotNull(connection, "connection");
         Check.NotNull(transaction, "transaction");

         Connection = connection;
         Transaction = transaction;
      }

      protected SqlConnection Connection { get; private set; }

      protected SqlTransaction Transaction { get; private set; }

      protected SqlCommand createCommand(string query)
      {
         SqlCommand command = Connection.CreateCommand();
         command.Transaction = Transaction;
         command.CommandText = query;
         return command;
      }

      protected static TValue? get<TValue>(object value, Func<object, TValue> converter) where TValue : struct
      {
         return value == DBNull.Value ? (TValue?) null : converter(value);
      }
   }
}