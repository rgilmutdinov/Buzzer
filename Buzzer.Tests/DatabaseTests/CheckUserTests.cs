using System.Collections.Generic;
using Buzzer.DataAccess.Repository;
using Buzzer.DomainModel.Services;
using NUnit.Framework;

namespace Buzzer.Tests.DatabaseTests
{
   [TestFixture]
   public class CheckUserTests
   {
      private BuzzerDatabase _buzzerDatabase;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _buzzerDatabase = new BuzzerDatabase(TestSettings.ConnectionString);
      }

      [Test]
      [TestCaseSource("TestCases")]
      public void CheckUserTest(string login, string password, bool expectedResult)
      {
         // Arrange.
         string passwordHash = CryptoService.GetHash(password);

         // Act.
         bool actualResult = _buzzerDatabase.CheckUser(login, passwordHash);

         // Assert.
         Assert.AreEqual(expectedResult, actualResult);
      }

      public TestCaseData[] TestCases
      {
         get
         {
            var testCases = new List<TestCaseData>();

            {
               var testCase =
                  new TestCaseData("User1", "123", true)
                     .SetName("CheckValidUserValidPasswordTest");
               testCases.Add(testCase);
            }
            
            {
               var testCase =
                  new TestCaseData("User3", "123", false)
                     .SetName("CheckInvalidUserValidPasswordTest");
               testCases.Add(testCase);
            }

            {
               var testCase =
                  new TestCaseData("User2", "123", false)
                     .SetName("CheckValidUserInvalidPasswordTest");
               testCases.Add(testCase);
            }

            {
               var testCase =
                  new TestCaseData("User3", "abc", false)
                     .SetName("CheckInvalidUserInvalidPasswordTest");
               testCases.Add(testCase);
            }
            
            return testCases.ToArray();
         }
      }
   }
}