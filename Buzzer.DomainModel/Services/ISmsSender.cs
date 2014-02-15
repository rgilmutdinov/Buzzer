namespace Buzzer.DomainModel.Services
{
   public interface ISmsSender
   {
      void Send(string message);
   }
}