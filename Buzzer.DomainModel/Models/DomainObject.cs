using System.Collections.Generic;
using System.ComponentModel;

namespace Buzzer.DomainModel.Models
{
   public abstract class DomainObject : IDataErrorInfo
   {
      protected DomainObject()
      {
         Id = NullValues.Id;
      }

      public int Id { get; set; }

      public bool IsNew
      {
         get { return Id == NullValues.Id; }
      }

      public virtual bool IsValid()
      {
         var errorInfo = (IDataErrorInfo) this;
         var isValid = true;

         foreach (var field in getRequiredFields())
            isValid &= errorInfo[field] == null;

         return isValid;
      }

      string IDataErrorInfo.this[string columnName]
      {
         get { return getErrorInfo(columnName); }
      }

      string IDataErrorInfo.Error
      {
         get { return null; }
      }

      protected abstract string getErrorInfo(string columnName);
      protected abstract IEnumerable<string> getRequiredFields();
   }
}