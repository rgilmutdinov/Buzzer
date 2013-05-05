using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Notifier.Common;

namespace Notifier.Parser
{
   public sealed class ExcelApplication
   {
      private Application Application { get; set; }
      private Workbook Workbook { get; set; }

      private ExcelApplication(Application application, Workbook workbook, Range range)
      {
         Check.NotNull(application, "application");
         Check.NotNull(workbook, "workbook");
         Check.NotNull(range, "range");

         Application = application;
         Workbook = workbook;
         Range = range;
      }

      public Range Range { get; private set; }

      public void Close()
      {
         Application.Quit();
         Marshal.ReleaseComObject(Workbook);
         Marshal.ReleaseComObject(Application);
         Application = null;
      }

      public static ExcelApplication Load(string filename)
      {
         var excelApplication = new Application();
         var workBook = excelApplication.Workbooks.Open(filename);
         _Worksheet workSheet = workBook.Sheets[1];
         return new ExcelApplication(excelApplication, workBook, workSheet.UsedRange);
      }
   }
}