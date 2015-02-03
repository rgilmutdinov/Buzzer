using System.Data.Common;
using System.Linq;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveRequiredCreditDocumentsCommand : CommandBase
   {
      private readonly RequiredCreditDocuments _requiredCreditDocuments;

      public SaveRequiredCreditDocumentsCommand(DbConnection connection, DbTransaction transaction, RequiredCreditDocuments requiredCreditDocuments)
         : base(connection, transaction)
      {
         Check.NotNull(requiredCreditDocuments, "requiredCreditDocuments");
         _requiredCreditDocuments = requiredCreditDocuments;
      }

      public void Execute()
      {
         RequiredCreditDocuments original = getRequiredCreditDocuments(_requiredCreditDocuments.CreditType.Id);

         if (original == null)
            insertRequiredCreditDocuments();
         else
            updateRequiredCreditDocuments();
      }

      private RequiredCreditDocuments getRequiredCreditDocuments(int creditTypeId)
      {
         var selectCommand = new SelectRequiredCreditDocuments(Connection, Transaction);
         RequiredCreditDocuments[] requiredCreditDocuments = selectCommand.Execute();
         return requiredCreditDocuments.SingleOrDefault(item => item.CreditType.Id == creditTypeId);
      }

      private void insertRequiredCreditDocuments()
      {
         string insertRequiredCreditDocumentsQuery =
            string.Format(
               "INSERT INTO RequiredCreditDocuments ({0}, {1}) VALUES ({2}, {3});",
               CreditTypeId.Name, DocumentTypeId.Name,
               CreditTypeId.ParameterName, DocumentTypeId.ParameterName
               );

         foreach (DocumentType documentType in _requiredCreditDocuments.DocumentTypes)
         {
            using (DbCommand command = createCommand(insertRequiredCreditDocumentsQuery))
            {
               command.AddParameter(_requiredCreditDocuments.CreditType.Id, CreditTypeId);
               command.AddParameter(documentType.Id, DocumentTypeId);
               command.ExecuteNonQuery();
            }
         }
      }

      private void updateRequiredCreditDocuments()
      {
         deleteRequiredCreditDocuments();
         insertRequiredCreditDocuments();
      }

      private void deleteRequiredCreditDocuments()
      {
         string deleteRequiredCreditDocumentsQuery =
            string.Format("DELETE FROM RequiredCreditDocuments WHERE {0}={1};",
                          CreditTypeId.Name, CreditTypeId.ParameterName);

         using (DbCommand command = createCommand(deleteRequiredCreditDocumentsQuery))
         {
            command.AddParameter(_requiredCreditDocuments.CreditType.Id, CreditTypeId);
            command.ExecuteNonQuery();
         }
      }
   }
}