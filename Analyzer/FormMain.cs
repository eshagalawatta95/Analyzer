using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Analyzer;
using Analyzer.Properties;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using _Peanalyzer.Data;
using Kaitai;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Controls;

namespace GUI_V_2
{
    public partial class FormMain : Form
    {
        string textFile = "";
        string filepath = "";
        int IPratecount = 0;
        string APIKey = "";
        string filename = "";
        int mainrecord = 0;
        int APIcount = 0;
        int mallciousSection = 0;
        int index = 0;
        string vtcount = "";
        int shell = 0;
        string currentfilename = "";
        string rshellip = "";

        System.Windows.Forms.OpenFileDialog choofdlog = new System.Windows.Forms.OpenFileDialog();      
        List<string> fileslist =new List<string>();
        
        DataTable vtdt = new DataTable();
        DataTable dtip = new DataTable();
        DataTable MLdt = new DataTable();
        DataTable apidt = new DataTable();

        public FormMain(string strTextBox)
        {
            InitializeComponent();
            label4.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            lblusuario.Text = strTextBox;
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button1, "Can select Multiple file");
            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.button2, "Download report");
            tabControl1.Visible = false;

            try
            {
                APIKey = File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/APIkey.txt");
            }
            catch (Exception k)
            {
                MessageBox.Show("No API key found", "Error");
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (MenuVertical.Width == 250)
            {
                MenuVertical.Width = 70;
            }
            else
                MenuVertical.Width = 250;
        }

        private void iconcerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconmaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            iconrestaurar.Visible = true;
            iconmaximizar.Visible = false;
        }

        private void iconrestaurar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            iconrestaurar.Visible = false;
            iconmaximizar.Visible = true;
        }

        private void iconminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle,0x112,0xf012,0);
        }

        private void AbrirFormEnPanel(object Formhijo)
        {
            //if (this.panelContenedor.Controls.Count > 0)
            //    this.panelContenedor.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panelContenedor.Controls.Add(fh);
            this.panelContenedor.Tag = fh;
            groupBox1.Hide();
            tabControl1.Hide();
            button3.Hide();
            fh.Show();

        }

        private void btnprod_Click(object sender, EventArgs e)
        {
            groupBox1.Show();
    
            if (vtgrid.Rows.Count > 0)
            {
                tabControl1.Show();
                button3.Show();
            }                    
        }

        private void btnlogoInicio_Click(object sender, EventArgs e)
        {
            //AbrirFormEnPanel(new InicioResumen());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnlogoInicio_Click(null,e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label5.Text = DateTime.Now.ToString("hh:mm:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new InicioResumen());
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            choofdlog.Title = "Select .exe file/s only";
            choofdlog.Filter = "Exe file (*.exe)|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;
            lblPath.Text = "";
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                if (choofdlog.FileNames.Count() >= 2)
                {  lblPath.Text = "Multiple files";  }

                else
                {
                    lblPath.Text = choofdlog.FileName;
                    textFile = choofdlog.FileName;
                }            
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (choofdlog.FileNames.Count() >= 1)
            {
                progressBar1.Value = 20;
                button4.Enabled = false;
                Ipgrid.DataSource = null;
                Apigrid.DataSource = null;
                vtgrid.DataSource = null;
                Ipgrid.Refresh();
                Apigrid.Refresh();
                vtgrid.Refresh();
                label6.Visible = true;
                label7.Visible = true;
                button3.Visible = false;

                APIcount = 0;
                IPratecount = 0;
                mallciousSection = 0;
                shell = 0;
                rshellip = "";

                vtdt.Clear();
                vtdt.Columns.Clear();
                vtdt.Columns.Add("File Name");
                vtdt.Columns.Add("Engines");
                vtdt.Columns.Add("Count");


                dtip.Clear();
                dtip.Columns.Clear();
                dtip.Clear();
                dtip.Columns.Add("File Name");
                dtip.Columns.Add("IP");
                dtip.Columns.Add("Blacklist Rate");
                dtip.Columns.Add("Continent Name");
                dtip.Columns.Add("Country Name");
                dtip.Columns.Add("City Name");
                dtip.Columns.Add("Latitude");
                dtip.Columns.Add("Longitude");

                apidt.Clear();
                apidt.Columns.Clear();
                apidt.Columns.Add("File Name");
                apidt.Columns.Add("DLL Name");
                apidt.Columns.Add("Win32 API");
                apidt.Columns.Add("Description");


                choofdlog.FileNames.ToList().ForEach(file =>
                {
                var val = Path.GetFileName(file);
                    val = val.Split('\\').Last();
                    fileslist.Add(val);
                    currentfilename = val;
                    Operations(file);
                });
                tabControl1.Visible = true;
                progressBar1.Value = 100;
                outputmsgs();
            }
            else
            {
                MessageBox.Show("Please select file first", "Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void Operations(string textFile)
        {
           

            try
                {               
                    filename = "sample" + DateTime.Now.Ticks;
                    filepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Samples/" + filename + ".exe";

                    File.Copy(textFile, filepath);
                    string basepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/lib";

                    //Task task1 = Task.Factory.StartNew(() => virustotal(currentfilename));
                   // Task task2 = Task.Factory.StartNew(() => sectionAnalysis(filepath, filename));
                    Stringpattern(filepath);
                    DLLsearch(filepath);
                   sectionAnalysis(filepath, filename);
                    virustotal(currentfilename);

                   //Task.WaitAll(task1, task2);

                    //progressBar1.Value = 100;
                    //tabControl1.Visible = true;
                    button4.Enabled = true;
                   
                
                    fileslist.Clear();

            }
                catch (Exception wex)
                {
                    MessageBox.Show("CMD Error", "Error");
                }          
        }

        private void Stringpattern(string filepath)
        {
            string txtfile = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Strings/" + filename + ".txt";
            string para = " /C lib01 " + filepath + " > " + txtfile;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = para + " & exit";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            isVirusIp(txtfile);

        }

        private void isVirusIp(string IpPath)
        {
            try
            {
                string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
                Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");

                Regex regex = new Regex(pattern);
                var ip = "";
                var url = "";
                if (File.Exists(IpPath))
                {
                    string[] lines = File.ReadAllLines(IpPath);
                    int row = 0;
                    foreach (string line in lines)
                    {
                        Match match = regex.Match(line);
                        while (match.Success)
                        {

                            var iss = CallAPi(match.Value);
                            if (Convert.ToInt32(iss.Item1) > 0)
                            {

                                DataRow dr = dtip.NewRow();
                                dr["File Name"] = currentfilename;
                                dr["IP"] = match.Value;
                                if (Convert.ToInt32(iss.Item1) >= 2) { IPratecount = 1; } else { IPratecount = 0; }
                                dr["Blacklist Rate"] = iss.Item1;
                                dr["Continent Name"] = iss.Item2;
                                dr["Country Name"] = iss.Item3;
                                dr["City Name"] = iss.Item4;
                                dr["Latitude"] = iss.Item5;
                                dr["Longitude"] = iss.Item6;
                                dtip.Rows.Add(dr);
                                row++;
                            }

                            match = match.NextMatch();
                        }

                    }

                    if (dtip.Rows.Count >= 1)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Ipgrid.DataSource = dtip;
                           // Ipgrid.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
                            Ipgrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Ipgrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                            progressBar1.Value = 50;
                            //Ipgrid.Rows[Ipgrid.Rows.Count-1].Selected = true;
                            label6.Visible = false;

                            try
                            {
                                foreach (DataGridViewRow rows in Ipgrid.Rows)
                                {
                                    if (Convert.ToInt32(rows.Cells["Blacklist Rate"].Value) < 1)
                                    {
                                        rows.Cells["Blacklist Rate"].Style.ForeColor = Color.Green;
                                    }
                                    else if (Convert.ToInt32(rows.Cells["Blacklist Rate"].Value) >= 2)
                                    {
                                        rows.Cells["Blacklist Rate"].Style.ForeColor = Color.Red;
                                    }
                                }
                            }
                            catch (Exception k) { }

                        });

                    }
                    //txtapi.DataSource = dt;
                    //txtapi.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
                    //txtapi.ColumnHeadersDefaultCellStyle.Font = new Font(txtapi.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    //progressBar1.Value = 50;
                }
            }
            catch (Exception wex)
            {
                MessageBox.Show("Check internet connection", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Tuple<string, string, string, string, string, string> CallAPi(string Ipadd)
        {
            //Ipadd = "192.168.1.1";
            string URL = "https://endpoint.apivoid.com/iprep/v1/pay-as-you-go/?key=" + APIKey + "&ip=" + Ipadd;
            //URL = "https://api.ipdata.co/"+ Ipadd + "?api-key=d2a0d5f3e1ee2cd969708e4ce5fac92889af81db0f77689db175f5b4";

            string error = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.ContentType = "application/json";

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    //Console.Out.WriteLine(response);
                    error = response;
                    var json_serializer = new JavaScriptSerializer();
                    var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(response);

                    dynamic data = json_serializer.Deserialize<object>(response);
                    string BlackCount = "0";
                    string country_name = "Private IP address";
                    string continent_name = "";
                    string city_name = "";
                    string latitude = "";
                    string longitude = "";

                    foreach (var property in data)
                    {
                        try
                        {
                            var report = property.Value;
                            foreach (var ritem in report)
                            {
                                var blacklists = ritem.Value;
                                //foreach (var bite in blacklists)
                                //{
                                //    var engin = bite.Key;
                                //    if (bite.Key == "blacklists")
                                //    {
                                //        BlackCount = Convert.ToString( bite.Value.Count);
                                //    }
                                //}

                                float truecount = Regex.Matches(response, "true").Count;
                                float tot = Regex.Matches(response, "false").Count + truecount;
                                float val = (truecount/ tot)*100;

                                if (val >= 81 && val <= 100) BlackCount = "5";
                                else if (val >= 61 && val <= 80) BlackCount = "4";
                                else if (val >= 41 && val <= 60) BlackCount = "3";
                                else if (val >= 21 && val <= 40) BlackCount = "2";
                                else if (val >= 0 && val <= 20) BlackCount = "1";


                                var information = ritem.Value;
                                foreach (var binfo in information)
                                {
                                    if (binfo.Key == "information")
                                    {

                                        foreach (var inf in binfo.Value)
                                        {
                                            if (inf.Key == "country_name")
                                            {
                                                country_name = Convert.ToString(inf.Value);
                                            }
                                            else if (inf.Key == "continent_name")
                                            {
                                                continent_name = Convert.ToString(inf.Value);
                                            }
                                            else if (inf.Key == "city_name")
                                            {
                                                city_name = Convert.ToString(inf.Value);
                                            }
                                            else if (inf.Key == "latitude")
                                            {
                                                latitude = Convert.ToString(Convert.ToDecimal(inf.Value));
                                            }
                                            else if (inf.Key == "longitude")
                                            {
                                                longitude = Convert.ToString(Convert.ToDecimal(inf.Value));
                                            }
                                        }
                                    }

                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(error+""+ex.ToString(), "Analyzer");
                            return Tuple.Create(BlackCount, country_name, continent_name, city_name, latitude, longitude);
                        }

                    }

                    return Tuple.Create(BlackCount, country_name, continent_name, city_name, latitude, longitude);
                }
            }
            catch (Exception e)
            {

                //Console.Out.WriteLine("-----------------");
                // Console.Out.WriteLine(e.Message);
                return null;
            }
        }

        private void DLLsearch(string filepath)
        {
            try
            {
                int chkmainprogram = 0;

                string basepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/lib";
                string txtfile = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Win32APIs/" + filename + ".txt";
                string para = " /C lib02 -x " + filepath + " > " + txtfile;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = para + " & exit";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            
                if (File.Exists(txtfile))
                {                   
                    //string txt = @"C:\Users\Administrator\Desktop\Analyer_me\Analyzer\Analyzer\bin\Debug\Outputs\Win32APIs\sample637244425696037752.txt";
                    //string[] lines = File.ReadAllLines(txt);
                    string[] lines = File.ReadAllLines(txtfile);
                
                    Boolean isfound = false;
                    int methodLine = 0;
                    int row = 0;
                    string DllName = "";
                    foreach (string line in lines)
                    {
                        Match match = Regex.Match(line, @"DLL Name:");
                        if (match.Success)
                        {
                            string Dllname = (line.Split(':')[1] != null) ? line.Split(':')[1] : "";

                            isfound = true;
                            DllName = Dllname.Trim();
                            Console.WriteLine(Dllname.Trim());
                            mainrecord = 0;
                           
                        }

                        if (isfound == true && methodLine > 1)
                        {
                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex("[ ]{2,}", options);
                            string newline = regex.Replace(line, " ");
                            if (newline.Length == 0)
                            {
                                isfound = false;
                                methodLine = 0;
                                //mainrecord = 0;
                            }
                            else
                            {
                                try
                                {
                                    string FunctionNAme = (newline.Split(' ')[2] != null) ? newline.Split(' ')[2] : "";
                                    var isvirus = IsVirusDLL(DllName, FunctionNAme.Trim());
                                    if (isvirus.Item1)
                                    {
                                        if (mainrecord == 1)
                                        {
                                            DataRow drc = apidt.NewRow();
                                            //if (chkmainprogram == 1)
                                            //{
                                            //    drc["File Name"] = currentfilename;
                                            //}
                                            //else
                                            //{
                                            //    drc["File Name"] = "";
                                            //}
                                            drc["File Name"] = currentfilename;
                                            drc["DLL Name"] = DllName;
                                            drc["Win32 API"] = "";
                                            drc["Description"] = "";
                                            apidt.Rows.Add(drc);
                                            row++;
                                        }

                                        Console.WriteLine(FunctionNAme.Trim());
                                        DataRow dr = apidt.NewRow();
                                        dr["File Name"] = "";
                                        dr["DLL Name"] = "";
                                        dr["Win32 API"] = FunctionNAme.Trim();
                                        dr["Description"] = isvirus.Item2;
                                        apidt.Rows.Add(dr);
                                        row++;
                                    }
                                    else
                                    {

                                    }
                                }
                                catch (Exception k) { }

                            }
                        }

                        if (isfound)
                        {
                            methodLine++;
                        }
                        
                    }

                    if (apidt.Rows.Count>=1) {
                        this.Invoke((MethodInvoker)delegate {
                            Apigrid.DataSource = apidt;
                            Apigrid.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
                            Apigrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Apigrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                            MLdt = apidt;
                        });
                        label7.Visible = false;
                        //tabPage1.ForeColor = Color.Red;
                        
                    }
                }


                else
                {
                    MessageBox.Show("No genarated Assmbly ", "Error");
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show("Error Reading", "Error");
            }
        
        }

        private Tuple<Boolean, string> IsVirusDLL(string DllName, string Api)
        {
            try
            {
                SqlConnection cnn;
                // var constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;               
                var constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\App_Data\analice_db.mdf;Integrated Security=True";
          
                constr = @"Data Source=localhost;Initial Catalog=Analyzer;User Id=sa;Password=1234";
                cnn = new SqlConnection(constr);
                cnn.Open();

                string sql = "select * from DLL_referance where Dll_name='" + DllName + "' and API='" + Api + "'";
                SqlCommand comm = new SqlCommand(sql, cnn);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable tbl = new DataTable();
                da.Fill(tbl);
                cnn.Close();
                if (tbl.Rows.Count > 0)
                {
                    mainrecord++;
                    APIcount = 1;
                    return Tuple.Create(true, tbl.Rows[0]["Discription"].ToString());
                }
                else
                {

                    return Tuple.Create(false, "");
                }
            }
            catch (Exception k)
            {
                MessageBox.Show("SQL Error", "Error");
                return Tuple.Create(false, "");
            }
        }

        private void outputmsgs()
        {
            button3.Visible = true;
            if (rshellip != "") { shell = 1; }
            Output frm = new Output(APIcount, mallciousSection, IPratecount,vtcount,shell, vtdt, dtip, apidt,lblusuario.Text,currentfilename,rshellip);
            frm.Show();

        }

        private async void sectionAnalysis(string filepath, string filename)
        {
            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Outputs/Sections/" + filename;
            System.IO.Directory.CreateDirectory(path);

            try
            {
                Headerbasicinfo myobj = new Headerbasicinfo();
                List<SectionPropertise> sectionsList = new List<SectionPropertise>();

                PeHeaderReader reader = new PeHeaderReader(filepath);
                var Charctor = reader.ImageSectionHeaders;

                if (reader.Is32BitHeader)
                {
                    myobj.EnteryAddress = reader.OptionalHeader32.AddressOfEntryPoint;
                    myobj.Sizeofcode = reader.OptionalHeader32.SizeOfCode;
                    myobj.SizeOfheader = reader.OptionalHeader32.SizeOfHeaders;
                    myobj.Noofsections = reader.ImageSectionHeaders.Count();
                    myobj.checksum = reader.OptionalHeader32.CheckSum;
                }
                else
                {
                    myobj.EnteryAddress = reader.OptionalHeader32.AddressOfEntryPoint;
                    myobj.Sizeofcode = reader.OptionalHeader32.SizeOfCode;
                    myobj.SizeOfheader = reader.OptionalHeader32.SizeOfHeaders;
                    myobj.Noofsections = reader.ImageSectionHeaders.Count();
                    myobj.checksum = reader.OptionalHeader32.CheckSum;
                }

                var data = MicrosoftPe.FromFile(filepath);
                var getsection = data.Pe.Sections;
                index = 0;
                foreach (var item in getsection)
                {
                    SectionPropertise obj = new SectionPropertise();
                    obj.Body = item.Body;
                    obj.Name = item.Name;
                    obj.Hexdump = BitConverter.ToString(item.Body).Replace("-", "");

                    obj.Character = Charctor[index].Characteristics.ToString();
                    obj.SizeofRawdata = item.SizeOfRawData;
                    obj.VirtualAddress = item.VirtualAddress;
                    obj.Index = index;
                    obj.VirtualSize = item.VirtualSize;
                    obj.Pointer = item.PointerToRawData;

                    if (obj.Character.Contains("ContentCode") || obj.Character.Contains("MemoryExecute")  /*1==1*/)
                    {
                        obj.Issuspicious = true; mallciousSection++;
                        string sectionname = obj.Name + "_" + index;
                        File.WriteAllBytes(path + "/" + sectionname + ".bin", obj.Body);
                        SectionAnalyzeprocess(path + "/" + sectionname + ".bin");
                    }
                    else obj.Issuspicious = false;

                    sectionsList.Add(obj);
                    index++;
                }

            }
            catch (Exception k)
            {
                MessageBox.Show("Section Table Error");
            }
        }

        private void virustotal(string currentfilename)
        {
            string x = "";
            using (FileStream stream = File.OpenRead(filepath))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                x = BitConverter.ToString(hash).Replace("-", String.Empty);
            }

            string URL = "https://www.virustotal.com/vtapi/v2/file/report?apikey=key&resource=" + x;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    dynamic data = JObject.Parse(response);

                    //dt.Clear();
                    
                    
                    if (data.response_code == 0)
                    {
                        DataRow dr1 = vtdt.NewRow();
                        dr1["File Name"] = currentfilename;
                        dr1["Engines"] = "Postive Engines";
                        dr1["Count"] = "No record";
                        vtdt.Rows.Add(dr1);
                        DataRow dr = vtdt.NewRow();
                        dr["Engines"] = "Total Engines";
                        dr["Count"] = "No record";
                        vtcount = "No record";
                        vtdt.Rows.Add(dr);
                    }
                    else
                    {

                        DataRow dr = vtdt.NewRow();
                        dr["File Name"] = currentfilename;
                        dr["Engines"] = "Postive Engines";
                        dr["Count"] = data.positives;
                        vtdt.Rows.Add(dr);
                        DataRow dr1 = vtdt.NewRow();
                        dr1["Engines"] = "Total Engines";
                        dr1["Count"] = data.total;
                        vtdt.Rows.Add(dr1);
                        DataRow dr2 = vtdt.NewRow();
                        dr2["Engines"] = "Rate";
                        vtcount = Math.Round(((Convert.ToDouble(data.positives) / Convert.ToDouble(data.total)) * 100), 0) + "%";
                        dr2["Count"] = vtcount;
                        vtdt.Rows.Add(dr2);
                    }

                    this.Invoke((MethodInvoker)delegate {
                        vtgrid.DataSource = vtdt;
                        vtgrid.Rows[0].DefaultCellStyle.ForeColor = Color.Red;
                        vtgrid.Columns[0].Width = 200;
                        vtgrid.Columns[1].Width = 180;
                        vtgrid.Columns[2].Width = 120;
                    });

                }
            }
            catch (Exception k)
            {
                MessageBox.Show("Virus Total Error", "Analyzer");
            }

        }

        public void MLinterface()
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(MLdt);

            //azure machine learning portal url
            string url = "" + "&Input=" + JSONString;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    //ML output
                }
            }
            catch (Exception k) { }
        }

        public class Headerbasicinfo
        {
            public uint EnteryAddress { get; set; }
            public uint Sizeofcode { get; set; }
            public uint SizeOfheader { get; set; }
            public int Noofsections { get; set; }
            public uint checksum { get; set; }
        }

        public class SectionPropertise
        {
            public bool Issuspicious { get; set; }
            public string Name { get; set; }
            public byte[] Body { get; set; }
            public string Character { get; set; }
            public uint SizeofRawdata { get; set; }
            public uint VirtualAddress { get; set; }
            public int Index { get; set; }
            public uint VirtualSize { get; set; }
            public uint Pointer { get; set; }
            public string Hexdump { get; set; }
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8
                + 3;

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3
                + (bytesPerLine - 1) / 8
                + 2;

            int lineLength = firstCharColumn
                + bytesPerLine
                + Environment.NewLine.Length;

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        public void SectionAnalyzeprocess(string path)
        {
            try
            {
                string para = "/C lib03 /findsc /auto /f " + path +"> shellIP.txt";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = para + " & exit";
                process.StartInfo = startInfo;
                process.Start();
                //Thread.Sleep(2000);
                process.WaitForExit();
                string IpPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/shellIP.txt";

                string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
                Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");

                Regex regex = new Regex(pattern);

                if (File.Exists(IpPath))
                {
                    string[] lines = File.ReadAllLines(IpPath);
                    foreach (string line in lines)
                    {
                        bool breakLoops = false;

                        Match match = regex.Match(line);
                        while (match.Success)
                        {                         
                            rshellip = match.Value;
                            APIcount = 1;
                            DataRow dr = apidt.NewRow();
                            //get values form db
                            dr["File Name"] = currentfilename;
                            dr["DLL Name"] = "Ws2_32.dll";
                            dr["Win32 API"] = "WSASocketA";
                            dr["Description"] = "Creates a socket that is bound to a specific transport-service provider";
                            apidt.Rows.Add(dr);

                            DataRow dr1 = apidt.NewRow();
                            dr1["File Name"] = currentfilename;
                            dr1["DLL Name"] = "Ws2_32.dll";
                            dr1["Win32 API"] = "connect";
                            dr1["Description"] = "Establishes a connection to a specified socket";
                            apidt.Rows.Add(dr1);

                            DataRow dr2 = apidt.NewRow();
                            dr2["File Name"] = currentfilename;
                            dr2["DLL Name"] = "Ws2_32.dll";
                            dr2["Win32 API"] = "recv";
                            dr2["Description"] = "Receives data from a connected socket or a bound connectionless socket";
                            apidt.Rows.Add(dr2);

                            DataRow dr3 = apidt.NewRow();
                            dr3["File Name"] = currentfilename;
                            dr3["DLL Name"] = "Ws2_32.dll";
                            dr3["Win32 API"] = "closesocket";
                            dr3["Description"] = "Closes an existing socket";
                            apidt.Rows.Add(dr3);

                            var iss = CallAPi(match.Value);

                            if (Convert.ToInt32(iss.Item1) > 0)
                            {

                                DataRow drip = dtip.NewRow();
                                drip["File Name"] = currentfilename;
                                drip["IP"] = match.Value;
                                if (Convert.ToInt32(iss.Item1) >= 5) { IPratecount = 1; } else { IPratecount = 0; }
                                drip["Blacklist Rate"] = iss.Item1;
                                drip["Continent Name"] = iss.Item2;
                                drip["Country Name"] = iss.Item3;
                                drip["City Name"] = iss.Item4;
                                drip["Latitude"] = iss.Item5;
                                drip["Longitude"] = iss.Item6;
                                dtip.Rows.Add(drip);
                            }

                            this.Invoke((MethodInvoker)delegate
                            {
                                Ipgrid.DataSource = dtip;
                                Ipgrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Ipgrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                                label6.Visible = false;                             
                            });

                            this.Invoke((MethodInvoker)delegate {
                                    Apigrid.DataSource = apidt;
                                    Apigrid.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
                                    Apigrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Apigrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                                    label7.Visible = false;
                            });
                              

                            breakLoops = true;
                            break;

                        }
                        if (breakLoops)
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception k)
            {

            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Ipgrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
      
        private void Ipgrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            outputmsgs();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
