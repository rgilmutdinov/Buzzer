using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using NLog;
using Notifier.Common;
using Notifier.Database;
using Notifier.PeriodicTasks;

namespace Notifier
{
   public static class Program
   {
      private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

      [STAThread]
      public static void Main()
      {
         AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

         var repository = new Repository();

         var contractsLoader =
            new PeriodicTask(new ContractsLoader(repository, new ContractCreator(repository),
                                                 getFolderWithContracts()),
                             getTimeout("LoadContractTimeout"));

         try
         {
            contractsLoader.Start();

            runMainForm(() => new MainForm(new ContractRepositoryDecorator(repository)));
         }
         finally
         {
            contractsLoader.Stop();
         }
      }

      private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         Logger.Fatal(e.ExceptionObject);
      }

      private static int getTimeout(string timeoutParameter)
      {
         var timeout = ConfigurationManager.AppSettings[timeoutParameter];

         if (timeout == null)
            throw new InitializeApplicationException("Не задан параметр {0}.", timeoutParameter);

         return int.Parse(timeout) * 1000;
      }

      private static DirectoryInfo getFolderWithContracts()
      {
         var folderWithContracts = ConfigurationManager.AppSettings["FolderWithContracts"];

         if (folderWithContracts == null)
            throw new InitializeApplicationException("Не задан параметр FolderWithContracts.");

         if (!Directory.Exists(folderWithContracts))
            throw new InitializeApplicationException("Папка \"{0}\" не существует.", folderWithContracts);

         return new DirectoryInfo(folderWithContracts);
      }

      private static void runMainForm(Func<Form> formCreator)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(formCreator());
      }
   }
}
