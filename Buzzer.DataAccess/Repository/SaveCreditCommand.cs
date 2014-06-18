using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using Buzzer.DataAccess.Helpers;
using Buzzer.DomainModel.Models;
using Common;

namespace Buzzer.DataAccess.Repository
{
   internal class SaveCreditCommand : CommandBase
   {
      private readonly CreditInfo _credit;

      public SaveCreditCommand(DbConnection connection, DbTransaction transaction, CreditInfo credit)
         : base(connection, transaction)
      {
         Check.NotNull(credit, "credit");
         _credit = credit;
      }

      public void Execute()
      {
         if (_credit.IsNew)
            saveNewCredit();
         else
            saveEditedCredit();
      }

      private void saveNewCredit()
      {
         int creditId = insertCredit(_credit);
         InsertPersonResult insertBorrowerResult = insertPerson(_credit.Borrower, creditId);
         ReadOnlyCollection<PersonInfo> guarantors = _credit.Guarantors;
         var insertGuarantorsResult = new InsertPersonResult[guarantors.Count];

         for (int i = 0; i < guarantors.Count; i++)
            insertGuarantorsResult[i] = insertPerson(guarantors[i], creditId);

         PaymentInfo[] paymentsSchedule = _credit.PaymentsSchedule;
         int[] paymentsIds = insertPayments(paymentsSchedule, creditId);

         _credit.Id = creditId;
         fillPersonIds(_credit.Borrower, insertBorrowerResult, creditId);

         for (int i = 0; i < guarantors.Count; i++)
            fillPersonIds(guarantors[i], insertGuarantorsResult[i], creditId);

         fillPaymentsIds(paymentsIds);
      }

      private void saveEditedCredit()
      {
         var command = new SelectCreditsCommand(Connection, Transaction, "ID = " + _credit.Id);
         CreditInfo original = command.Execute().Single();

         updateCredit(_credit);
         updatePerson(_credit.Borrower);
         mergePhoneNumbers(_credit.Borrower.PhoneNumbers, original.Borrower.PhoneNumbers);

         var result = new List<InsertPersonResult>();

         foreach (var guarantor in _credit.Guarantors)
         {
            if (guarantor.IsNew)
            {
               result.Add(insertPerson(guarantor, _credit.Id));
            }
            else
            {
               var originalGuarantor = original.Guarantors.First(item => item.Id == guarantor.Id);
               updatePerson(guarantor);
               mergePhoneNumbers(guarantor.PhoneNumbers, originalGuarantor.PhoneNumbers);
            }
         }

         List<PersonInfo> deletedGuarantors =
            original.Guarantors.Where(o => _credit.Guarantors.All(c => c.Id != o.Id)).ToList();
         deletedGuarantors.ForEach(
            item =>
               {
                  foreach (var phone in item.PhoneNumbers)
                     deletePhoneNumber(phone.Id);
                  deletePerson(item.Id);
               }
            );

         PaymentInfo[] payments = _credit.PaymentsSchedule;

         if (payments.Length > 0 && payments[0].IsNew)
         {
            foreach (PaymentInfo payment in original.PaymentsSchedule)
               deletePaymentInfo(payment.Id);

            int[] paymentsIds = insertPayments(payments, _credit.Id);
            fillPaymentsIds(paymentsIds);
         }

         int i = 0;

         foreach (var guarantor in _credit.Guarantors)
         {
            if (guarantor.IsNew)
            {
               fillPersonIds(guarantor, result[i++], _credit.Id);
            }
         }
      }

      private InsertPersonResult insertPerson(PersonInfo person, int creditId)
      {
         int personId = insertPersonQuery(person, creditId);
         ReadOnlyCollection<PhoneNumberInfo> phoneNumbers = person.PhoneNumbers;
         var phoneNumbersIds = new int[phoneNumbers.Count];

         for (int i = 0; i < phoneNumbers.Count; i++)
            phoneNumbersIds[i] = insertPhoneNumber(phoneNumbers[i], personId);

         return new InsertPersonResult(personId, phoneNumbersIds);
      }

      private void fillPersonIds(PersonInfo person, InsertPersonResult insertPersonResult, int creditId)
      {
         person.Id = insertPersonResult.PersonId;
         person.CreditId = creditId;

         for (int i = 0; i < person.PhoneNumbers.Count; i++)
         {
            PhoneNumberInfo phoneNumber = person.PhoneNumbers[i];
            phoneNumber.Id = insertPersonResult.PhoneNumberIds[i];
            phoneNumber.PersonId = person.Id;
         }
      }

      [Todo("Сделать откат ID inserted номеров телефонов, если транзакция упадет")]
      private void mergePhoneNumbers(IList<PhoneNumberInfo> current, IList<PhoneNumberInfo> original)
      {
         var insertedPhones = current.Where(c => c.IsNew).ToList();
         var updatedPhones = current.Where(c => original.Any(o => o.Id == c.Id)).ToList();
         var deletedPhones = original.Where(o => current.All(c => c.Id != o.Id)).ToList();

         insertedPhones.ForEach(item =>
                                   {
                                      int id = insertPhoneNumber(item, item.PersonId);
                                      item.Id = id;
                                   });
         updatedPhones.ForEach(updatePhoneNumber);
         deletedPhones.ForEach(item => deletePhoneNumber(item.Id));
      }

      private int[] insertPayments(PaymentInfo[] paymentsSchedule, int creditId)
      {
         var paymentsIds = new int[paymentsSchedule.Length];

         for (int i = 0; i < paymentsSchedule.Length; i++)
            paymentsIds[i] = insertPaymentInfo(paymentsSchedule[i], creditId);

         return paymentsIds;
      }
      
      private void fillPaymentsIds(int[] paymentsIds)
      {
         for (int i = 0; i < paymentsIds.Length; i++)
            _credit.PaymentsSchedule[i].Id = paymentsIds[i];
      }

      #region Credits

      private int insertCredit(CreditInfo creditInfo)
      {
         string insertCreditQuery =
            string.Format(
               "INSERT INTO Credits ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}) " +
               "VALUES ({10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19});" +
               "SELECT last_insert_rowid();",

               CreditNumber.Name, ApplicationDate.Name, ProtocolDate.Name,
               CreditAmount.Name, CreditIssueDate.Name, MonthsCount.Name,
               DiscountRate.Name, EffectiveDiscountRate.Name, ExchangeRate.Name,
               CreditState.Name,

               CreditNumber.ParameterName, ApplicationDate.ParameterName, ProtocolDate.ParameterName,
               CreditAmount.ParameterName, CreditIssueDate.ParameterName, MonthsCount.ParameterName,
               DiscountRate.ParameterName, EffectiveDiscountRate.ParameterName, ExchangeRate.ParameterName,
               CreditState.ParameterName
               );

         using (DbCommand command = createCommand(insertCreditQuery))
         {
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.ApplicationDate, ApplicationDate);
            command.AddParameter(creditInfo.ProtocolDate, ProtocolDate);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            command.AddParameter((int) creditInfo.CreditState, CreditState);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updateCredit(CreditInfo creditInfo)
      {
         string updateCreditQuery =
            string.Format(
               "UPDATE Credits SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13}, {14}={15}, {16}={17}, {18}={19} " +
               "WHERE {20}={21};",
               CreditNumber.Name, CreditNumber.ParameterName,
               ApplicationDate.Name, ApplicationDate.ParameterName,
               ProtocolDate.Name, ProtocolDate.ParameterName,
               CreditAmount.Name, CreditAmount.ParameterName,
               CreditIssueDate.Name, CreditIssueDate.ParameterName,
               MonthsCount.Name, MonthsCount.ParameterName,
               DiscountRate.Name, DiscountRate.ParameterName,
               EffectiveDiscountRate.Name, EffectiveDiscountRate.ParameterName,
               ExchangeRate.Name, ExchangeRate.ParameterName,
               CreditState.Name, CreditState.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updateCreditQuery))
         {
            command.AddParameter(creditInfo.CreditNumber, CreditNumber);
            command.AddParameter(creditInfo.ApplicationDate, ApplicationDate);
            command.AddParameter(creditInfo.ProtocolDate, ProtocolDate);
            command.AddParameter(creditInfo.CreditAmount, CreditAmount);
            command.AddParameter(creditInfo.CreditIssueDate, CreditIssueDate);
            command.AddParameter(creditInfo.MonthsCount, MonthsCount);
            command.AddParameter(creditInfo.DiscountRate, DiscountRate);
            command.AddParameter(creditInfo.EffectiveDiscountRate, EffectiveDiscountRate);
            command.AddParameter(creditInfo.ExchangeRate, ExchangeRate);
            command.AddParameter((int) creditInfo.CreditState, CreditState);
            command.AddParameter(creditInfo.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region Persons

      private int insertPersonQuery(PersonInfo personInfo, int creditId)
      {
         string insertPersonQuery =
            string.Format(
               "INSERT INTO Persons ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) VALUES ({9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, PersonalNumber.Name, Name.Name, RegistrationAddress.Name, FactAddress.Name,
               PassportNumber.Name, PassportIssueDate.Name, PassportIssuer.Name, PersonType.Name,

               CreditId.ParameterName, PersonalNumber.ParameterName, Name.ParameterName,
               RegistrationAddress.ParameterName, FactAddress.ParameterName,
               PassportNumber.ParameterName, PassportIssueDate.ParameterName, PassportIssuer.ParameterName,
               PersonType.ParameterName
               );

         using (DbCommand command = createCommand(insertPersonQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, PersonType);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updatePerson(PersonInfo personInfo)
      {
         var updatePersonQuery =
            string.Format(
               "UPDATE Persons SET {0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}, {12}={13}, {14}={15} WHERE {16}={17};",
               PersonalNumber.Name, PersonalNumber.ParameterName,
               Name.Name, Name.ParameterName,
               RegistrationAddress.Name, RegistrationAddress.ParameterName,
               FactAddress.Name, FactAddress.ParameterName,
               PassportNumber.Name, PassportNumber.ParameterName,
               PassportIssueDate.Name, PassportIssueDate.ParameterName,
               PassportIssuer.Name, PassportIssuer.ParameterName,
               PersonType.Name, PersonType.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (var command = createCommand(updatePersonQuery))
         {
            command.AddParameter(personInfo.PersonalNumber, PersonalNumber);
            command.AddParameter(personInfo.PersonName, Name);
            command.AddParameter(personInfo.RegistrationAddress, RegistrationAddress);
            command.AddParameter(personInfo.FactAddress, FactAddress);
            command.AddParameter(personInfo.PassportNumber, PassportNumber);
            command.AddParameter(personInfo.PassportIssueDate, PassportIssueDate);
            command.AddParameter(personInfo.PassportIssuer, PassportIssuer);
            command.AddParameter(personInfo.IsBorrower, PersonType);
            command.AddParameter(personInfo.Id, Id);

            command.ExecuteNonQuery();
         }
      }

      private void deletePerson(int personId)
      {
         string deletePersonQuery =
            string.Format(
               "DELETE FROM Persons WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (var command = createCommand(deletePersonQuery))
         {
            command.AddParameter(personId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region PhoneNumbers

      private int insertPhoneNumber(PhoneNumberInfo phoneNumber, int personId)
      {
         string insertPhoneNumberQuery =
            string.Format(
               "INSERT INTO PhoneNumbers ({0}, {1}) VALUES ({2}, {3});" +
               "SELECT last_insert_rowid();",
               PersonId.Name, PhoneNumber.Name,
               PersonId.ParameterName, PhoneNumber.ParameterName
               );

         using (DbCommand command = createCommand(insertPhoneNumberQuery))
         {
            command.AddParameter(personId, PersonId);
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void updatePhoneNumber(PhoneNumberInfo phoneNumber)
      {
         string updatePhoneNumberQuery =
            string.Format(
               "UPDATE PhoneNumbers SET {0}={1} WHERE {2}={3};",
               PhoneNumber.Name, PhoneNumber.ParameterName,
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(updatePhoneNumberQuery))
         {
            command.AddParameter(phoneNumber.PhoneNumber, PhoneNumber);
            command.AddParameter(phoneNumber.Id, Id);
            command.ExecuteNonQuery();
         }
      }

      private void deletePhoneNumber(int phoneNumberId)
      {
         string deletePhoneNumberQuery =
            string.Format(
               "DELETE FROM PhoneNumbers WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(deletePhoneNumberQuery))
         {
            command.AddParameter(phoneNumberId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      #region PaymentsSchedule

      private int insertPaymentInfo(PaymentInfo paymentInfo, int creditId)
      {
         string insertPaymentInfoQuery =
            string.Format(
               "INSERT INTO PaymentsSchedule ({0}, {1}, {2}, {3}) VALUES ({4}, {5}, {6}, {7});" +
               "SELECT last_insert_rowid();",

               CreditId.Name, PaymentAmount.Name,
               PaymentDate.Name, IsNotified.Name,

               CreditId.ParameterName, PaymentAmount.ParameterName,
               PaymentDate.ParameterName, IsNotified.ParameterName
               );

         using (DbCommand command = createCommand(insertPaymentInfoQuery))
         {
            command.AddParameter(creditId, CreditId);
            command.AddParameter(paymentInfo.PaymentAmount, PaymentAmount);
            command.AddParameter(paymentInfo.PaymentDate, PaymentDate);
            command.AddParameter(paymentInfo.IsNotified, IsNotified);

            return Convert.ToInt32(command.ExecuteScalar());
         }
      }

      private void deletePaymentInfo(int paymentId)
      {
         string deletePaymentInfoQuery =
            string.Format(
               "DELETE FROM PaymentsSchedule WHERE {0}={1};",
               Id.Name, Id.ParameterName
               );

         using (DbCommand command = createCommand(deletePaymentInfoQuery))
         {
            command.AddParameter(paymentId, Id);
            command.ExecuteNonQuery();
         }
      }

      #endregion

      private sealed class InsertPersonResult
      {
         public InsertPersonResult(int personId, int[] phoneNumberIds)
         {
            PersonId = personId;
            PhoneNumberIds = phoneNumberIds;
         }

         public int PersonId { get; private set; }
         public int[] PhoneNumberIds { get; private set; }
      }
   }
}