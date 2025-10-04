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
    public partial class MainForm : Form, IPchForm
    {
        private bool _isClosing = false;

        public bool /* IPchForm */ IsClosing() 
        {
            return _isClosing;
        }

        enum PlayPauseButton
        {
            Play,
            Pause,
        }

        private MediaPlayerManager _mpm;

        private bool _unpauseAfterSeeking = false;

        public MainForm(MediaPlayerManager mpm)
        {
            _mpm = mpm;

            InitializeComponent();

            buttonPlay.Tag = PlayPauseButton.Play;
            videoView.MediaPlayer = _mpm.VideoPlayer;

            if (Program.UseDarkTheme)
            {
                BackColor = Program.DarkTheme.BackColor;

                // These aren't in Controls for whatever reason...
                textBoxTrimFrom.ForeColor = Program.DarkTheme.ForeColor;
                textBoxTrimFrom.BackColor = Program.DarkTheme.BackColor;

                textBoxTrimTo.ForeColor = Program.DarkTheme.ForeColor;
                textBoxTrimTo.BackColor = Program.DarkTheme.BackColor;

                numericAudioDelay.ForeColor = Program.DarkTheme.ForeColor;
                numericAudioDelay.BackColor = Program.DarkTheme.BackColor;

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
                    else if (c is GroupBox)
                    {
                        c.ForeColor = Program.DarkTheme.ForeColor;
                    }
                    else if (c is CheckBox)
                    {
                        c.ForeColor = Program.DarkTheme.ForeColor;
                    }
                }

                Program.EnableDarkTitlebar(this);
            }

            _mpm.VideoBuffering += new EventHandler<MediaPlayerBufferingEventArgs>(VideoPlayerBuffering);
            _mpm.VideoEndReached += new EventHandler<EventArgs>(VideoPlayerEndReached);
            _mpm.VideoOpening += new EventHandler<EventArgs>(VideoPlayerOpening);
            _mpm.VideoPaused += new EventHandler<EventArgs>(VideoPlayerPaused);
            _mpm.VideoPlaying += new EventHandler<EventArgs>(VideoPlayerPlaying);
            _mpm.VideoStopped += new EventHandler<EventArgs>(VideoPlayerStopped);
            _mpm.VideoTimeChanged += new EventHandler<MediaPlayerTimeChangedEventArgs>(VideoPlayerTimeChanged);

            volumeBarVideo.Value = _mpm.VideoVolume;
            volumeBarAudio.Value = _mpm.AudioVolume;

            labelStatus.Text = "";
        }

        private void UpdatePlayPauseButton(PlayPauseButton name)
        {
            buttonPlay.Tag = name;

            if (name == PlayPauseButton.Play)
            {
                buttonPlay.Text = "Play";
                buttonPlay.Image = PhilClipHelper.Properties.Resources.play_solid_full;
            }
            else if (name == PlayPauseButton.Pause)
            {
                buttonPlay.Text = "Pause";
                buttonPlay.Image = PhilClipHelper.Properties.Resources.pause_solid_full;
            }

            buttonPlay.Refresh();
        }

        private void UpdateTimeLabel(long time)
        {
            TimeSpan elapsedTime = new TimeSpan(time * 10000);
            TimeSpan maxTime = new TimeSpan(_mpm.VideoDuration * 10000);

            labelTime.Text = elapsedTime.ToString(@"m\:ss\.fff") + " / " + maxTime.ToString(@"m\:ss\.fff");
            labelTime.Refresh();
        }

        private void PlayAudioVideo()
        {
            _mpm.Play();
            UpdatePlayPauseButton(PlayPauseButton.Pause);
        }

        private void StopAudioVideo()
        {
            _mpm.Stop();
            UpdatePlayPauseButton(PlayPauseButton.Play);
        }

        private void PauseAudioVideo()
        {
            _mpm.Pause();
            UpdatePlayPauseButton(PlayPauseButton.Play);
        }

        private void UnpauseAudioVideo()
        {
            _mpm.Unpause();
            UpdatePlayPauseButton(PlayPauseButton.Pause);
        }

        private void SeekAudioVideo(long ms)
        {
            _mpm.Seek(ms);
            UpdateTimeLabel(ms);
        }

        private bool IsAudioVideoPlaying()
        {
            if ((PlayPauseButton)buttonPlay.Tag == PlayPauseButton.Play)
            {
                return false;
            }
            else if ((PlayPauseButton)buttonPlay.Tag == PlayPauseButton.Pause)
            {
                return true;
            }

            throw new Exception("IsAudioVideoPlaying is indeterminate.");
        }

        private void VideoPlayerBuffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            Program.ControlBeginInvoke(labelStatus, new MethodInvoker(delegate ()
            {
                if (e.Cache < 100.0)
                {
                    labelStatus.Text = "Buffering... " + Math.Round(e.Cache, 0) + "%";
                }
                else
                {
                    labelStatus.Text = "";
                }
                labelStatus.Refresh();
            }));
        }

        private void VideoPlayerEndReached(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(buttonPlay, new MethodInvoker(delegate ()
            {
                UpdatePlayPauseButton(PlayPauseButton.Play);
            }));
        }

        private void VideoPlayerOpening(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(labelStatus, new MethodInvoker(delegate ()
            {
                labelStatus.Text = "Opening...";
                labelStatus.Refresh();
            }));
        }

        private void VideoPlayerPaused(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(buttonPlay, new MethodInvoker(delegate ()
            {
                UpdatePlayPauseButton(PlayPauseButton.Play);
            }));
        }

        private void VideoPlayerPlaying(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(buttonPlay, new MethodInvoker(delegate ()
            {
                UpdatePlayPauseButton(PlayPauseButton.Pause);
            }));
        }

        private void VideoPlayerStopped(object sender, EventArgs e)
        {
            Program.ControlBeginInvoke(buttonPlay, new MethodInvoker(delegate ()
            {
                UpdatePlayPauseButton(PlayPauseButton.Play);
            }));
        }

        private void VideoPlayerTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            Program.ControlBeginInvoke(labelTime, new MethodInvoker(delegate ()
            {
                UpdateTimeLabel(e.Time);
            }));

            Program.ControlBeginInvoke(seekBar, new MethodInvoker(delegate ()
            {
                seekBar.Value = (int)e.Time;
            }));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+P, Space, Play/Pause
            if (keyData == (Keys.Control | Keys.P) || keyData == Keys.Space || keyData == Keys.MediaPlayPause)
            {
                buttonPlay.PerformClick();
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
                long seekTime = seekBar.Value + 1000;
                if (seekTime > seekBar.Maximum)
                {
                    seekTime = seekBar.Maximum;
                }

                seekBar.Value = (int)seekTime;
                SeekAudioVideo(seekTime);
                return true;
            }

            // Left arrow
            else if (keyData == Keys.Left)
            {
                long seekTime = seekBar.Value - 1000;
                if (seekTime > seekBar.Maximum)
                {
                    seekTime = seekBar.Maximum;
                }

                seekBar.Value = (int)seekTime;
                SeekAudioVideo(seekTime);
                return true;
            }

            // Comma (,)
            else if (keyData == Keys.Oemcomma && !IsAudioVideoPlaying())
            {
                long seekTime = _mpm.VideoTime - (int)_mpm.VideoFrameTime;
                if (seekTime < 0)
                {
                    seekTime = 0;
                }

                seekBar.Value = (int)seekTime;
                SeekAudioVideo(seekTime);
                return true;
            }

            // Period (.)
            else if (keyData == Keys.OemPeriod && !IsAudioVideoPlaying())
            {
                long seekTime = _mpm.VideoTime + (int)_mpm.VideoFrameTime;
                if (seekTime > seekBar.Maximum)
                {
                    seekTime = seekBar.Maximum;
                }

                seekBar.Value = (int)seekTime;
                SeekAudioVideo(seekTime);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.Location;
            Size = Properties.Settings.Default.Size;
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

            // FIXME: mousedown deadlock
            SeekAudioVideo(seekBar.Value = Convert.ToInt32(newValue));
        }

        // Actively seek media players as we're playing with the trackbar
        private void seekBar_Scroll(object sender, EventArgs e)
        {
            SeekAudioVideo(seekBar.Value);
        }

        // Pause the media players while we're seeking
        private void seekBar_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateSeek(e);

            if (IsAudioVideoPlaying())
            {
                _unpauseAfterSeeking = true;
                PauseAudioVideo();
            }
        }

        // And unpause them when we're done
        private void seekBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mpm.VideoTime == -1)
            {
                PlayAudioVideo();
            }

            UpdateSeek(e);

            if (_unpauseAfterSeeking)
            {
                _unpauseAfterSeeking = false;
                UnpauseAudioVideo();
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
            _isClosing = true;

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
            _mpm.AudioVolume = volumeBarAudio.Value;
        }

        private void volumeBarVideo_Scroll(object sender, EventArgs e)
        {
            _mpm.VideoVolume = volumeBarVideo.Value;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            PauseAudioVideo();

            openAVDialog.InitialDirectory = Properties.Settings.Default.LastLoadFolder;
            DialogFormat.SetOpenDialogFilters(openAVDialog, DialogFormat.VideoFormats);

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

                foreach (DialogFormat audioFormat in DialogFormat.AudioFormats)
                {
                    int extIndex = audioFormat.Pattern.LastIndexOf('.');
                    string ext = extIndex == -1 ? audioFormat.Pattern : audioFormat.Pattern.Substring(extIndex, audioFormat.Pattern.Length - 1);

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
                        DialogFormat.SetOpenDialogFilters(openAVDialog, DialogFormat.AudioFormats);
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

                if (_mpm.Open(videoFile, audioFile))
                {
                    // HACK: please fucking nuke me first chance you get because i don't know how the media descriptor can be parsed while the duration is still -1
                    long duration = _mpm.VideoDuration;
                    while (duration < 0)
                    {
                        duration = _mpm.VideoDuration;
                    }

                    Text = "Phi(C)lipHelper - " + Path.GetFileNameWithoutExtension(videoFile);
                    seekBar.Maximum = (int)_mpm.VideoDuration;
                    numericAudioDelay.Value = 0;
                    textBoxTrimFrom.Text = "";
                    textBoxTrimTo.Text = "";

                    PlayAudioVideo();
                    SeekAudioVideo(seekBar.Value = 0);
                }
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (IsAudioVideoPlaying())
            {
                PauseAudioVideo();
                
            }
            else
            {
                PlayAudioVideo();
                UnpauseAudioVideo();
            }
        }

        private void checkBoxAOT_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBoxAOT.Checked;
        }

        private void numericAudioDelay_ValueChanged(object sender, EventArgs e)
        {
            _mpm.AudioDelay = (int)numericAudioDelay.Value;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            PauseAudioVideo();

            saveVideoDialog.InitialDirectory = Properties.Settings.Default.LastSaveFolder;
            saveVideoDialog.FileName = Path.GetFileNameWithoutExtension(_mpm.VideoFile);
            DialogFormat.SetSaveDialogFilters(saveVideoDialog, DialogFormat.VideoFormats);

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

                string args = "-n -i \"" + _mpm.VideoFile + "\" -i \"" + _mpm.AudioFile + "\" -filter_complex \"[1:a]volume=8,adelay=" + _mpm.AudioDelay + "|" + _mpm.AudioDelay + "[idk];[0:a][idk]amix=inputs=2[aout]\" -map \"[aout]\":a -map 0:v";

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
            PauseAudioVideo();
        }
    }
}
