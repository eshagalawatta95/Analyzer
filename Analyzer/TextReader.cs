using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Nerdicus.VirusTotalNET;
using Nerdicus.VirusTotalNET.Models.File;
using System.Security.Cryptography;
using System.Threading;
using _Peanalyzer.Data;
using Kaitai;

namespace Analyzer
{
    public partial class TextReader : Form
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
        DataTable MLdt = new DataTable();

        public TextReader()
        {
            InitializeComponent();

            try
            {
                APIKey = File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/APIkey.txt");
            }
            catch(Exception k)
            {
                MessageBox.Show("No API key found", "Error");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label5.Text = "";
            APIcount = 0;
            IPratecount = 0;
            mallciousSection = 0;

            if (textFile != "")
            {
                button2.Enabled = false;
                txtapi.DataSource = null;
                txtGrid.DataSource = null;
                txtVT.DataSource = null;
                txtapi.Refresh();
                txtGrid.Refresh();
                txtVT.Refresh();
                //txtapi.Rows.Clear();
                //txtGrid.Rows.Clear();
                try
                {
                    label5.Text = "";
                    progressBar1.Value = 20;
                    filename = "sample" + DateTime.Now.Ticks;
                    filepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Samples/" + filename + ".exe";

                    File.Copy(textFile, filepath);
                    string basepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/lib";
                  
                     Task task1 = Task.Factory.StartNew(() => virustotal());                                                  
                     Stringpattern(filepath);
                     DLLsearch(filepath);
                     sectionAnalysis(filepath, filename);
                     outputmsgs();

                     button2.Enabled = true;

                }
                catch (Exception wex)
                {
                    MessageBox.Show("CMD Error", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select File", "Error");
            }

        }

        private  void Stringpattern(string filepath)
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

                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("IP");
                    dt.Columns.Add("Blacklist Rate");
                    dt.Columns.Add("Continent Name");
                    dt.Columns.Add("Country Name");
                    dt.Columns.Add("City Name");
                    dt.Columns.Add("Latitude");
                    dt.Columns.Add("Longitude");
                    int row = 0;
                    foreach (string line in lines)
                    {
                        Match match = regex.Match(line);
                        while (match.Success)
                        {

                            var iss = CallAPi(match.Value);
                            if (Convert.ToInt32(iss.Item1) > 0)
                            {

                                DataRow dr = dt.NewRow();
                                dr["IP"] = match.Value;
                                if (Convert.ToInt32(iss.Item1) >= 5) { IPratecount = 1; } else { IPratecount = 0; }
                                dr["Blacklist Rate"] = iss.Item1;
                                dr["Continent Name"] = iss.Item2;
                                dr["Country Name"] = iss.Item3;
                                dr["City Name"] = iss.Item4;
                                dr["Latitude"] = iss.Item5;
                                dr["Longitude"] = iss.Item6;
                                dt.Rows.Add(dr);
                                row++;
                            }

                            match = match.NextMatch();

                        }


                    }

                    this.Invoke((MethodInvoker)delegate {
                        txtapi.DataSource = dt;
                        txtapi.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
                        txtapi.ColumnHeadersDefaultCellStyle.Font = new Font(txtapi.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                        progressBar1.Value = 50;
                    });

                    //txtapi.DataSource = dt;
                    //txtapi.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
                    //txtapi.ColumnHeadersDefaultCellStyle.Font = new Font(txtapi.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    //progressBar1.Value = 50;
                }
            }
            catch (Exception wex)
            {
                MessageBox.Show("Check internet connection", "Warning");
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

                                int truecount = Regex.Matches(response, "true").Count;

                                BlackCount = truecount.ToString();

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

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("DLL Name");
                dt.Columns.Add("Win32 API");
                dt.Columns.Add("Description");

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
                                            DataRow drc = dt.NewRow();
                                            drc["DLL Name"] = DllName;
                                            drc["Win32 API"] = "";
                                            drc["Description"] = "";
                                            dt.Rows.Add(drc);
                                            row++;
                                        }

                                        Console.WriteLine(FunctionNAme.Trim());
                                        DataRow dr = dt.NewRow();
                                        dr["DLL Name"] = "";
                                        dr["Win32 API"] = FunctionNAme.Trim();
                                        dr["Description"] = isvirus.Item2;
                                        dt.Rows.Add(dr);
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

                    this.Invoke((MethodInvoker)delegate {
                        txtGrid.DataSource = dt;
                        txtGrid.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
                        txtGrid.ColumnHeadersDefaultCellStyle.Font = new Font(txtGrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                        MLdt = dt;
                    });

                    //txtGrid.DataSource = dt;
                    //txtGrid.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
                    //txtGrid.ColumnHeadersDefaultCellStyle.Font = new Font(txtGrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    //MLdt = dt;
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

            this.Invoke((MethodInvoker)delegate {
                progressBar1.Value = 100;
            });
           // progressBar1.Value = 100;
        }

        private Tuple<Boolean, string> IsVirusDLL(string DllName, string Api)
        {
            try
            {
                SqlConnection cnn;
                // var constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;               
                 var constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\App_Data\analice_db.mdf;Integrated Security=True";
                //var constr = "Data Source=SQL5042.site4now.net;Initial Catalog=DB_A0B2B6_PeCRM;User Id=DB_A0B2B6_PeCRM_admin;Password=hbsi@1234";
                constr = @"Data Source=localhost;Initial Catalog=Analyzer;User Id=sa;Password=hbsi@1234";
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

        private void lblPath_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Exe file (*.exe)|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            lblPath.Text = "";
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                lblPath.Text = choofdlog.FileName;
                textFile = choofdlog.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            //  choofdlog.Filter = "Exe 64 bit file (*.exe)|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                //lblapipath.Text = choofdlog.FileName;

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void outputmsgs()
        {
            string msg = "";
            string secmsg = "";
            if (mallciousSection > 1)
                secmsg = "Injected suspicious section detected.";

            if (APIcount == 1 && IPratecount == 1)
            {
                msg = "";
                label5.Text = "Backdoor Detected";
                msg = "Suspicious IP address / Windows APIs detected." + secmsg + "This Program may be backdoored.";
                MessageBox.Show(msg, "Analyzer V 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (APIcount == 1 && IPratecount == 0)
            {
                msg = "";
                label5.Text = "Backdoor Detected";
                msg = "Suspicious Windows APIs Detected." + secmsg + "This program may be backdoored.";
                MessageBox.Show(msg, "Analyzer V 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (IPratecount == 1 && APIcount == 0)
            {
                msg = "";
                label5.Text = "Backdoor Detected";
                msg = "Suspicious IP address detected." + secmsg + "This Program may be backdoored.";
                MessageBox.Show(msg, "Analyzer V 1.0", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            else if (APIcount == 0 && IPratecount == 0 && mallciousSection == 1)
            {
                msg = "";
                label5.Text = "";
                msg = "No any suspicious Activity detected.";
                MessageBox.Show(msg, "Analyzer V 1.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (mallciousSection > 1)
            {
                label5.Text = "Backdoor Detected";
                MessageBox.Show(secmsg, "Analyzer V 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void sectionAnalysis(string filepath,string filename)
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

                    if (obj.Character.Contains("ContentCode") || obj.Character.Contains("MemoryExecute")  /*1==1*/) {
                        obj.Issuspicious = true; mallciousSection++;
                        string sectionname =obj.Name+"_"+index;
                        File.WriteAllBytes(path+"/"+sectionname + ".bin", obj.Body);
                        SectionAnalyzeprocess(path + "/" + sectionname + ".bin");
                    } else obj.Issuspicious = false;

                    sectionsList.Add(obj);
                    index++;
                }

            }
            catch (Exception k)
            {
                MessageBox.Show("Section Table Error");
            }
        }

        private void virustotal()
        {
            string x = "";
            DataTable dt = new DataTable();
         
            using (FileStream stream = File.OpenRead(filepath))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                x = BitConverter.ToString(hash).Replace("-", String.Empty);
            }

            string URL = "https://www.virustotal.com/vtapi/v2/file/report?apikey=df72c25d0ee62f5e1c22cb6d260870637ff4b1aac6ba4e700bb16584792de497&resource=" + x;

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

                    dt.Clear();
                    dt.Columns.Add("Title");
                    dt.Columns.Add("Count");

                    if (data.response_code == 0)
                    {
                        DataRow dr1 = dt.NewRow();
                        dr1["Title"] = "Postive Engines";
                        dr1["Count"] = "No record";
                        dt.Rows.Add(dr1);
                        DataRow dr = dt.NewRow();
                        dr["Title"] = "Total Engines";
                        dr["Count"] = "No record";
                     
                        dt.Rows.Add(dr);
                    }
                    else
                    {

                        DataRow dr = dt.NewRow();
                        dr["Title"] = "Postive Engines";
                        dr["Count"] = data.positives;
                        dt.Rows.Add(dr);
                        DataRow dr1 = dt.NewRow();
                        dr1["Title"] = "Total Engines";
                        dr1["Count"] = data.total;
                        dt.Rows.Add(dr1);
                        DataRow dr2 = dt.NewRow();
                        dr2["Title"] = "Rate";
                        dr2["Count"] = Math.Round(((Convert.ToDouble(data.positives) / Convert.ToDouble(data.total)) * 100), 0)+"%";
                        dt.Rows.Add(dr2);
                    }

                    this.Invoke((MethodInvoker)delegate {
                        txtVT.DataSource = dt;
                        txtVT.Rows[0].DefaultCellStyle.ForeColor = Color.Red;
                    });
                 
                }
            }
            catch (Exception k)
            {
                MessageBox.Show("Virus Total Error","Analyzer");
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
    
        public async void copyDB(){
            try
            {
                SqlConnection source = new SqlConnection(@"Data Source=SQL5042.site4now.net;Initial Catalog=DB_A0B2B6_PeCRM;User Id=DB_A0B2B6_PeCRM_admin;Password=hbsi@1234");
                SqlConnection destination = new SqlConnection(@"Data Source=localhost;Initial Catalog=Analyzer;User Id=sa;Password=hbsi@1234");
                //SqlConnection destination = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\App_Data\analice_db.mdf;Integrated Security=True");

                SqlCommand cmd = new SqlCommand("DELETE FROM DLL_referance", destination);
                source.Open();
                destination.Open();
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("SELECT * FROM DLL_referance", source);
                SqlDataReader reader = cmd.ExecuteReader();
                SqlBulkCopy bulkData = new SqlBulkCopy(destination);
                bulkData.DestinationTableName = "DLL_referance";
                bulkData.WriteToServer(reader);
                bulkData.Close();
                destination.Close();
                source.Close();

            }
            catch(Exception k)
            {
                MessageBox.Show("DB Error","Analyzer");
            }
           

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            copyDB();
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
                string para = " /C scdbg /findsc /f" + path + " > " + "a.txt";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = para + " & exit";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch(Exception k)
            {

            }

        }

        private void TextReader_Load(object sender, EventArgs e)
        {

        }

        private void txtGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtapi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtVT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }

}
