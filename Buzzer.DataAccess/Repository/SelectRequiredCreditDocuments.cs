using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Buzzer.DomainModel.Models;

namespace Buzzer.DataAccess.Repository
{
   internal class SelectRequiredCreditDocuments : CommandBase
   {
      public SelectRequiredCreditDocuments(DbConnection connection, DbTransaction transaction)
         : base(connection,  transaction)
      {
      }

      public RequiredCreditDocuments[] Execute()
      {
         using (DataTable requiredCreditDocumentsTable = selectRequiredCreditDocuments())
            return createRequiredCreditDocuments(requiredCreditDocumentsTable);
      }

      private DataTable selectRequiredCreditDocuments()
      {
         const string selectRequiredCreditDocumentsQuery = "SELECT * FROM RequiredCreditDocuments;";

         using (DbCommand command = createCommand(selectRequiredCreditDocumentsQuery))
         using (DbDataReader reader = command.ExecuteReader())
         {
            var requiredCreditDocuments = new DataTable();
            requiredCreditDocuments.Load(reader);
            return requiredCreditDocuments;
         }
      }

      private RequiredCreditDocuments[] createRequiredCreditDocuments(DataTable dataTable)
      {
         CreditType[] creditTypes = selectCreditTypes();
         DocumentType[] documentTypes = selectDocumentTypes();

         DataRowCollection rows = dataTable.Rows;
         var creditDocuments = new Dictionary<int, List<int>>();

         for (int i = 0; i < rows.Count; i++)
         {
            DataRow row = rows[i];
            int creditTypeId = Convert.ToInt32(row[CreditTypeId.Name]);
            int documentTypeId = Convert.ToInt32(row[DocumentTypeId.Name]);

            if (creditDocuments.ContainsKey(creditTypeId))
               creditDocuments[creditTypeId].Add(documentTypeId);
            else
               creditDocuments.Add(creditTypeId, new List<int> {documentTypeId});
         }

         return
            creditDocuments
               .Select(item => RequiredCreditDocuments.Create(getCreditTypeById(creditTypes, item.Key),
                                                              getDocumentTypes(documentTypes, item.Value)))
               .ToArray();
      }
      
      private CreditType[] selectCreditTypes()
      {
         var selectQuery = new SelectCreditTypesCommand(Connection, Transaction);
         return selectQuery.Execute();
      }

      private DocumentType[] selectDocumentTypes()
      {
         var selectQuery = new SelectDocumentTypesCommand(Connection, Transaction);
         return selectQuery.Execute();
      }

      private CreditType getCreditTypeById(CreditType[] creditTypes, int creditTypeId)
      {
         return creditTypes.Single(item => item.Id == creditTypeId);
      }

      private List<DocumentType> getDocumentTypes(DocumentType[] documentTypes, List<int> documentTypeIds)
      {
         return documentTypeIds.Select(id => documentTypes.Single(type => type.Id == id)).ToList();
      }
   }
}