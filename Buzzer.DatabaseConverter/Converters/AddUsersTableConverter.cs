using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
   internal sealed class AddUsersTableConverter : ConverterBase
   {
      public AddUsersTableConverter(CommandFactory commandFactory)
         : base(commandFactory)
      {
      }

      public override void Execute()
      {
         using (DbCommand command = CommandFactory.CreateCommand(Resource.AddUsersTable))
            command.ExecuteNonQuery();
      }
   }
}