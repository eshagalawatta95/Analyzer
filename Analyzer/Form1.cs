using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analyzer
{
    public partial class Form1 : Form
    {
        string sSelectedPath;
        public static string TextData { get; set; }

        public Form1()
        {
            InitializeComponent();
            label2.Text = TextData;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Exe 64 bit file (*.exe)|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text =sSelectedPath;
                sSelectedPath = choofdlog.FileName;
               
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findIpandURL(sSelectedPath);
           // disassmbler(sSelectedPath);
           // ExeOperations(sSelectedPath);
        }


        public void ExeOperations(string sSelectedPath)
        {           
            //ExeDisassmbly(sSelectedPath);
            ExeStrings(sSelectedPath);
        }

        public byte[] Exetohex(byte[] hexdump)
        {
            string hex = String.Join("", hexdump.Select(p => p.ToString()).ToArray());
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public void ExeDisassmbly(string sSelectedPath)
        {
            SharpDisasm.ArchitectureMode mode = SharpDisasm.ArchitectureMode.x86_64;
            SharpDisasm.Disassembler.Translator.IncludeAddress = true;
            SharpDisasm.Disassembler.Translator.IncludeBinary = true;

            var code2= File.ReadAllBytes(sSelectedPath);
             Exetohex(File.ReadAllBytes(sSelectedPath));

            var disasm = new SharpDisasm.Disassembler(code2, mode, 0, true);

            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Diss.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            foreach (var insn in disasm.Disassemble())
            {
                Console.SetOut(writer);
                Console.WriteLine(insn.ToString());
                Console.SetOut(oldOut);
            }
            writer.Close();
            ostrm.Close();

        }

        public void ExeStrings(string sSelectedPath)
        {
            byte[] BytArr = File.ReadAllBytes(sSelectedPath);
            string filedump = HexDump(BytArr, 16);
            fileright(filedump);
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

        static public void fileright(string file)
        {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./string.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);
            Console.WriteLine(file);
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
        }

        public void disassmbler(string sSelectedPath)
        {
            try
            {
                File.Copy(sSelectedPath, Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/sample.exe");
                string para = "/C objdump -d " + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/sample.exe" + " > code.txt";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = para;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception k)
            {

            }
        }
        
        public void findIpandURL(string sSelectedPath)
        {
            File.Copy(sSelectedPath, Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/sample.exe");
            string para = "/C strings64 -a " + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/sample.exe" + " > stringpatterns.txt";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = para;
            process.StartInfo = startInfo;
            process.Start();

            string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
            Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");

            Regex regex = new Regex(pattern);
            var ip = "";
            var url = "";
            string[] lines = File.ReadAllLines(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/stringpatterns.txt");

            foreach (string line in lines)
            {
                Match match = regex.Match(line);
                while (match.Success)
                {
                    ip = ip + "," + match.Value;
                   
                    match = match.NextMatch();
                    //methanata API call eka gin blnna ona
                }

                
                Match m = r.Match(line);
                while (m.Success)
                {
                    url = url + "," + m.Value;

                    m = m.NextMatch();
                }
            }

           
            
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
