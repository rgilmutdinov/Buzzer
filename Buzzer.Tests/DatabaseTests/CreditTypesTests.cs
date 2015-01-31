using System.Linq;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class CreditTypesTests
   {
      private BuzzerDatabase _buzzerDatabase;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _buzzerDatabase = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      public void SelectAllCreditTypesTest()
      {
         CreditType[] creditTypes = _buzzerDatabase.GetAllCreditTypes();

         Assert.IsNotNull(creditTypes);

         assertContainsCreditType("Credit type 1", creditTypes);
         assertContainsCreditType("Credit type 2", creditTypes);
      }

      [Test]
      public void SaveNewCreditTypeTest()
      {
         CreditType creditType = CreditType.CreateNew();
         creditType.Name = "New credit type";

         _buzzerDatabase.SaveCreditType(creditType);

         CreditType creditTypeFromDb = getCreditTypeById(creditType.Id);
         assertCreditTypesAreEqual(creditType, creditTypeFromDb);
      }

      [Test]
      public void SaveUpdatedCreditTypeTest()
      {
         CreditType creditType = getCreditTypeByName("Credit type to update");
         creditType.Name = "Updated credit type";

         _buzzerDatabase.SaveCreditType(creditType);

         CreditType creditTypeFromDb = getCreditTypeById(creditType.Id);
         assertCreditTypesAreEqual(creditType, creditTypeFromDb);
      }
      
      private void assertContainsCreditType(string creditType, CreditType[] creditTypes)
      {
         Assert.IsNotNull(creditTypes.SingleOrDefault(item => item.Name == creditType));
      }

      private CreditType getCreditTypeById(int id)
      {
         return
            _buzzerDatabase
               .GetAllCreditTypes()
               .Single(item => item.Id == id);
      }

      private void assertCreditTypesAreEqual(CreditType expectedCreditType, CreditType actualCreditType)
      {
         Assert.IsNotNull(expectedCreditType);
         Assert.IsNotNull(actualCreditType);

         Assert.AreEqual(expectedCreditType.Id, actualCreditType.Id);
         Assert.AreEqual(expectedCreditType.Name, actualCreditType.Name);
      }

      private CreditType getCreditTypeByName(string name)
      {
         return
            _buzzerDatabase
               .GetAllCreditTypes()
               .Single(item => item.Name == name);
      }
   }
}