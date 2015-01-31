using System;
using System.Collections.Generic;

namespace Buzzer.DomainModel.Models
{
   public sealed class DocumentType : DomainObject
   {
      public static DocumentType CreateNew()
      {
         return new DocumentType {Name = string.Empty};
      }

      public static DocumentType Create(int id, string name)
      {
         return new DocumentType {Id = id, Name = name};
      }

      private DocumentType()
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