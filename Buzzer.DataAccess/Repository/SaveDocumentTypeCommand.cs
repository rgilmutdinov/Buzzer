using System;
using System.Data.Common;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveDocumentTypeCommand : CommandBase
   {
      private readonly DocumentType _documentType;

      public SaveDocumentTypeCommand(DbConnection connection, DbTransaction transaction, DocumentType documentType)
         : base(connection, transaction)
      {
         Check.NotNull(documentType, "documentType");
         _documentType = documentType;
      }

      public void Execute()
      {
         if (_documentType.IsNew)
            insertDocumentType();
         else
            updateDocumentType();
      }

      private void insertDocumentType()
      {
         string insertDocumentTypeQuery =
            string.Format(
               "INSERT INTO DocumentTypes ({0}) VALUES ({1});" +
               "SELECT last_insert_rowid();",
               DocumentTypeName.Name, DocumentTypeName.ParameterName
               );

         using (DbCommand command = createCommand(insertDocumentTypeQuery))
         {
            command.AddParameter(_documentType.Name, DocumentTypeName);
            _documentType.Id = Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateDocumentType()
      {
         string updateDocumentTypeTest =
            string.Format(
               "UPDATE DocumentTypes SET {0}={1} WHERE {2}={3}",
               DocumentTypeName.Name, DocumentTypeName.ParameterName,
               Id.Name, Id.ParameterName
               );
         
         using (DbCommand command = createCommand(updateDocumentTypeTest))
         {
            command.AddParameter(_documentType.Name, DocumentTypeName);
            command.AddParameter(_documentType.Id, Id);

            command.ExecuteNonQuery();
         }
      }
   }
}