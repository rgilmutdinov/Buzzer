using System;
using System.ComponentModel;

namespace Buzzer.ViewModel.Common
{
   public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
   {
      public string DisplayName { get; protected set; }

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      protected void propertyChanged(string name)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         onDispose();
      }

      protected virtual void onDispose()
      {
      }

      #endregion
   }
}