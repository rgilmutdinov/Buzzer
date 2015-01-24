using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddTodoItemsTableConverter : ConverterBase
   {
      public AddTodoItemsTableConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.AddTodoItemsTable;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}