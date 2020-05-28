using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_V_2
{
    public partial class InicioResumen : Form
    {
        public InicioResumen()
        {
            try
            {
                InitializeComponent();
                lblupdat.Text = "Last update date - " + File.ReadAllLines(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/DBlog.txt").Last();
                progressBar1.Visible = false;
            }
            catch
            {

            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            dbUpdate();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            dbUpdate();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            del();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            del();
        }

        private void dbUpdate()
        {
            try
            {
                progressBar1.Visible = true;
                SqlConnection source = new SqlConnection(@"remote db connection sting");
                SqlConnection destination = new SqlConnection(@"Data Source=localhost;Initial Catalog=Analyzer;User Id=sa;Password=1234");
                source.Open();
                destination.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM DLL_referance", destination);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("SELECT * FROM DLL_referance", source);
                SqlDataReader reader = cmd.ExecuteReader();
                SqlBulkCopy bulkData = new SqlBulkCopy(destination);
                bulkData.DestinationTableName = "DLL_referance";
                bulkData.WriteToServer(reader);
                bulkData.Close();
                destination.Close();
                source.Close();

                source.Open();
                destination.Open();
                SqlCommand cmd2 = new SqlCommand("DELETE FROM tbl_users", destination);
                cmd2.ExecuteNonQuery();
                cmd2 = new SqlCommand("SELECT * FROM tbl_users", source);
                SqlDataReader reader2 = cmd2.ExecuteReader();
                SqlBulkCopy bulkData2 = new SqlBulkCopy(destination);
                bulkData2.DestinationTableName = "tbl_users";
                bulkData2.WriteToServer(reader2);
                bulkData2.Close();

                destination.Close();
                source.Close();

                File.AppendAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/DBlog.txt", "\n"+DateTime.Now.ToString("dd-MM-yyyy"));
                lblupdat.Text = "Last update date - " + File.ReadAllLines(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/DBlog.txt").Last();
                progressBar1.Value = 100;
                MessageBox.Show("Siganture base updated", "Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception k)
            {
                MessageBox.Show("DB Error", "Analyzer");
            }
        }

        private void del()
        {
            if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Samples/").ToList().ForEach(File.Delete);
                    Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Sections/").ToList().ForEach(File.Delete);
                    Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Strings/").ToList().ForEach(File.Delete);
                    Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Win32APIs/").ToList().ForEach(File.Delete);
                    Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Pdf/").ToList().ForEach(File.Delete);
                    MessageBox.Show("History cleared", "Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception k)
                {
                    MessageBox.Show("Path no found", "Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
                      
        }
    }
}
