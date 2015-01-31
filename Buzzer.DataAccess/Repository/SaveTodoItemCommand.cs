using System;
using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal sealed class SaveTodoItemCommand : CommandBase
   {
      private readonly TodoItem _todoItem;

      public SaveTodoItemCommand(DbConnection connection, DbTransaction transaction, TodoItem todoItem)
         : base(connection, transaction)
      {
         Check.NotNull(todoItem, "todoItem");
         _todoItem = todoItem;
      }

      public void Execute()
      {
         if (_todoItem.CreditId == NullValues.Id)
            throw new InvalidOperationException();

         if (_todoItem.IsNew)
            insertTodoItem();
         else
            updateTodoItem();
      }

      private void insertTodoItem()
      {
         string insertTodoItemQuery =
            string.Format(
               "INSERT INTO TodoItems ({0}, {1}, {2}, {3}, {4}) VALUES ({5}, {6}, {7}, {8}, {9});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, Description.Name, TodoItemState.Name,
               NotificationCount.Name, NotificationDate.Name,

               CreditId.ParameterName, Description.ParameterName, TodoItemState.ParameterName,
               NotificationCount.ParameterName, NotificationDate.ParameterName
               );

         using (DbCommand command = createCommand(insertTodoItemQuery))
         {
            command.AddParameter(_todoItem.CreditId, CreditId);
            command.AddParameter(_todoItem.Description, Description);
            command.AddParameter(_todoItem.State, TodoItemState);
            command.AddParameter(_todoItem.NotificationCount, NotificationCount);
            command.AddParameter(_todoItem.NotificationDate, NotificationDate);

            _todoItem.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateTodoItem()
      {
         string updateTodoItemQuery =
            string.Format(
               "UPDATE TodoItems SET {0}={1}, {2}={3}, {4}={5}, {6}={7} WHERE {8}={9};",
               Description.Name, Description.ParameterName,
               TodoItemState.Name, TodoItemState.ParameterName,
               NotificationCount.Name, NotificationCount.ParameterName,
               NotificationDate.Name, NotificationDate.ParameterName,
               Id.Name, Id.ParameterName
               );
         
         using (DbCommand command = createCommand(updateTodoItemQuery))
         {
            command.AddParameter(_todoItem.Description, Description);
            command.AddParameter(_todoItem.State, TodoItemState);
            command.AddParameter(_todoItem.NotificationCount, NotificationCount);
            command.AddParameter(_todoItem.NotificationDate, NotificationDate);
            command.AddParameter(_todoItem.Id, Id);

            command.ExecuteNonQuery();
         }
      }
   }
}