using System;
using System.ComponentModel;
using System.Windows.Input;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.CreditContract
{
   public sealed class PayoffViewModel : ViewModelBase, IDataErrorInfo
   {
      private readonly bool _isUsd;

      private ICommand _savePayoffCommand;
      private ICommand _cancelCommand;

      public event EventHandler SavePayoff;
      public event EventHandler CancelPayoff;

      public PayoffViewModel(PayoffInfo payoffInfo, bool isUsd)
      {
         Check.NotNull(payoffInfo, "payoffInfo");

         Original = payoffInfo;
         _isUsd = isUsd;
      }

      public PayoffInfo Original { get; private set; }

      public DateTime PayoffDate
      {
         get { return Original.PayoffDate; }
         set
         {
            Original.PayoffDate = value;
            propertyChanged("PayoffDate");
         }
      }

      public decimal PayoffAmount
      {
         get { return Original.PayoffAmount; }
         set
         {
            Original.PayoffAmount = value;
            propertyChanged("PayoffAmount");
            propertyChanged("PayoffAmountText");
         }
      }

      public string Remarks
      {
         get { return Original.Remarks; }
         set
         {
            Original.Remarks = value;
            propertyChanged("Remarks");
         }
      }

      public string PayoffAmountText
      {
         get { return Original.PayoffAmount + (_isUsd ? " USD" : " KGS"); }
      }

      public void CopyDataFrom(PayoffViewModel source)
      {
         PayoffAmount = source.PayoffAmount;
         PayoffDate = source.PayoffDate;
         Remarks = source.Remarks;
      }

      #region Commands

      public ICommand SavePayoffCommand
      {
         get { return _savePayoffCommand ?? (_savePayoffCommand = new CommandDelegate(OnSavePayoff, canSavePayoff)); }
      }

      public ICommand CancelCommand
      {
         get { return _cancelCommand ?? (_cancelCommand = new CommandDelegate(OnCancelPayoff)); }
      }

      private bool canSavePayoff()
      {
         return Original != null && Original.IsValid();
      }

      private void OnSavePayoff()
      {
         if (SavePayoff != null)
            SavePayoff(this, EventArgs.Empty);
      }

      private void OnCancelPayoff()
      {
         if (CancelPayoff != null)
            CancelPayoff(this, EventArgs.Empty);
      }

      #endregion

      #region IDataErrorInfo Members

      string IDataErrorInfo.this[string columnName]
      {
         get
         {
            string error = null;

            switch (columnName)
            {
               case "PayoffAmount":
               case "PayoffDate":
               case "Remarks":
                  error = (Original as IDataErrorInfo)[columnName];
                  break;
            }

            CommandManager.InvalidateRequerySuggested();

            return error;
         }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }

      #endregion
   }
}
