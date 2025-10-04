using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhilClipHelper
{
    public partial class StartupForm : Form
    {
        private MediaPlayerManager _mpm;
        public MediaPlayerManager MPM
        {
            get => _mpm;
        }

        public StartupForm()
        {
            InitializeComponent();

            if (Program.UseDarkTheme)
            {
                BackColor = Program.DarkTheme.BackColor;

                foreach (Control c in Controls)
                {
                    if (c is Label)
                    {
                        c.ForeColor = Program.DarkTheme.ForeColor;
                    }
                }

                Program.EnableDarkTitlebar(this);
            }
        }

        private void StartupForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            _mpm = new MediaPlayerManager();

            DialogResult = DialogResult.OK;

        }
    }
}
