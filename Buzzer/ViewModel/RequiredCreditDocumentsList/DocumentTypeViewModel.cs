using System.ComponentModel;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.RequiredCreditDocumentsList
{
   public class DocumentTypeViewModel : ViewModelBase, IDataErrorInfo
   {
      private readonly DocumentType _documentType;

      public DocumentTypeViewModel(DocumentType documentType)
      {
         Check.NotNull(documentType, "documentType");
         _documentType = documentType;
      }

      public DocumentType Original
      {
         get { return _documentType; }
      }

      public string Name
      {
         get { return _documentType.Name; }
         set
         {
            if (_documentType.Name == value)
               return;

            _documentType.Name = value;
            propertyChanged("Name");
         }
      }

      public string this[string columnName]
      {
         get
         {
            if (columnName == "Name")
               return ((IDataErrorInfo) _documentType)["Name"];

            return null;
         }
      }

      public string Error
      {
         get { return null; }
      }
   }
}