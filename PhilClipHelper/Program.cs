using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhilClipHelper
{
    internal static class Program
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        private static bool _useDarkTheme = false;
        public static bool UseDarkTheme
        {
            get => _useDarkTheme;
        }

        public class Theme
        {
            public Color BackColor;
            public Color ForeColor;
        }

        public static Theme DarkTheme = new Theme
        {
            BackColor = Color.FromArgb(255, 32, 32, 32),
            ForeColor = Color.FromArgb(255, 224, 224, 224),
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Object appsUseLightTheme = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
                if (appsUseLightTheme != null)
                {
                    _useDarkTheme = ((int)appsUseLightTheme == 0);
                }
            }
            catch
            {
                // Don't really care - force light theme anyways
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartupForm startupForm = new StartupForm();
            if (startupForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainForm(startupForm.MPM));
            }
            else
            {
                MessageBox.Show("Fatal error: LibVLCSharp failed to initialize.", "Phi(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void EnableDarkTitlebar(Form form)
        {
            // Enable dark titlebar if supported
            if (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 17763)
            {
                int darkModeAttribute = 19;
                if (Environment.OSVersion.Version.Build >= 18985)
                {
                    darkModeAttribute = 20;
                }

                int one = 1;
                DwmSetWindowAttribute(form.Handle, darkModeAttribute, ref one, sizeof(int));
            }
        }

        public static void ControlBeginInvoke(Control control, MethodInvoker invoker)
        {
            if (control.IsDisposed)
            {
                return;
            }

            if (!(control is Form))
            {
                Form form = control.FindForm();
                if (form != null)
                {
                    if (form.IsDisposed)
                    {
                        return;
                    }

                    IPchForm pchForm = (IPchForm)form;
                    if (pchForm.IsClosing())
                    {
                        return;
                    }
                }
            }

            if (control.InvokeRequired)
            {
                control.BeginInvoke(invoker);
            }
            else
            {
                invoker();
            }
        }
    }
}
