using System.Collections.Generic;
using System.ComponentModel;
using Common;
using DataAccess.Common;
using DataAccess.Repository;

namespace DataAccess.Model
{
   public abstract class RepositoryItem : IDataErrorInfo
   {
      private BuzzerDatabase _context;

      protected RepositoryItem()
      {
         Id = NullValues.Id;
      }

      public int Id { get; internal set; }

      public bool IsNew
      {
         get { return Id == NullValues.Id; }
      }

      internal BuzzerDatabase Context
      {
         get { return _context; }
         set
         {
            Check.NotNull(value, "value");
            _context = value;
         }
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