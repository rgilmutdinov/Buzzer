using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddRequiredDocumentsTableConverter : ConverterBase
   {
      public AddRequiredDocumentsTableConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.AddRequiredDocumentsTable;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}