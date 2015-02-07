using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class RequiredCreditDocumentsTests
   {
      private BuzzerDatabase _buzzerDatabase;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _buzzerDatabase = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SelectAllRequiredCreditDocumentsTest()
      {
         RequiredCreditDocuments[] requiredCreditDocuments = _buzzerDatabase.GetAllRequiredCreditDocuments();
         
         Assert.IsNotNull(requiredCreditDocuments);
         assertContainsRequiredCreditDocument("CT1", new[] {"DT1"}, requiredCreditDocuments);
         assertContainsRequiredCreditDocument("CT2", new[] {"DT1", "DT2"}, requiredCreditDocuments);
      }

      [Test]
      public void SaveNewRequiredCreditDocumentsTest()
      {
         RequiredCreditDocuments requiredCreditDocuments =
            RequiredCreditDocuments.Create(getCreditTypeByName("CT3"), new DocumentType[0]);
         requiredCreditDocuments.AddDocumentType(getDocumentTypeByName("DT2"));

         _buzzerDatabase.SaveRequiredCreditDocuments(requiredCreditDocuments);

         RequiredCreditDocuments requiredCreditDocumentsFromDb =
            getRequiredCreditDocumentsByCreditTypeName("CT3");
         assertAreEqual(requiredCreditDocuments, requiredCreditDocumentsFromDb);
      }

      [Test]
      public void SaveUpdatedRequiredCreditDocumentsTest()
      {
         RequiredCreditDocuments requiredCreditDocuments =
            getRequiredCreditDocumentsByCreditTypeName("CT4");
         requiredCreditDocuments.RemoveDocumentType(getDocumentTypeByName("DT2"));
         requiredCreditDocuments.AddDocumentType(getDocumentTypeByName("DT3"));

         _buzzerDatabase.SaveRequiredCreditDocuments(requiredCreditDocuments);

         RequiredCreditDocuments requiredCreditDocumentsFromDb =
            getRequiredCreditDocumentsByCreditTypeName("CT4");
         assertAreEqual(requiredCreditDocuments, requiredCreditDocumentsFromDb);
      }

      [Test]
      public void SaveDeletedRequiredCreditDocumentsTest()
      {
         RequiredCreditDocuments requiredCreditDocuments =
            getRequiredCreditDocumentsByCreditTypeName("CT5");
         requiredCreditDocuments.RemoveDocumentType(getDocumentTypeByName("DT1"));

         _buzzerDatabase.SaveRequiredCreditDocuments(requiredCreditDocuments);

         RequiredCreditDocuments requiredCreditDocumentsFromDb =
            safeGetRequiredCreditDocumentsByCreditTypeName("CT5");
         Assert.IsNull(requiredCreditDocumentsFromDb);
      }

      private void assertContainsRequiredCreditDocument(string creditType, string[] documentTypes, RequiredCreditDocuments[] requiredCreditDocuments)
      {
         RequiredCreditDocuments creditDocuments = requiredCreditDocuments.Single(item => item.CreditType.Name == creditType);
         ReadOnlyCollection<DocumentType> creditDocumentTypes = creditDocuments.DocumentTypes;

         Assert.AreEqual(documentTypes.Length, creditDocumentTypes.Count);

         foreach (string documentType in documentTypes)
            Assert.IsNotNull(creditDocumentTypes.SingleOrDefault(item => item.Name == documentType));
      }

      private CreditType getCreditTypeByName(string name)
      {
         return
            _buzzerDatabase
               .GetAllCreditTypes()
               .Single(item => item.Name == name);
      }

      private DocumentType getDocumentTypeByName(string name)
      {
         return
            _buzzerDatabase
               .GetAllDocumentTypes()
               .Single(item => item.Name == name);
      }

      private RequiredCreditDocuments getRequiredCreditDocumentsByCreditTypeName(string name)
      {
         return _buzzerDatabase
            .GetAllRequiredCreditDocuments()
            .Single(item => item.CreditType.Name == name);
      }

      private RequiredCreditDocuments safeGetRequiredCreditDocumentsByCreditTypeName(string name)
      {
         return _buzzerDatabase
            .GetAllRequiredCreditDocuments()
            .SingleOrDefault(item => item.CreditType.Name == name);
      }

      private void assertAreEqual(RequiredCreditDocuments expected, RequiredCreditDocuments actual)
      {
         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.CreditType.Id, actual.CreditType.Id);
         Assert.AreEqual(expected.DocumentTypes.Count, actual.DocumentTypes.Count);

         for (int i = 0; i < expected.DocumentTypes.Count; i++)
            Assert.AreEqual(expected.DocumentTypes[i].Id, actual.DocumentTypes[i].Id);
      }
   }
}