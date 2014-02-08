using System;
using System.ComponentModel;

namespace Buzzer.ViewModel.Common
{
   public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
   {
      private string _displayName;

      public string DisplayName
      {
         get { return _displayName; }
         protected set
         {
            _displayName = value;
            propertyChanged("DisplayName");
         }
      }

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