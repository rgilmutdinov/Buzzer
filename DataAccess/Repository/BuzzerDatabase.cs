using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Common;
using DataAccess.Model;

namespace DataAccess.Repository
{
   public sealed class BuzzerDatabase
   {
      private readonly CreditsRepository _credits;
      private readonly PersonsRepository _persons;
      private readonly PhoneNumbersRepository _phoneNumbers;

      public BuzzerDatabase(string connectionString)
      {
         Check.NotNull(connectionString, "connectionString");

         _credits = new CreditsRepository(connectionString);
         _persons = new PersonsRepository(connectionString);
         _phoneNumbers = new PhoneNumbersRepository(connectionString);
      }

      public CreditInfo[] GetAllCredits()
      {
         var credits = _credits.SelectAll();

         foreach (var credit in credits)
            credit.Context = this;

         return credits;
      }

      public void SaveCredit(CreditInfo credit)
      {
         using (var transaction = new TransactionScope())
         {
            saveCredit(credit);
            transaction.Complete();
         }
      }
      
      public void DeleteCredit(CreditInfo credit)
      {
         using (var transaction = new TransactionScope())
         {
            var original = getCreditById(credit.Id);

            foreach (var phone in original.Borrower.PhoneNumbers)
               _phoneNumbers.Delete(phone);
            _persons.Delete(original.Borrower);

            foreach (var guarantor in original.Guarantors)
            {
               foreach (var phone in guarantor.PhoneNumbers)
                  _phoneNumbers.Delete(phone);
               _persons.Delete(guarantor);
            }

            _credits.Delete(original);
            transaction.Complete();
         }
      }

      internal PersonInfo[] GetPersons(int creditId)
      {
         var condition = string.Format("{0}={1}", PersonsRepository.CreditId.Name, creditId);
         var persons = _persons.Select(condition);

         foreach (var person in persons)
            person.Context = this;

         return persons;
      }

      internal PhoneNumberInfo[] GetPhoneNumbers(int personId)
      {
         var condition = string.Format("{0}={1}", PhoneNumbersRepository.PersonId.Name, personId);
         var phoneNumbers = _phoneNumbers.Select(condition);

         foreach (var phone in phoneNumbers)
            phone.Context = this;

         return phoneNumbers;
      }

      private CreditInfo getCreditById(int id)
      {
         var credit = _credits.Select(id);

         if (credit == null)
            return null;

         credit.Context = this;
         return credit;
      }
      
      [Todo("Сделать откат Id при возникновении исключения.")]
      private void saveCredit(CreditInfo credit)
      {
         var rollbackActions = new List<Action>();

         try
         {
            if (credit.IsNew)
            {
               _credits.Insert(credit);

               credit.Borrower.CreditId = credit.Id;
               _persons.Insert(credit.Borrower);

               foreach (var phone in credit.Borrower.PhoneNumbers)
               {
                  phone.PersonId = credit.Borrower.Id;
                  _phoneNumbers.Insert(phone);
               }

               foreach (var guarantor in credit.Guarantors)
               {
                  guarantor.CreditId = credit.Id;
                  _persons.Insert(guarantor);

                  foreach (var phone in guarantor.PhoneNumbers)
                  {
                     phone.PersonId = guarantor.Id;
                     _phoneNumbers.Insert(phone);
                  }
               }
            }
            else
            {
               var original = getCreditById(credit.Id);

               _credits.Update(credit);
               _persons.Update(credit.Borrower);
               mergePhoneNumbers(credit.Borrower.PhoneNumbers, original.Borrower.PhoneNumbers);

               foreach (var guarantor in credit.Guarantors)
               {
                  if (guarantor.IsNew)
                  {
                     _persons.Insert(guarantor);

                     foreach (var phone in guarantor.PhoneNumbers)
                     {
                        phone.PersonId = guarantor.Id;
                        _phoneNumbers.Insert(phone);
                     }
                  }
                  else
                  {
                     var originalGuarantor = original.Guarantors.First(item => item.Id == guarantor.Id);
                     _persons.Update(guarantor);
                     mergePhoneNumbers(guarantor.PhoneNumbers, originalGuarantor.PhoneNumbers);
                  }
               }

               var deletedGuarantors = original.Guarantors.Where(o => credit.Guarantors.All(c => c.Id != o.Id)).ToList();
               deletedGuarantors.ForEach(
                  item =>
                     {
                        foreach (var phone in item.PhoneNumbers)
                           _phoneNumbers.Delete(phone);
                        _persons.Delete(item);
                     }
                  );
            }
         }
         catch
         {
            rollbackActions.ForEach(action => action());
            throw;
         }
      }

      private void mergePhoneNumbers(IList<PhoneNumberInfo> current, IList<PhoneNumberInfo> original)
      {
         var insertedPhones = current.Where(c => c.IsNew).ToList();
         var updatedPhones = current.Where(c => original.Any(o => o.Id == c.Id)).ToList();
         var deletedPhones = original.Where(o => current.All(c => c.Id != o.Id)).ToList();

         insertedPhones.ForEach(item => _phoneNumbers.Insert(item));
         updatedPhones.ForEach(item => _phoneNumbers.Update(item));
         deletedPhones.ForEach(item => _phoneNumbers.Delete(item));
      }
   }
}