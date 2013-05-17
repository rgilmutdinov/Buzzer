using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Notifier.Common;
using Excel = Microsoft.Office.Interop.Excel;

namespace Notifier.Parser
{
   public static class ContractParser
   {
      private const string ContractNumberPattern = ".+№\\s+(\\d+-\\d+)";

      public static ContractData Parse(string filename)
      {
         Check.NotNull(filename, "filename");

         ExcelApplication excelApplication = null;

         try
         {
            excelApplication = ExcelApplication.Load(filename);

            var excelData = excelApplication.Range;
            var contractNumber = readContractNumber(excelData);
            var borrowerName = readBorrowerName(excelData);
            var exchangeRate = readExchangeRate(excelData);
            var payments = readPayments(excelData);

            return new ContractData(contractNumber, borrowerName, exchangeRate, payments);
         }
         finally
         {
            if (excelApplication != null)
               excelApplication.Close();
         }
      }

      private static string tryGetValue(string value, string message)
      {
         if (string.IsNullOrEmpty(value))
            throw new ParseContractException(message);

         return value;
      }

      private static T tryGetValue<T>(object value, string message)
      {
         if (value == null)
            throw new ParseContractException(message);

         return (T) value;
      }

      private static string readContractNumber(Excel.Range excelData)
      {
         const string message = "Не задан номер контракта. Ячейка H2.";
         string value = tryGetValue(excelData.Cells[2, 8].Text, message);
         var regex = Regex.Match(value, ContractNumberPattern);

         if (!regex.Success)
            throw new ParseContractException(message);

         return regex.Groups[1].Value;
      }

      private static string readBorrowerName(Excel.Range excelData)
      {
         const string message = "Не задано наименование заемщика. Ячейка E8.";
         return (string) tryGetValue(excelData.Cells[8, 5].Text, message);
      }

      private static decimal readExchangeRate(Excel.Range excelData)
      {
         const string message = "Не задан курс доллара США. Ячейка H10.";
         string value = tryGetValue(excelData.Cells[10, 8].Text, message);
         return Convert.ToDecimal(value);
      }

      private static PaymentData[] readPayments(Excel.Range excelData)
      {
         const int firstRow = 16;
         const string noPaymentDateMessage = "Не задана Дата погашения процентов по займу.";
         const string noPaymentAmountMessage = "Не задано значение Всего к оплате в долларах США.";

         var result = new List<PaymentData>();
         var row = firstRow;

         while (excelData.Cells[row, 1].Value2 != null)
         {
            double seconds = tryGetValue<double>(excelData.Cells[row, 2].Value2,
                                                 noPaymentDateMessage + " Ячейка B" + row + ".");
            var date = DateTime.FromOADate(seconds).Date;
            string amount = tryGetValue(excelData.Cells[row, 11].Text,
                                        noPaymentAmountMessage + " Ячейка K" + row + ".");
            var isNotified = date < DateTime.Today;

            result.Add(new PaymentData(Convert.ToDecimal(amount), date, isNotified));
            row++;
         }

         return result.ToArray();
      }
   }
}