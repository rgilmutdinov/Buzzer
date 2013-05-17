using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;
using Notifier.Common;
using Notifier.Database;
using Notifier.Parser;

namespace Notifier.PeriodicTasks
{
   public sealed class ContractsLoader : IPeriodicAction
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      private const string ContractNumberPattern = "(\\d+-\\d+).+";
      private const string ExcelFileSeachPattern = "*.xls?";

      private static readonly object Locker = new object();

      private readonly Repository _repository;
      private readonly ContractCreator _contractCreator;
      private readonly DirectoryInfo _folder;

      private List<string> _contractNumbers;
      private readonly List<DirectoryInfo> _directories;

      public ContractsLoader(
         Repository repository,
         ContractCreator contractCreator,
         DirectoryInfo folder)
      {
         Check.NotNull(repository, "repository");
         Check.NotNull(contractCreator, "contractCreator");
         Check.NotNull(folder, "folder");

         _repository = repository;
         _contractCreator = contractCreator;
         _folder = folder;

         _directories = new List<DirectoryInfo>();
      }

      public void Execute()
      {
         if (Monitor.TryEnter(Locker))
         {
            string filename = null;

            try
            {
               if (_contractNumbers == null)
                  _contractNumbers = _repository.GetContracts().Select(item => item.ContractNumber).ToList();

               filename = getNextFile();

               if (filename == null)
                  return;

               var data = ContractParser.Parse(filename);
               Logger.Info("File \"{0}\" parsed successfully.", filename);

               var contract = getContract(data);

               _repository.SaveContract(contract);
               _contractNumbers.Add(contract.ContractNumber);

               Logger.Info("The contract \"{0}\" from the file \"{1}\" was saved in the databse.",
                           contract.ContractNumber, filename);
            }
            catch (ParseContractException e)
            {
               var message = string.Format("Error occured while parsing file \"{0}\".", filename);
               Logger.Info(message);
               Logger.Error(message);
               Logger.Error(e);
            }
            catch (ContractNumberIsNotUniqueException e)
            {
               var message =
                  string.Format(
                     "Unable to save the contract \"{0}\" from the file \"{1}\". " +
                     "There is a contract with the same number in the database.",
                     e.ContractNumber, filename
                     );
               Logger.Info(message);
               Logger.Error(message);
               Logger.Error(e);
            }
            catch (Exception e)
            {
               Logger.Error(e);
            }
            finally
            {
               Monitor.Exit(Locker);
            }
         }
      }

      private string getNextFile()
      {
         if (_directories.Count == 0)
         {
            var directories = _folder.GetDirectories();

            foreach (var item in directories)
            {
               var regex = Regex.Match(item.Name, ContractNumberPattern);

               if (regex.Success)
               {
                  var isContractLoaded = _contractNumbers.Any(number => number == regex.Groups[1].Value);

                  if (!isContractLoaded)
                     _directories.Add(item);
               }
            }
         }

         var directory = _directories.FirstOrDefault();

         if (directory == null)
            return null;

         _directories.Remove(directory);

         var files = directory.GetFiles(ExcelFileSeachPattern);

         if (files.Length == 0)
         {
            Logger.Info("There is no excel files in the \"{0}\" directory.", directory.FullName);
            return null;
         }

         if (files.Length == 1)
         {
            var filename = files[0].FullName;
            Logger.Info("File for processing: \"{0}\".", filename);
            return filename;
         }

         Logger.Info("There are several excel files in the \"{0}\" directory.", directory.FullName);
         return null;
      }

      private Contract getContract(ContractData data)
      {
         var payments = new Payment[data.PaymentsData.Length];

         for (var i = 0; i < data.PaymentsData.Length; i++)
         {
            payments[i] = new Payment(data.PaymentsData[i].Date,
                                      data.PaymentsData[i].Amount,
                                      data.PaymentsData[i].IsNotified);
         }

         return _contractCreator.Create(data.ContractNumber, data.BorrowerName,
                                        data.ExchangeRate, null, payments);
      }
   }
}
