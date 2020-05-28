using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Analyzer
{
    public partial class Output : Form
    {
        string htmlstring = "";
        DataTable vtdt;
        DataTable dtip;
        DataTable apidt;
        string user = "";
        string rsellip = "";
        string j= "style='color:green'", k= "style='color:green'", l= "style='color:green'", m= "style='color:green'";

        
        public Output(int APIcount, int mallciousSection, int IPratecount,string vt,int shell,DataTable vtdt1, DataTable dtip1, DataTable apidt1,string use,string current,string rsell)
        {
            InitializeComponent();
            vtdt = vtdt1;
            dtip = dtip1;
            apidt = apidt1;
            user = use;
            rsellip = rsell;
            this.Text = "Output";
            try
            {
                
                if (mallciousSection > 1)
                {
                    sec.Text = "Detected";
                    sec.ForeColor = Color.Red;
                     l = "style='color:red'";
                }
                if (APIcount == 1)
                {
                    dll.Text = "Detected";
                    dll.ForeColor = Color.Red;
                     k = "style='color:red'";
                }
                if (IPratecount == 1)
                {
                    ip.Text = "Detected";
                    ip.ForeColor = Color.Red;
                   j = "style='color:red'";
                }
                if (shell == 1)
                {
                    shelllbl.Text = "Detected";
                    shelllbl.ForeColor = Color.Red;
                    m = "style='color:red'";
                    rsellip = "<br/>Remote IP is " + rsellip;
                }
                vtlbl.Text = vt;
                if (vt != "No record")
                {
                    if (Convert.ToInt32(vt.Split('%')[0]) > 40)
                    {
                        vtlbl.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception k){ }
        }

        private void Output_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string html = "<div style='background:#f5f5f5;margin:-8px;padding:10px;'>"
                    + "<h1 style ='text-align:center;color:#343434;font-size:15px;margin:2px;'><strong> Analyzer 1.0 </strong></h1><h1 style ='text-align:center;font-size:20px; margin:5px; color:#343434;'><strong> Scan Report </strong></h1>"
                   + "</div><div style ='padding:20px 80px; background:white;'>"
                     + "<div style ='font-weight:600;'>"
                      + " <p> &nbsp; &nbsp; &nbsp; User -" + user + " &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; Date and Time -" + DateTime.Now.ToString("dd-MM-yyyy hh:mm") + "</p>"
                           + " </div></div><hr/>";
         
            StringBuilder strHTMLBuilder = new StringBuilder();

            strHTMLBuilder.Append("<br/>Suspicious IP Address list - <span " + j+">" + ip.Text + "</span><br/>");
            if (dtip.Rows.Count > 0) { 
            strHTMLBuilder.Append("<table border='1' style='border-collapse:collapse;width:100 %;'>");
            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dtip.Columns)
            {
                strHTMLBuilder.Append("<td style='background-color:#f5f5f5'><strong>");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</strong></td>");
            }
            strHTMLBuilder.Append("</tr>");
            foreach (DataRow myRow in dtip.Rows)
            {
                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dtip.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");
                }
                strHTMLBuilder.Append("</tr>");
            }
            strHTMLBuilder.Append("</table><br/>");
            }

            strHTMLBuilder.Append("<br/>Suspicious DLLs for Win32APIs - <span " + k + ">" + dll.Text + "</span><br/>");
            if (apidt.Rows.Count > 0)
            {
                strHTMLBuilder.Append("<table border='1' style='border-collapse:collapse;width:100 %;'>");
                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in apidt.Columns)
                {
                    strHTMLBuilder.Append("<td style='background-color:#f5f5f5'><strong>");
                    strHTMLBuilder.Append(myColumn.ColumnName);
                    strHTMLBuilder.Append("</strong></td>");
                }
                strHTMLBuilder.Append("</tr>");
                foreach (DataRow myRow in apidt.Rows)
                {
                    strHTMLBuilder.Append("<tr>");
                    foreach (DataColumn myColumn in apidt.Columns)
                    {
                        strHTMLBuilder.Append("<td>");
                        strHTMLBuilder.Append("<pre>" + myRow[myColumn.ColumnName].ToString().Replace("<none>", " ") + "</pre>");
                        strHTMLBuilder.Append("</td>");
                    }
                    strHTMLBuilder.Append("</tr>");
                }
                strHTMLBuilder.Append("</table><br/>");
            }

            strHTMLBuilder.Append("<br/>Virus Total Results <br/><table border='1' style='border-collapse:collapse;width:100 %;'>");
            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in vtdt.Columns)
            {
                strHTMLBuilder.Append("<td style='background-color:#f5f5f5'><strong>");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</strong></td>");
            }
            strHTMLBuilder.Append("</tr>");
            foreach (DataRow myRow in vtdt.Rows)
            {
                strHTMLBuilder.Append("<tr>");
                foreach (DataColumn myColumn in vtdt.Columns)
                {
                    strHTMLBuilder.Append("<td>");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");
                }
                strHTMLBuilder.Append("</tr>");
            }
            strHTMLBuilder.Append("</table><br/>");
            strHTMLBuilder.Append("Suspicious Sections - <span " + l + ">" + sec.Text + "</span><br/><br/>");
            strHTMLBuilder.Append("Embedded Shell Codes - <span " + m + ">" + shelllbl.Text + "</span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "+ rsellip + " <br/>");
            html = html + strHTMLBuilder.ToString() + "<br/><br/><br/><br/><br/><br/><br/><hr/>";


            string content = "<html><body> "+ html + " </body></html>";
            string path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
            path = path + "/Output"+ DateTime.Now.Ticks + ".pdf";

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A3, 51f, 51f, 51f, 51f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                StringReader sr = new StringReader(content);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                stream.Close();
            }
                ProcessStartInfo startInfo = new ProcessStartInfo(path);
                Process.Start(startInfo);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
