using System.Data.SQLite;
using System.Windows.Forms;

namespace Notifier
{
   public partial class MainForm : Form
   {
      public MainForm()
      {
         InitializeComponent();
      }

      private void button1_Click(object sender, System.EventArgs e)
      {
         using (var connection = new SQLiteConnection(@"Data Source=.\Database\LocalDatabase.db"))
         {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
               command.CommandText = "select * from Contracts";

               using (var reader = command.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     var id = reader.GetInt64(0);
                     var name = reader.GetString(1);
                  }
               }
            }
         }
      }
   }
}
