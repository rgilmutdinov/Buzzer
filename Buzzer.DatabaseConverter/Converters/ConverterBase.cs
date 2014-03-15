using Common;

namespace Buzzer.DatabaseConverter.Converters
{
   internal abstract class ConverterBase
   {
      protected ConverterBase(CommandFactory commandFactory)
      {
         Check.NotNull(commandFactory, "commandFactory");
         CommandFactory = commandFactory;
      }

      protected CommandFactory CommandFactory { get; private set; }

      public abstract void Execute();
   }
}