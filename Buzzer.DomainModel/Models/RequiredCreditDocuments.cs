using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Buzzer.DomainModel.Models
{
   public sealed class RequiredCreditDocuments : DomainObject
   {
      private List<DocumentType> _documentTypes; 

      public static RequiredCreditDocuments Create(CreditType creditType, IEnumerable<DocumentType> documentTypes)
      {
         return new RequiredCreditDocuments
                   {
                      CreditType = creditType,
                      _documentTypes = documentTypes.ToList()
                   };
      }

      public static RequiredCreditDocuments CreateNew(CreditType creditType)
      {
         return new RequiredCreditDocuments
                   {
                      CreditType = creditType,
                      _documentTypes = new List<DocumentType>()
                   };
      }

      public CreditType CreditType { get; set; }

      public ReadOnlyCollection<DocumentType> DocumentTypes
      {
         get { return _documentTypes.AsReadOnly(); }
      }

      public void AddDocumentType(DocumentType documentType)
      {
         _documentTypes.Add(documentType);
      }

      public void RemoveDocumentType(DocumentType documentType)
      {
         DocumentType itemToRemove = _documentTypes.FirstOrDefault(item => item.Id == documentType.Id);
         _documentTypes.Remove(itemToRemove);
      }

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