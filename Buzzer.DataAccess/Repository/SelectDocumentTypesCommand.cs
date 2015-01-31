using System;
using System.Data;
using System.Data.Common;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   internal class SelectDocumentTypesCommand : CommandBase
   {
      public SelectDocumentTypesCommand(DbConnection connection, DbTransaction transaction)
         : base(connection, transaction)
      {
      }

      public DocumentType[] Execute()
      {
         using (DataTable documentTypesTable = selectDocumentTypes())
            return createDocumentTypes(documentTypesTable);
      }

      private DataTable selectDocumentTypes()
      {
         const string selectDocumentTypesQuery = "SELECT * FROM DocumentTypes;";
         
         using (DbCommand command = createCommand(selectDocumentTypesQuery))
         {
            var documentTypes = new DataTable();

            using (DbDataReader reader = command.ExecuteReader())
               documentTypes.Load(reader);

            return documentTypes;
         }
      }

      private DocumentType[] createDocumentTypes(DataTable documentTypesTable)
      {
         DataRowCollection rows = documentTypesTable.Rows;
         var documentTypes = new DocumentType[rows.Count];

         for (int i = 0; i < documentTypes.Length; i++)
         {
            DataRow row = rows[i];

            documentTypes[i] =
               DocumentType.Create(
                  Convert.ToInt32(row[Id.Name]),
                  Convert.ToString(row[DocumentTypeName.Name])
                  );
         }

         return documentTypes;
      }
   }
}