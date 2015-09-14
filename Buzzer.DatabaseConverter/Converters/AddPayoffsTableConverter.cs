using System.Data.Common;
using Buzzer.DatabaseConverter.Properties;

namespace Buzzer.DatabaseConverter.Converters
{
    internal sealed class AddPayoffsTableConverter : ConverterBase
    {
        public AddPayoffsTableConverter(CommandFactory commandFactory)
            : base(commandFactory)
        {
        }

        public override void Execute()
        {
            using (DbCommand command = CommandFactory.CreateCommand(Resource.AddPayoffsTable))
                command.ExecuteNonQuery();
        }
    }
}
