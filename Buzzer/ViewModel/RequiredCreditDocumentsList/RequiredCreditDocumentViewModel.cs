using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Buzzer.DomainModel.Models;
using Buzzer.ViewModel.Common;
using Common;

namespace Buzzer.ViewModel.RequiredCreditDocumentsList
{
   public class RequiredCreditDocumentsViewModel : ViewModelBase, IDataErrorInfo
   {
      private readonly RequiredCreditDocuments _requiredCreditDocuments;
      private readonly CheckedDocumentTypeViewModel[] _checkedDocumentTypes;
      private readonly CreditType _creditType;

      public RequiredCreditDocumentsViewModel(DocumentType[] documentTypes, CreditType creditType, RequiredCreditDocuments requiredCreditDocuments)
      {
         Check.NotNull(documentTypes, "documentTypes");
         Check.NotNull(creditType, "creditType");

         _creditType = creditType;
         _requiredCreditDocuments = requiredCreditDocuments;
         _checkedDocumentTypes = getCheckedDocumentTypes(documentTypes);
      }

      public CreditType CreditType
      {
         get { return _creditType; }
      }

      public IEnumerable<CheckedDocumentTypeViewModel> CheckedDocumentTypes
      {
         get { return _checkedDocumentTypes; }
      }

      public string CreditTypeName
      {
         get { return _creditType.Name; }
         set
         {
            if (_creditType.Name == value)
               return;

            _creditType.Name = value;
            propertyChanged("CreditTypeName");
         }
      }

      public string this[string columnName]
      {
         get
         {
            if (columnName == "CreditTypeName")
               return ((IDataErrorInfo) _creditType)["Name"];

            return null;
         }
      }

      public string Error
      {
         get { return null; }
      }

      public RequiredCreditDocuments GetRequiredCreditDocuments()
      {
         DocumentType[] checkedDocumentTypes =
            CheckedDocumentTypes
               .Where(item => item.IsChecked)
               .Select(item => item.DocumentType)
               .ToArray();

         if (_requiredCreditDocuments == null)
            return RequiredCreditDocuments.Create(_creditType, checkedDocumentTypes);

         removeNotCheckedDocumentTypes(checkedDocumentTypes);
         addCheckedDocumentTypes(checkedDocumentTypes);

         return _requiredCreditDocuments;
      }

      private void removeNotCheckedDocumentTypes(DocumentType[] checkedDocumentTypes)
      {
         foreach (DocumentType documentType in _requiredCreditDocuments.DocumentTypes.ToArray())
         {
            if (!isCollectionContains(documentType, checkedDocumentTypes))
               _requiredCreditDocuments.RemoveDocumentType(documentType);
         }
      }

      private bool isCollectionContains(DocumentType documentType, IEnumerable<DocumentType> checkedDocumentTypes)
      {
         return checkedDocumentTypes.Any(item => item.Id == documentType.Id);
      }

      private void addCheckedDocumentTypes(DocumentType[] checkedDocumentTypes)
      {
         foreach (DocumentType documentType in checkedDocumentTypes)
         {
            if (!isCollectionContains(documentType, _requiredCreditDocuments.DocumentTypes))
               _requiredCreditDocuments.AddDocumentType(documentType);
         }
      }

      private CheckedDocumentTypeViewModel[] getCheckedDocumentTypes(DocumentType[] documentTypes)
      {
         var checkedDocumentTypes = new CheckedDocumentTypeViewModel[documentTypes.Length];

         for (int i = 0; i < checkedDocumentTypes.Length; i++)
         {
            bool isChecked = isDocumentTypeChecked(documentTypes[i]);
            checkedDocumentTypes[i] = new CheckedDocumentTypeViewModel(documentTypes[i], isChecked);
         }

         return checkedDocumentTypes;
      }

      private bool isDocumentTypeChecked(DocumentType documentType)
      {
         if (_requiredCreditDocuments == null)
            return false;

         return _requiredCreditDocuments.DocumentTypes.Any(item => item.Id == documentType.Id);
      }
   }
}