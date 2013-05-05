using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Notifier.Common;
using Excel = Microsoft.Office.Interop.Excel;

namespace Notifier.Parser
{
   public sealed class ContractParser
   {
      private readonly string _filename;

      public ContractParser(string filename)
      {
         Check.NotNull(filename, "filename");
         _filename = filename;
      }

      public ContractData Parse()
      {
         ExcelApplication excelApplication = null;

         try
         {
            excelApplication = ExcelApplication.Load(_filename);

            var excelData = excelApplication.Range;
            var contractNumber = readContracNumber(excelData);
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

      private static string readContracNumber(Excel.Range excelData)
      {
         const string message = "Не задан номер контракта. Ячейка H2.";
         string value = tryGetValue(excelData.Cells[2, 8].Text, message);
         var regex = Regex.Match(value, ".+№\\s+(\\d+-\\d+)");

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

      private static Payment[] readPayments(Excel.Range excelData)
      {
         const string noPaymentDateMessage = "Не задана Дата погашения процентов по займу.";
         const string noPaymentAmountMessage = "Не задано значение Всего к оплате в долларах США.";

         var result = new List<Payment>();
         var row = 16;

         while (excelData.Cells[row, 1].Value2 != null)
         {
            double seconds = tryGetValue<double>(excelData.Cells[row, 2].Value2,
                                                 noPaymentDateMessage + " Ячейка B" + row + ".");
            var date = DateTime.FromOADate(seconds);
            string amount = tryGetValue(excelData.Cells[row, 11].Text,
                                        noPaymentAmountMessage + " Ячейка K" + row + ".");
            result.Add(new Payment(Convert.ToDecimal(amount), date));
            row++;
         }

         return result.ToArray();
      }
   }
}