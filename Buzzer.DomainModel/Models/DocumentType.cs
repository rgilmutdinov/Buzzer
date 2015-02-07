using System;
using System.Collections.Generic;
using Buzzer.DomainModel.Properties;

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
         if (columnName == "Name")
            return validateName();

         throw new ArgumentException(columnName, "columnName");
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return new[] {"Name"};
      }

      private string validateName()
      {
         if (Name.SafeGetLength() > 255)
            return string.Format(Resources.MaxLengthExceeded, 255);

         return string.IsNullOrEmpty(Name) ? Resources.FieldMustBeFilled : null;
      }
   }
}