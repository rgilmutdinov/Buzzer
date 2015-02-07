using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using Buzzer.Tests.Common;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class DocumentTypesTests
   {
      private BuzzerDatabase _buzzerDatabase;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _buzzerDatabase = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SelectAllDocumentTypesTest()
      {
         DocumentType[] documentTypes = _buzzerDatabase.GetAllDocumentTypes();

         Assert.IsNotNull(documentTypes);

         assertContainsDocumentType("Document type 1", documentTypes);
         assertContainsDocumentType("Document type 2", documentTypes);
      }

      [Test]
      public void SaveNewDocumentTypeTest()
      {
         DocumentType documentType = DocumentType.CreateNew();
         documentType.Name = "New document type";

         _buzzerDatabase.SaveDocumentType(documentType);

         DocumentType documentTypeFromDb = getDocumentTypeById(documentType.Id);
         AssertUtils.AssertDocumentTypesAreEqual(documentType, documentTypeFromDb);
      }

      [Test]
      public void SaveUpdatedDocumentTypeTest()
      {
         DocumentType documentType = getDocumentTypeByName("Document to update");
         documentType.Name = "Updated document";

         _buzzerDatabase.SaveDocumentType(documentType);

         DocumentType documentTypeFromDb = getDocumentTypeById(documentType.Id);
         AssertUtils.AssertDocumentTypesAreEqual(documentType, documentTypeFromDb);
      }

      private void assertContainsDocumentType(string documentTypeName, DocumentType[] documentTypes)
      {
         Assert.IsNotNull(documentTypes.SingleOrDefault(item => item.Name == documentTypeName));
      }

      private DocumentType getDocumentTypeById(int id)
      {
         DocumentType documentTypeFromDb =
            _buzzerDatabase
               .GetAllDocumentTypes()
               .Single(item => item.Id == id);

         return documentTypeFromDb;
      }

      private DocumentType getDocumentTypeByName(string name)
      {
         DocumentType documentType =
            _buzzerDatabase
               .GetAllDocumentTypes()
               .Single(item => item.Name == name);

         return documentType;
      }
   }
}