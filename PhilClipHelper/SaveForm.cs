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
        private string videoFile = "";

        private Point mainFormLocation;
        private Size mainFormSize;

        private Process process;
        private bool processRunning = false;

        public SaveForm(string args, string videoFile, Point mainFormLocation, Size mainFormSize)
        {
            this.args = args;
            this.videoFile = videoFile;
            this.mainFormLocation = mainFormLocation;
            this.mainFormSize = mainFormSize;

            InitializeComponent();

            if (Program.useDarkTheme)
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

            labelStatus.Text = "\"" + Path.GetFileName(videoFile) + "\" is being saved...";
        }

        private void process_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            Invoke(new MethodInvoker(delegate ()
            {
                textBoxOutput.AppendText(e.Data + Environment.NewLine);
            }));
        }

        private void process_Exited(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                if (File.Exists(videoFile))
                {
                    labelStatus.Text = "\"" + Path.GetFileName(videoFile) + "\" has been saved.";
                    buttonPlay.Enabled = true;
                    buttonFolder.Enabled = true;
                }
                else
                {
                    labelStatus.Text = "\"" + Path.GetFileName(videoFile) + "\" failed to save!";
                }

                labelStatus.Font = new Font(labelStatus.Font, FontStyle.Bold);
                buttonClose.Enabled = true;
            }));

            process.Close();
            processRunning = false;
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            double midWidth = mainFormLocation.X + (mainFormSize.Width / 2.0);
            double midHeight = mainFormLocation.Y + (mainFormSize.Height / 2.0);

            double halfWidth = (Size.Width / 2.0);
            double halfHeight = (Size.Height / 2.0);

            Location = new Point((int)(midWidth - halfWidth), (int)(midHeight - halfHeight));

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            process = new Process();
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += new DataReceivedEventHandler(process_DataReceived);
            process.OutputDataReceived += new DataReceivedEventHandler(process_DataReceived);
            process.Exited += new EventHandler(process_Exited);
            process.StartInfo = startInfo;
            try
            {
                process.Start();
            }
            catch (Win32Exception ex)
            {
                process.Close();
                MessageBox.Show("FFmpeg failed to start: " + ex.Message, "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            processRunning = true;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Process.Start(videoFile);
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetDirectoryName(videoFile));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (processRunning)
            {
                process.Kill();
                process.Close();
                processRunning = false;
            }
        }

        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processRunning)
            {
                e.Cancel = true;
            }
        }
    }
}
