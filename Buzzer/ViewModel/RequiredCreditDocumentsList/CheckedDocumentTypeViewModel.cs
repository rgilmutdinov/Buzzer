using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.RequiredCreditDocumentsList
{
   public class CheckedDocumentTypeViewModel : ViewModelBase
   {
      private readonly DocumentType _documentType;

      public CheckedDocumentTypeViewModel(DocumentType documentType, bool isChecked)
      {
         Check.NotNull(documentType, "documentType");

         _documentType = documentType;
         IsChecked = isChecked;
      }

      public DocumentType DocumentType
      {
         get { return _documentType; }
      }

      public bool IsChecked { get; set; }

      public string Name
      {
         get { return _documentType.Name; }
      }
   }
}