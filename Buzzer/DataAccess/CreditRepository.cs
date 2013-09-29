using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Transactions;
using Buzzer.Common;
using Buzzer.Model;

namespace Buzzer.DataAccess
{
   public sealed class CreditRepository
   {
      public T[] QueryDatabase<T>(Func<BuzzerDatabaseEntities, T[]> query)
      {
         using (var database = new BuzzerDatabaseEntities())
         {
            return query(database);
         }
      }

      public void InsertOrUpdate(CreditInfo creditInfo)
      {
         Check.NotNull(creditInfo, "newCredit");

         using (var transactionScope = new TransactionScope())
         {
            using (var database = new BuzzerDatabaseEntities())
            {
               database.Connection.Open();
               var postActions = new List<Action>();

               if (creditInfo.Id == NullValues.Id)
               {
                  // Insert.
                  var credit = createCreditEntity(creditInfo, database, ref postActions);
                  database.Credits.AddObject(credit);
               }
               else
               {
                  // Update.

                  var credit = createCreditEntity(creditInfo, database, ref postActions);
               }

               database.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
               transactionScope.Complete();

               postActions.ForEach(item => item());
            }
         }
      }

      private static Credit createCreditEntity(
         CreditInfo newCredit, BuzzerDatabaseEntities database, ref List<Action> postActions)
      {
         var credit = new Credit
                         {
                            CreditNumber = newCredit.CreditNumber,
                            CreditAmount = newCredit.CreditAmount,
                            CreditIssueDate = newCredit.CreditIssueDate,
                            MonthsCount = newCredit.MonthsCount,
                            DiscountRate = newCredit.DiscountRate,
                            EffectiveDiscountRate = newCredit.EffectiveDiscountRate,
                            ExchangeRate = newCredit.UsdRate
                         };
         postActions.Add(() => newCredit.Id = credit.ID);

         var borrower = createPersonEntity(newCredit.Borrower, ref postActions);
         postActions.Add(() => newCredit.Borrower.Id = borrower.ID);

         credit.PersonsToCredits.Add(
            new PersonsToCredit
               {
                  Credit = credit,
                  Person = borrower,
                  IsBorrower = true
               }
            );

         foreach (var guarantor in newCredit.Guarantors)
         {
            var person = createPersonEntity(guarantor, ref postActions);
            var currentGuarantor = guarantor;
            postActions.Add(() => currentGuarantor.Id = person.ID);

            credit.PersonsToCredits.Add(
               new PersonsToCredit
                  {
                     Credit = credit,
                     Person = person,
                     IsBorrower = false
                  }
               );
         }

         return credit;
      }

      private static Person createPersonEntity(PersonInfo newPerson, ref List<Action> postActions)
      {
         var person = new Person
                         {
                            Name = newPerson.PersonName,
                            RegistrationAddress = newPerson.RegistrationAddress,
                            FactAddress = newPerson.FactAddress,
                            PassportNumber = newPerson.PassportNumber,
                            PassportIssueDate = newPerson.PassportIssueDate,
                            PassportIssuer = newPerson.PassportIssuer
                         };

         foreach (var phoneNumberInfo in newPerson.PhoneNumbers)
         {
            var phoneNumber = createPhoneNumberEntity(phoneNumberInfo);
            phoneNumber.Person = person;

            var currentPhoneNumberInfo = phoneNumberInfo;
            postActions.Add(() => currentPhoneNumberInfo.Id = phoneNumber.ID);
         }

         return person;
      }

      private static PhoneNumber createPhoneNumberEntity(PhoneNumberInfo newPhoneNumber)
      {
         return new PhoneNumber {Number = newPhoneNumber.PhoneNumber};
      }
   }
}
