using System.ComponentModel;

namespace Buzzer.ViewModel.Common
{
   public abstract class ViewModelBase : INotifyPropertyChanged
   {
      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      protected void propertyChanged(string name)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
      }

      #endregion
   }
}