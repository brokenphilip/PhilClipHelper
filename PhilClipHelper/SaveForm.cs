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
        private string _args = "";
        private string _videoFile = "";

        private Point _mainFormLocation;
        private Size _mainFormSize;

        private Process _process;
        private bool _processRunning = false;

        public SaveForm(string args, string videoFile, Point mainFormLocation, Size mainFormSize)
        {
            _args = args;
            _videoFile = videoFile;
            _mainFormLocation = mainFormLocation;
            _mainFormSize = mainFormSize;

            InitializeComponent();

            if (Program.UseDarkTheme)
            {
                BackColor = Program.DarkTheme.BackColor;

                textBoxOutput.ForeColor = Program.DarkTheme.ForeColor;
                textBoxOutput.BackColor = Program.DarkTheme.BackColor;

                foreach (Control c in Controls)
                {
                    if (c is Label)
                    {
                        c.ForeColor = Program.DarkTheme.ForeColor;
                    }
                    else if (c is Button)
                    {
                        c.BackColor = Program.DarkTheme.BackColor;
                        c.ForeColor = Program.DarkTheme.ForeColor;
                    }
                }

                Program.EnableDarkTitlebar(this);
            }

            labelStatus.Text = "\"" + Path.GetFileName(videoFile) + "\" is being saved...";
        }

        private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            Program.ControlBeginInvoke(textBoxOutput, new MethodInvoker(delegate ()
            {
                textBoxOutput.AppendText(e.Data + Environment.NewLine);
            }));
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(labelStatus, new MethodInvoker(delegate ()
            {
                if (File.Exists(_videoFile))
                {
                    labelStatus.Text = "\"" + Path.GetFileName(_videoFile) + "\" has been saved.";
                    buttonPlay.Enabled = true;
                    buttonFolder.Enabled = true;
                }
                else
                {
                    labelStatus.Text = "\"" + Path.GetFileName(_videoFile) + "\" failed to save!";
                }

                labelStatus.Font = new Font(labelStatus.Font, FontStyle.Bold);
                buttonClose.Enabled = true;
            }));

            _process.Close();
            _processRunning = false;
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            double midWidth = _mainFormLocation.X + (_mainFormSize.Width / 2.0);
            double midHeight = _mainFormLocation.Y + (_mainFormSize.Height / 2.0);

            double halfWidth = (Size.Width / 2.0);
            double halfHeight = (Size.Height / 2.0);

            // Put our save form in the middle of the main form
            Location = new Point((int)(midWidth - halfWidth), (int)(midHeight - halfHeight));

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = _args;

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            _process = new Process();
            _process.EnableRaisingEvents = true;
            _process.ErrorDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
            _process.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
            _process.Exited += new EventHandler(ProcessExited);
            _process.StartInfo = startInfo;
            try
            {
                _process.Start();
            }
            catch (Exception ex)
            {
                _process.Close();
                MessageBox.Show("FFmpeg failed to start: " + ex.Message, "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            _processRunning = true;
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Process.Start(_videoFile);
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetDirectoryName(_videoFile));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_processRunning)
            {
                // If we're closing anyways, fuck it, terminate FFmpeg
                _process.Kill();
                _process.Close();
                _processRunning = false;
            }
        }

        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_processRunning)
            {
                // Prevent the form from closing while FFmpeg is running
                e.Cancel = true;
            }
        }
    }
}
