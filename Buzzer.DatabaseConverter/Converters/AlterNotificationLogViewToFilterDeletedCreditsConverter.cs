using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AlterNotificationLogViewToFilterDeletedCreditsConverter : ConverterBase
   {
      public AlterNotificationLogViewToFilterDeletedCreditsConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         string query = Resource.AlterNotificationLogViewToFilterDeletedCredits;
         using (DbCommand command = CommandFactory.CreateCommand(query))
            command.ExecuteNonQuery();
      }
   }
}