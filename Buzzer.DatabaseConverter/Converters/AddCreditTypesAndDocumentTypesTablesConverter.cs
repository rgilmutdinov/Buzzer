using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddCreditTypesAndDocumentTypesTablesConverter : ConverterBase
   {
      public AddCreditTypesAndDocumentTypesTablesConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.AddCreditTypesAndDocumentTypesTables;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}