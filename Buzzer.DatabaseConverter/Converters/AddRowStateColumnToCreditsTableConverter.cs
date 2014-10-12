using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddRowStateColumnToCreditsTableConverter : ConverterBase
   {
      public AddRowStateColumnToCreditsTableConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.AddRowStateColumnToCreditsTable;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}