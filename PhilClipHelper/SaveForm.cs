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

namespace PhilClipHelper
{
    public partial class SaveForm : Form
    {
        private string args = "";
        private string fileName = "";

        public SaveForm(string args, string outputName)
        {
            this.args = args;
            this.fileName = outputName;

            InitializeComponent();

            if (MainForm.useDarkTheme)
            {
                Color dark = Color.FromArgb(255, 32, 32, 32);
                Color light = Color.FromArgb(255, 224, 224, 224);

                BackColor = dark;

                textBoxOutput.ForeColor = light;
                textBoxOutput.BackColor = dark;

                foreach (Control c in Controls)
                {
                    if (c is Label)
                    {
                        c.ForeColor = light;
                    }
                    else if (c is Button)
                    {
                        c.BackColor = dark;
                        c.ForeColor = light;
                    }
                }

                // Enable dark titlebar if supported
                if (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 17763)
                {
                    int darkModeAttribute = 19;
                    if (Environment.OSVersion.Version.Build >= 18985)
                    {
                        darkModeAttribute = 20;
                    }

                    int one = 1;
                    MainForm.DwmSetWindowAttribute(Handle, darkModeAttribute, ref one, sizeof(int));
                }
            }

            labelStatus.Text = "\"" + Path.GetFileName(fileName) + "\" is being saved...";
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            textBoxOutput.Text += e.Data;
        }

        Process process;

        private void process_Exited(object sender, EventArgs e)
        {
            if (File.Exists(fileName))
            {
                labelStatus.Text = "\"" + Path.GetFileName(fileName) + "\" has been saved.";
            }
            else
            {
                labelStatus.Text = "\"" + Path.GetFileName(fileName) + "\" failed to save!";
            }

            process.Close();
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "C:\\PathTools\\FFmpeg\\bin\\ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            process = new Process();
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            process.OutputDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            process.Exited += new EventHandler(process_Exited);
            process.StartInfo = startInfo;
            process.Start();
            process.BeginOutputReadLine();
        }
    }
}
