using System;
using System.Collections.Generic;

namespace Buzzer.DomainModel.Models
{
   public sealed class CreditType : DomainObject
   {
      public static CreditType CreateNew()
      {
         return new CreditType {Name = string.Empty};
      }

      public static CreditType Create(int id, string name)
      {
         return new CreditType {Id = id, Name = name};
      }

      private CreditType()
      {
      }
    
      public string Name { get; set; }

      protected override string getErrorInfo(string columnName)
      {
         throw new NotImplementedException();
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         throw new NotImplementedException();
      }
   }
}