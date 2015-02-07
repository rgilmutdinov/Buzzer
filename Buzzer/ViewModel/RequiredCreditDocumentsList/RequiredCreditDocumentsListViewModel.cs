using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Properties;
using Buzzer.ViewModel.Common;
using Common;
using NLog;

namespace Buzzer.ViewModel.RequiredCreditDocumentsList
{
   public class RequiredCreditDocumentsListViewModel : WorkspaceViewModel
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private readonly BuzzerDatabase _buzzerDatabase;

      private ICommand _addDocumentTypeCommand;
      private ICommand _saveDocumentTypesCommand;

      private ICommand _addRequiredCreditDocumentsCommand;
      private ICommand _saveRequiredCreditDocumentsCommand;

      public RequiredCreditDocumentsListViewModel(BuzzerDatabase buzzerDatabase)
      {
         Check.NotNull(buzzerDatabase, "buzzerDatabase");

         _buzzerDatabase = buzzerDatabase;

         DocumentTypes = getDocumentTypes();
         RequiredCreditDocuments = getRequiredCreditDocuments();

         DisplayName = Resources.RequiredCreditDocumentsViewModel_Caption;
      }

      public ObservableCollection<DocumentTypeViewModel> DocumentTypes { get; private set; }

      public ObservableCollection<RequiredCreditDocumentsViewModel> RequiredCreditDocuments { get; private set; }

      public ICommand AddDocumentType
      {
         get
         {
            if (_addDocumentTypeCommand != null)
               return _addDocumentTypeCommand;

            _addDocumentTypeCommand = new CommandDelegate(addDocumentType);
            return _addDocumentTypeCommand;
         }
      }

      public ICommand SaveDocumentTypes
      {
         get
         {
            if (_saveDocumentTypesCommand != null)
               return _saveDocumentTypesCommand;

            _saveDocumentTypesCommand = new CommandDelegate(saveDocumentTypes, canSaveDocumentTypes);
            return _saveDocumentTypesCommand;
         }
      }

      public ICommand AddRequiredCreditDocuments
      {
         get
         {
            if (_addRequiredCreditDocumentsCommand != null)
               return _addRequiredCreditDocumentsCommand;

            _addRequiredCreditDocumentsCommand = new CommandDelegate(addRequiredCreditDocuments);
            return _addRequiredCreditDocumentsCommand;
         }
      }

      public object SaveRequiredCreditDocuments
      {
         get
         {
            if (_saveRequiredCreditDocumentsCommand != null)
               return _saveRequiredCreditDocumentsCommand;

            _saveRequiredCreditDocumentsCommand = new CommandDelegate(saveRequiredCreditDocuments,
                                                                      canSaveRequiredCreditDocuments);
            return _saveRequiredCreditDocumentsCommand;
         }
      }

      private ObservableCollection<DocumentTypeViewModel> getDocumentTypes()
      {
         return
            new ObservableCollection<DocumentTypeViewModel>(
               _buzzerDatabase
                  .GetAllDocumentTypes()
                  .Select(item => new DocumentTypeViewModel(item))
               );
      }

      private ObservableCollection<RequiredCreditDocumentsViewModel> getRequiredCreditDocuments()
      {
         DocumentType[] documentTypes = getOriginalDocumentTypes();
         CreditType[] creditType = _buzzerDatabase.GetAllCreditTypes();
         RequiredCreditDocuments[] requiredCreditDocuments = _buzzerDatabase.GetAllRequiredCreditDocuments();

         return
            new ObservableCollection<RequiredCreditDocumentsViewModel>(
               creditType.Select(
                  item => new RequiredCreditDocumentsViewModel(
                             documentTypes, item, getRequiredDocuments(item, requiredCreditDocuments)
                             )
                  )
               );
      }

      private DocumentType[] getOriginalDocumentTypes()
      {
         return DocumentTypes.Select(item => item.Original).ToArray();
      }

      private RequiredCreditDocuments getRequiredDocuments(CreditType creditType, IEnumerable<RequiredCreditDocuments> requiredCreditDocuments)
      {
         return requiredCreditDocuments.SingleOrDefault(item => item.CreditType.Id == creditType.Id);
      }

      private void addDocumentType()
      {
         DocumentType documentType = DocumentType.CreateNew();
         DocumentTypes.Add(new DocumentTypeViewModel(documentType));
      }

      private void saveDocumentTypes()
      {
         saveData(
            () =>
               {
                  foreach (DocumentTypeViewModel documentType in DocumentTypes)
                     _buzzerDatabase.SaveDocumentType(documentType.Original);
               }
            );
      }

      private bool canSaveDocumentTypes()
      {
         return DocumentTypes.All(item => item.Original.IsValid());
      }

      private void addRequiredCreditDocuments()
      {
         CreditType creditType = CreditType.CreateNew();
         var viewModel = new RequiredCreditDocumentsViewModel(getOriginalDocumentTypes(), creditType, null);
         RequiredCreditDocuments.Add(viewModel);
      }

      private void saveRequiredCreditDocuments()
      {
         saveData(
            () =>
               {
                  foreach (RequiredCreditDocumentsViewModel creditDocuments in RequiredCreditDocuments)
                  {
                     _buzzerDatabase.SaveCreditType(creditDocuments.CreditType);

                     RequiredCreditDocuments requiredCreditDocuments = creditDocuments.GetRequiredCreditDocuments();
                     
                     if (requiredCreditDocuments != null)
                        _buzzerDatabase.SaveRequiredCreditDocuments(requiredCreditDocuments);
                  }
               }
            );
      }

      private bool canSaveRequiredCreditDocuments()
      {
         return RequiredCreditDocuments.All(item => item.CreditType.IsValid());
      }

      private void saveData(Action saveDataAction)
      {
         try
         {
            saveDataAction();
         }
         catch (Exception e)
         {
            MessageBox.Show(Resources.ErrorWhileSavingInformationToDatabase,
                            Resources.BuzzerErrorMessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Logger.Error(e);
         }
      }
   }
}
