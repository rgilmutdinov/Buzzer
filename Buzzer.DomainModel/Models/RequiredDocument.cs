using System;
using System.Collections.Generic;

namespace Buzzer.DomainModel.Models
{
   public sealed class RequiredDocument : DomainObject
   {
      public static RequiredDocument CreateNew(int creditId, DocumentType documentType)
      {
         return new RequiredDocument
                   {
                      CreditId = creditId,
                      DocumentType = documentType,
                      State = RequiredDocumentState.None
                   };
      }

      public static RequiredDocument Create(
         int id,
         int creditId,
         DocumentType documentType,
         RequiredDocumentState state
         )
      {
         return new RequiredDocument
                   {
                      Id = id,
                      CreditId = creditId,
                      DocumentType = documentType,
                      State = state
                   };
      }

      private RequiredDocument()
      {
      }

      public int CreditId { get; set; }

      public DocumentType DocumentType { get; set; }

      public RequiredDocumentState State { get; set; }

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