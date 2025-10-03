using LibVLCSharp.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhilClipHelper
{
    public partial class MainForm : Form
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        private LibVLC vlc;

        private MediaPlayer mpVideo;
        private string mpVideoFile = "";
        private int mpVideoVolume = 25;

        private MediaPlayer mpAudio;
        private string mpAudioFile = "";
        private int mpAudioVolume = 100;

        // Time in milliseconds to delay the audio clip
        // ie. how many milliseconds to wait for the video clip to play the audio clip
        // ie. how far ahead of time is the video clip compared to the audio clip
        private int mpAudioDelay = 0;

        private bool mpAudioCatchUp = false;

        private bool isClosing = false;

        private bool seeking = false;
        private bool unpauseAfterSeeking = false;

        struct DialogFormat
        {
            public string name;
            public string ext;

            public DialogFormat(string name, string ext)
            {
                this.name = name;
                this.ext = ext;
            }

            // Appends filters/formats for the Save/OpenFileDialog - only for internal use by SetOpen/SaveDialogFilters
            private static void AppendFileDialogFilters(FileDialog fd, DialogFormat[] dfs)
            {
                bool firstFormat = true;

                // We immediately want to add a '|', if this filter isn't empty, as we're about to append more formats to it
                if (!String.IsNullOrEmpty(fd.Filter))
                {
                    firstFormat = false;
                }

                // Can't modify fd.Filter directly, otherwise exceptions will be thrown - make a temporary variable first, and then copy it over to the file dialog filters
                string filter = "";
                foreach (DialogFormat df in dfs)
                {
                    if (!firstFormat)
                    {
                        filter += "|";
                    }

                    firstFormat = false;
                    filter += df.name + "|" + df.ext;
                }

                // Note that this function is "Append", not "Set" - the caller has the responsibility to clear the filters beforehand
                fd.Filter += filter;
            }

            // First adds a filter for all supported formats, then appends the rest of the filters/formats, and finally adds a filter for "All Files"
            public static void SetOpenDialogFilters(OpenFileDialog ofd, DialogFormat[] dfs)
            {
                bool firstFormat = true;
                string supportedExts = "";

                foreach (DialogFormat df in dfs)
                {
                    if (!firstFormat)
                    {
                        supportedExts += ";";
                    }

                    firstFormat = false;
                    supportedExts += df.ext;
                }

                ofd.Filter = "Supported files|" + supportedExts;
                AppendFileDialogFilters(ofd, dfs);
                ofd.Filter += "|All files|*.*";
            }

            // Simply appends the listed formats for the save dialog filters, as we can't save as "Supported files"/"All files"
            public static void SetSaveDialogFilters(SaveFileDialog sfd, DialogFormat[] dfs)
            {
                sfd.Filter = "";
                AppendFileDialogFilters(sfd, dfs);
            }
        }

        DialogFormat[] videoFormats =
        {
            new DialogFormat("MPEG-4 Video", "*.mp4"),
            new DialogFormat("Flash Video", "*.flv"),
            new DialogFormat("Audio Video Interleave", "*.avi"),
            new DialogFormat("Matroska Video", "*.mkv"),
            new DialogFormat("QuickTime Video", "*.mov"),
        };

        DialogFormat[] audioFormats =
        {
            new DialogFormat("MPEG-4 Audio", "*.m4a"),
            new DialogFormat("MPEG-3 Audio", "*.mp3"),
            new DialogFormat("Waveform Audio", "*.wav"),
        };

        public MainForm()
        {
            InitializeComponent();

            if (Program.useDarkTheme)
            {
                Color dark = Color.FromArgb(255, 32, 32, 32);
                Color light = Color.FromArgb(255, 224, 224, 224);

                BackColor = dark;

                // These aren't in Controls for whatever reason...
                textBoxTrimFrom.ForeColor = light;
                textBoxTrimFrom.BackColor = dark;

                textBoxTrimTo.ForeColor = light;
                textBoxTrimTo.BackColor = dark;

                numericAudioDelay.ForeColor = light;
                numericAudioDelay.BackColor = dark;

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
                    else if (c is GroupBox)
                    {
                        c.ForeColor = light;
                    }
                    else if (c is CheckBox)
                    {
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
                    DwmSetWindowAttribute(Handle, darkModeAttribute, ref one, sizeof(int));
                }
            }

            Core.Initialize();

            // BUG: DirectSound is "slower", but MMDevice (default) makes it so both players use the same volume control
            vlc = new LibVLC("--aout=directsound");
            mpVideo = new MediaPlayer(vlc);
            mpAudio = new MediaPlayer(vlc);
            videoView.MediaPlayer = mpVideo;

            mpVideo.Buffering += new EventHandler<MediaPlayerBufferingEventArgs>(mpVideo_Buffering);
            mpVideo.EndReached += new EventHandler<EventArgs>(mpVideo_EndReached);
            mpVideo.EncounteredError += new EventHandler<EventArgs>(mpVideo_EncounteredError);
            mpVideo.Opening += new EventHandler<EventArgs>(mpVideo_Opening);
            mpVideo.Paused += new EventHandler<EventArgs>(mpVideo_Paused);
            mpVideo.Playing += new EventHandler<EventArgs>(mpVideo_Playing);
            mpVideo.Stopped += new EventHandler<EventArgs>(mpVideo_Stopped);
            mpVideo.TimeChanged += new EventHandler<MediaPlayerTimeChangedEventArgs>(mpVideo_TimeChanged);

            // Probably not necessary... for now?
            //mpAudio.Buffering += new EventHandler<MediaPlayerBufferingEventArgs>(mpAudio_Buffering);
            mpAudio.EncounteredError += new EventHandler<EventArgs>(mpAudio_EncounteredError);

            volumeBarVideo.Value = mpVideoVolume;
            volumeBarAudio.Value = mpAudioVolume;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+P, Space, Play/Pause
            if (keyData == (Keys.Control | Keys.P) || keyData == Keys.Space || keyData == Keys.MediaPlayPause)
            {
                buttonPlay.PerformClick();
                return true;
            }

            // Ctrl+L
            else if (keyData == (Keys.Control | Keys.L))
            {
                checkBoxPLMenu.Checked = !checkBoxPLMenu.Checked;
                return true;
            }

            // Ctrl+O
            else if (keyData == (Keys.Control | Keys.O))
            {
                buttonLoad.PerformClick();
                return true;
            }

            // Ctrl+S
            else if (keyData == (Keys.Control | Keys.S))
            {
                buttonSave.PerformClick();
                return true;
            }

            // Ctrl+T
            else if (keyData == (Keys.Control | Keys.T))
            {
                checkBoxAOT.Checked = !checkBoxAOT.Checked;
                return true;
            }

            // Right arrow
            else if (keyData == Keys.Right)
            {
                int newValue = seekBar.Value + 1000;
                if (newValue > seekBar.Maximum)
                {
                    newValue = seekBar.Maximum;
                }
                seekBar.Value = newValue;
                SeekAVTo(seekBar.Value);

                return true;
            }

            // Left arrow
            else if (keyData == Keys.Left)
            {
                int newValue = seekBar.Value - 1000;
                if (newValue < 0)
                {
                    newValue = 0;
                }
                seekBar.Value = newValue;
                SeekAVTo(seekBar.Value);

                return true;
            }

            else if (keyData == Keys.Oemcomma && !mpVideo.IsPlaying)
            {
                float msPerFrame = 1000 / mpVideo.Fps;
                int newTime = (int)mpVideo.Time - (int)msPerFrame;
                SeekAVTo(newTime);

                return true;
            }

            else if (keyData == Keys.OemPeriod && !mpVideo.IsPlaying)
            {
                float msPerFrame = 1000 / mpVideo.Fps;
                int newTime = (int)mpVideo.Time + (int)msPerFrame;
                SeekAVTo(newTime);

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // God I fucking hate C#
        private void InvokeControl(Control c, MethodInvoker me)
        {
            if (isClosing)
            {
                return;
            }

            if (this.IsDisposed)
            {
                return;
            }

            if (c.IsDisposed)
            {
                return;
            }

            if (c.InvokeRequired)
            {
                c.Invoke(me);
            }
            else
            {
                me();
            }
        }

        private void mpVideo_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            if (seeking)
            {
                return;
            }

            mpAudio.Volume = mpAudioVolume;
            mpVideo.Volume = mpVideoVolume;

            if (e.Cache < 100.0)
            {
                // DO NOT CALL InvokeControl HERE UNDER ANY CIRCUMSTANCES

                mpAudio.SetPause(true);
            }
            else
            {
                if (mpVideo.IsPlaying)
                {
                    mpAudio.SetPause(false);
                }
            }
        }

        private void mpVideo_EndReached(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Play";
            });

            mpAudio.SetPause(true);
        }

        private void mpVideo_EncounteredError(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Play";
            });

            mpAudio.Stop();
            mpVideo.Stop();

            MessageBox.Show("An unknown error occurred during video playback.", "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void mpVideo_Opening(object sender, EventArgs e)
        {
            // DO NOT CALL InvokeControl HERE UNDER ANY CIRCUMSTANCES

            mpAudio.SetPause(true);
        }

        private void mpVideo_Paused(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Play";
            });

            mpAudio.SetPause(true);
        }

        private void mpVideo_Playing(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Pause";
            });

            mpAudio.Volume = mpAudioVolume;
            mpVideo.Volume = mpVideoVolume;

            mpAudio.SetPause(false);
        }

        private void mpVideo_Stopped(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Play";
            });

            mpAudio.SetPause(true);
        }

        private void UpdateLabelTime()
        {
            // I can't even venture to fucking guess why I need this
            if (mpVideo != null && mpVideo.Time != -1)
            {
                TimeSpan elapsedTime = new TimeSpan(mpVideo.Time * 10000);
                TimeSpan maxTime = new TimeSpan(mpVideo.Media.Duration * 10000);

                labelTime.Text = elapsedTime.ToString(@"m\:ss\.fff") + " / " + maxTime.ToString(@"m\:ss\.fff");
            }
            else
            {
                labelTime.Text = "0:00.000 / 0:00.000";
            }
        }

        private void mpVideo_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            // Update our seek bar to the current video clip time
            InvokeControl(seekBar, delegate ()
            {
                seekBar.Value = (int)e.Time;
            });

            InvokeControl(labelTime, delegate ()
            {
                UpdateLabelTime();
            });

            // Video time has surpassed audio delay - at this point, our audio clip should be heard
            if ((int)e.Time >= mpAudioDelay)
            {
                // If we need to perform a catch-up, seek the audio clip to our designated time, and unpause it
                if (mpAudioCatchUp)
                {
                    mpAudio.SeekTo(TimeSpan.FromMilliseconds(e.Time - mpAudioDelay));
                    mpAudio.SetPause(false);

                    // We only need to perform this catch-up once upon request, let the media players do their thing afterwards so we aren't seeking all the time
                    mpAudioCatchUp = false;
                }
            }
            else
            {
                // If the video time has not surpassed our audio delay, we should wait for the video clip to catch up, pausing the audio clip in the meantime
                mpAudioCatchUp = true;

                mpAudio.SetPause(true);
            }
        }

        private void mpAudio_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            if (seeking)
            {
                return;
            }

            if (e.Cache >= 100.0)
            {
                mpAudio.SeekTo(TimeSpan.FromMilliseconds((int)mpVideo.Time - mpAudioDelay));
            }
        }

        private void mpAudio_EncounteredError(object sender, EventArgs e)
        {
            InvokeControl(buttonPlay, delegate ()
            {
                buttonPlay.Text = "Play";
            });

            mpAudio.Stop();
            mpVideo.Stop();

            MessageBox.Show("An unknown error occurred during audio playback.", "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.Location;
            Size = Properties.Settings.Default.Size;
        }

        private void SeekAVTo(int ms)
        {
            mpVideo.SeekTo(TimeSpan.FromMilliseconds(ms));
            mpAudio.SeekTo(TimeSpan.FromMilliseconds(ms - mpAudioDelay));

            UpdateLabelTime();
        }

        // HACK: When we click on the TrackBar freely, seek to the cursor's position
        private void UpdateSeek(MouseEventArgs e)
        {
            double newValue = (((double)e.X - 13.0) / ((double)seekBar.Width - 26.0)) * (seekBar.Maximum - seekBar.Minimum);

            if (newValue < 0)
            {
                newValue = 0;
            }
            if (newValue > seekBar.Maximum)
            {
                newValue = seekBar.Maximum;
            }

            seekBar.Value = Convert.ToInt32(newValue);
            SeekAVTo(seekBar.Value);
        }

        // Actively seek media players as we're playing with the trackbar
        private void seekBar_Scroll(object sender, EventArgs e)
        {
            SeekAVTo(seekBar.Value);
        }

        // Pause the media players while we're seeking
        private void seekBar_MouseDown(object sender, MouseEventArgs e)
        {
            seeking = true;

            UpdateSeek(e);

            if (mpVideo.IsPlaying)
            {
                unpauseAfterSeeking = true;

                mpVideo.SetPause(true);
                mpAudio.SetPause(true);
            }
        }

        // And unpause them when we're done
        private void seekBar_MouseUp(object sender, MouseEventArgs e)
        {
            seeking = false;

            if (mpVideo.State == VLCState.Ended || mpVideo.State == VLCState.Error)
            {
                mpVideo.Stop();
                mpAudio.Stop();

                mpVideo.Play();
                mpAudio.Play();
            }

            UpdateSeek(e);

            if (unpauseAfterSeeking)
            {
                unpauseAfterSeeking = false;

                mpVideo.SetPause(false);
                mpAudio.SetPause(false);
            }
        }

        // HACK: When we click on the TrackBar freely, MouseMove gets called instead of Scroll
        private void seekBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateSeek(e);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent Invoke() methods from calling
            isClosing = true;

            // Save current window location and size for next startup
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Location = Location;
                Properties.Settings.Default.Size = Size;
            }
            else
            {
                Properties.Settings.Default.Location = RestoreBounds.Location;
                Properties.Settings.Default.Size = RestoreBounds.Size;
            }
            Properties.Settings.Default.Save();
        }

        private void volumeBarAudio_Scroll(object sender, EventArgs e)
        {
            mpAudio.Volume = mpAudioVolume = volumeBarAudio.Value;
        }

        private void volumeBarVideo_Scroll(object sender, EventArgs e)
        {
            mpVideo.Volume = mpVideoVolume = volumeBarVideo.Value;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            mpVideo.SetPause(true);
            mpAudio.SetPause(true);

            openAVDialog.InitialDirectory = Properties.Settings.Default.LastLoadFolder;

            DialogFormat.SetOpenDialogFilters(openAVDialog, videoFormats);

            if (openAVDialog.ShowDialog() == DialogResult.OK)
            {
                string videoFile = openAVDialog.FileName;
                string audioFile = "";

                // Remember the last folder we attempted to load a file from
                Properties.Settings.Default.LastLoadFolder = Path.GetDirectoryName(videoFile);
                Properties.Settings.Default.Save();

                // Path.GetFileNameWithoutExtension(videoFile) doesn't include the path - manual labor required
                int index = videoFile.LastIndexOf('.');
                string fileNameNoExt = index == -1 ? videoFile : videoFile.Substring(0, index);

                foreach (DialogFormat audioFormat in audioFormats)
                {
                    int extIndex = audioFormat.ext.LastIndexOf('.');
                    string ext = extIndex == -1 ? audioFormat.ext : audioFormat.ext.Substring(extIndex, audioFormat.ext.Length - 1);

                    if (File.Exists(fileNameNoExt + ext))
                    {
                        audioFile = fileNameNoExt + ext;
                        break;
                    }
                }

                if (audioFile == "")
                {
                    if (MessageBox.Show("No audio file associated with this video was found.\n\nDo you want to try loading another audio file?", "Phi(C)lipHelper", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        DialogFormat.SetOpenDialogFilters(openAVDialog, audioFormats);
                        if (openAVDialog.ShowDialog() == DialogResult.OK)
                        {
                            audioFile = openAVDialog.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                Media mediaVideo = new Media(vlc, videoFile);
                mediaVideo.Parse();
                if (mediaVideo.IsParsed)
                {
                    if (mediaVideo.ParsedStatus != MediaParsedStatus.Done)
                    {
                        MessageBox.Show("Failed to parse video: " + mediaVideo.ParsedStatus.ToString(), "Phi(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                Media mediaAudio = new Media(vlc, audioFile);
                mediaAudio.Parse();
                if (mediaAudio.IsParsed)
                {
                    if (mediaAudio.ParsedStatus != MediaParsedStatus.Done)
                    {
                        MessageBox.Show("Failed to parse audio: " + mediaAudio.ParsedStatus.ToString(), "Phi(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                mpVideoFile = videoFile;
                mpAudioFile = audioFile;

                // Wait until the video duration has been parsed and is available (unrelated to parsing the media descriptors)
                int dur = (int)mediaVideo.Duration;
                while (dur == -1)
                {
                    dur = (int)mediaVideo.Duration;
                }

                seekBar.Maximum = dur;
                seekBar.Value = 0;
                SeekAVTo(0);

                numericAudioDelay.Value = 0;
                textBoxTrimFrom.Text = "";
                textBoxTrimTo.Text = "";

                Text = "Phi(C)lipHelper - " + Path.GetFileNameWithoutExtension(videoFile);

                mpAudio.Volume = volumeBarAudio.Value;
                mpVideo.Volume = volumeBarVideo.Value;

                mpVideo.Play(mediaVideo);
                mpAudio.Play(mediaAudio);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (mpVideo.State == VLCState.Ended || mpVideo.State == VLCState.Error)
            {
                seekBar.Value = 0;
                SeekAVTo(0);

                mpVideo.Stop();
                mpAudio.Stop();

                mpVideo.Play();
                mpAudio.Play();
            }
            else if (mpVideo.State == VLCState.Playing)
            {
                mpVideo.SetPause(true);
                mpAudio.SetPause(true);
            }
            else if (mpVideo.State == VLCState.Paused)
            {
                mpVideo.SetPause(false);
                mpAudio.SetPause(false);
            }
        }

        private void buttonPlay_TextChanged(object sender, EventArgs e)
        {
            if (buttonPlay.Text == "Pause")
            {
                buttonPlay.Image = PhilClipHelper.Properties.Resources.pause_solid_full;
            }
            else if (buttonPlay.Text == "Play")
            {
                buttonPlay.Image = PhilClipHelper.Properties.Resources.play_solid_full;
            }
        }

        private void checkBoxAOT_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBoxAOT.Checked;
        }

        private void numericAudioDelay_ValueChanged(object sender, EventArgs e)
        {
            mpAudioDelay = (int)numericAudioDelay.Value;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            mpVideo.SetPause(true);
            mpAudio.SetPause(true);

            saveVideoDialog.InitialDirectory = Properties.Settings.Default.LastSaveFolder;

            saveVideoDialog.FileName = Path.GetFileNameWithoutExtension(mpVideoFile);

            DialogFormat.SetSaveDialogFilters(saveVideoDialog, videoFormats);

            if (saveVideoDialog.ShowDialog() == DialogResult.OK)
            {
                string videoFile = saveVideoDialog.FileName;

                // User was prompted to overwrite, and accepted - delete the original file
                // FFmpeg has been supplied with the '-n' parameter, meaning it won't prompt to overwrite files and simply fail
                // (this is done so that the FFmpeg CLI won't require manual input)
                if (File.Exists(videoFile))
                {
                    File.Delete(videoFile);
                }

                // Remember the last folder we attempted to save a file in
                Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(videoFile);
                Properties.Settings.Default.Save();

                string args = "-n -i \"" + mpVideoFile + "\" -i \"" + mpAudioFile + "\" -filter_complex \"[1:a]volume=8,adelay=" + mpAudioDelay + "|" + mpAudioDelay + "[idk];[0:a][idk]amix=inputs=2[aout]\" -map \"[aout]\":a -map 0:v";

                if (textBoxTrimFrom.Text != "" && textBoxTrimTo.Text != "")
                {
                    args += " -ss " + textBoxTrimFrom.Text + " -to " + textBoxTrimTo.Text;
                }

                args += " -c:v copy -c:a aac \"" + videoFile + "\"";

                new SaveForm(args, videoFile, Location, Size).ShowDialog();
            }
        }

        private void numericAudioDelay_Enter(object sender, EventArgs e)
        {
            mpVideo.SetPause(true);
            mpAudio.SetPause(true);
        }
    }
}
