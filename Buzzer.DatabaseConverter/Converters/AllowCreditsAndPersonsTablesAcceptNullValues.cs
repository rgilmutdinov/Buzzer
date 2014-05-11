using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AllowCreditsAndPersonsTablesAcceptNullValues : ConverterBase
   {
      public AllowCreditsAndPersonsTablesAcceptNullValues(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.ChangeCreditsAndPersonsTablesToAllowNullValues;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}