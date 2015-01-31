using System;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Tests.Common;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class SaveNotifiedTodoItemTests
   {
      private BuzzerDatabase _database;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _database = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      [ExpectedException(typeof (InvalidOperationException))]
      public void SaveNewTodoItemOfNewCreditTest()
      {
         CreditInfo credit = CreditInfo.CreatNew();
         TodoItem todoItem = credit.AddTodoItem();

         _database.SaveTodoItem(todoItem);
      }

      [Test]
      public void SaveNewTodoItemTest()
      {
         const string creditNumber = "CNSNTI";
         CreditInfo credit = getCreditByNumber(creditNumber);

         TodoItem todoItem = credit.AddTodoItem();
         todoItem.Description = "Todo item description";
         todoItem.Notified();

         _database.SaveTodoItem(todoItem);

         TodoItem todoItemFromDatabase = getTodoItemById(credit.Id, todoItem.Id);
         AssertUtils.AssertTodoItemsAreEqual(todoItemFromDatabase, todoItem);
      }

      [Test]
      public void SaveUpdatedTodoItemTest()
      {
         const string creditNumber = "CNSNTI";
         CreditInfo credit = getCreditByNumber(creditNumber);
         TodoItem todoItem = getTodoItemByDescription(credit, "Todo item to update");

         todoItem.Notified();
         todoItem.Description = "Todo item notified";
         todoItem.State = TodoItemState.Done;

         _database.SaveTodoItem(todoItem);

         TodoItem todoItemFromDatabase = getTodoItemById(credit.Id, todoItem.Id);
         AssertUtils.AssertTodoItemsAreEqual(todoItemFromDatabase, todoItem);
      }

      private CreditInfo getCreditByNumber(string creditNumber)
      {
         return
            _database
               .GetAllCredits()
               .Single(item => item.CreditNumber == creditNumber);
      }

      private TodoItem getTodoItemById(int creditId, int todoItemId)
      {
         return
            _database
               .GetAllCredits()
               .Single(item => item.Id == creditId)
               .TodoList
               .Single(item => item.Id == todoItemId);
      }
      
      private static TodoItem getTodoItemByDescription(CreditInfo credit, string description)
      {
         return credit.TodoList.Single(item => item.Description == description);
      }
   }
}