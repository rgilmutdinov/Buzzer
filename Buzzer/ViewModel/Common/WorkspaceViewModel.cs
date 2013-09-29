namespace Buzzer.ViewModel.Common
{
   public abstract class WorkspaceViewModel : ViewModelBase
   {
      private string _name;

      public string Name
      {
         get { return _name; }
         set
         {
            if (_name == value)
               return;

            _name = value;
            propertyChanged("Name");
         }
      }
   }
}