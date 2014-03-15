using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddNotificationLogConverter : ConverterBase
   {
      public AddNotificationLogConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         using (DbCommand command = CommandFactory.CreateCommand(Resource.AddNotificationLogTable))
            command.ExecuteNonQuery();
      }
   }
}