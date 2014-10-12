using Buzzer.DomainModel.Models;
using NUnit.Framework;

namespace Buzzer.Tests.DomainModelTests
{
   [TestFixture]
   public class DeleteCreditInfoTests
   {
      [Test]
      public void DeleteCreditInfo()
      {
         // Arrange.
         CreditInfo credit = CreditInfo.CreatNew();

         // Act.
         credit.Delete();

         // Assert.
         Assert.AreEqual(RowState.Deleted, credit.RowState);
      }
   }
}