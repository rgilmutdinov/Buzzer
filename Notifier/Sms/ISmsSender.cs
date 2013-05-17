namespace Notifier.Sms
{
   public interface ISmsSender
   {
      void Send(string message);
   }
}