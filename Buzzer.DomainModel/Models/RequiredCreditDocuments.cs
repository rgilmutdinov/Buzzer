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

      public CreditType CreditType { get; private set; }

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
         return null;
      }

      protected override IEnumerable<string> getRequiredFields()
      {
         return Enumerable.Empty<string>();
      }
   }
}