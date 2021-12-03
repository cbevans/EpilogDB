using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using Microsoft.Data.Sqlite;

namespace EpilogDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdGetDbFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    lblDbFileName.Content = ofd.FileName;
                    System.IO.FileInfo fi = new System.IO.FileInfo(ofd.FileName);
                    lblOldFileSize.Content = "Pre-Cleaned File Size: " + Math.Floor((double)(fi.Length / 1000)) + " kB";
                } catch (Exception)
                {
                    lblDbFileName.Content = "File Error - Please Try Again";
                }
            }
        }

        private void cmdClean_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.cmdClean.IsEnabled = false;
                this.CleanDb(lblDbFileName.Content.ToString());
            } catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            } finally
            {
                this.cmdClean.IsEnabled = true;
            }
        }

        public void CleanDb(string fileName)
        {
            if (fileName == "") { throw new Exception("File Cannot Be Blank");  }
            if (! System.IO.File.Exists(fileName)) { throw new Exception("Invalid File - File Does Not Exist");  }

            using (var connection = new SqliteConnection("Data Source=" + fileName))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // Make sure DB is proper format with Jobs and JobData tables and has entries in both tables
                try
                {
                    command.CommandText = "SELECT count(*) from Jobs;";
                    Int64 jobs = Convert.ToInt64(command.ExecuteScalar());

                    command.CommandText = "SELECT count(*) from JobData;";
                    Int64 JobData = Convert.ToInt64(command.ExecuteScalar());

                    if (jobs < 1 || JobData < 1)
                    {
                        throw new Exception("Database Has No Jobs or No Job Data - Is This a Valid Epilog Database?");
                    }
                } catch (Exception)
                {
                    // Invalid DB or doesn't have needed tables
                    throw new Exception("Invalid Database File - Jobs and JobData Tables Must Exist");
                }

                // Query and report number of records that will be removed
                try
                {
                    command.CommandText = "SELECT COUNT(*) FROM JobData WHERE id NOT IN (SELECT JobDataID from Jobs);";
                    Int64 NumRecordsToDelete = Convert.ToInt64(command.ExecuteScalar());
                    if (System.Windows.MessageBox.Show(NumRecordsToDelete.ToString() + " Records Found - These Records Will Be Deleted - Continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }
                    if (System.Windows.MessageBox.Show("Continuing will permanently delete records from your database - they cannot be recovered - continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No) { return; }

                    command.CommandText = "DELETE FROM JobData WHERE id NOT IN (SELECT JobDataID from Jobs);";
                    int RecordsDeleted = command.ExecuteNonQuery();

                    command.CommandText = "VACUUM";
                    int VaccuumedRecords = command.ExecuteNonQuery();

                    System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
                    this.lblNewFileSize.Content = "Cleaned File Size: " + Math.Floor((double)fi.Length / 1000) + " kB";

                    System.Windows.MessageBox.Show("Done!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                } catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

        }
    }
}
