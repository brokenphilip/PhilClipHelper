namespace PhilClipHelper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.videoView = new LibVLCSharp.WinForms.VideoView();
            this.seekBar = new System.Windows.Forms.TrackBar();
            this.volumeBarAudio = new System.Windows.Forms.TrackBar();
            this.volumeBarVideo = new System.Windows.Forms.TrackBar();
            this.labelVideo = new System.Windows.Forms.Label();
            this.labelAudio = new System.Windows.Forms.Label();
            this.openAVDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxTrimVideo = new System.Windows.Forms.GroupBox();
            this.labelTo = new System.Windows.Forms.Label();
            this.labelFrom = new System.Windows.Forms.Label();
            this.textBoxTrimTo = new System.Windows.Forms.TextBox();
            this.textBoxTrimFrom = new System.Windows.Forms.TextBox();
            this.groupBoxAudioDelay = new System.Windows.Forms.GroupBox();
            this.labelMS = new System.Windows.Forms.Label();
            this.numericAudioDelay = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAOT = new System.Windows.Forms.CheckBox();
            this.checkBoxPLMenu = new System.Windows.Forms.CheckBox();
            this.saveVideoDialog = new System.Windows.Forms.SaveFileDialog();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.labelTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.videoView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seekBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBarAudio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBarVideo)).BeginInit();
            this.groupBoxTrimVideo.SuspendLayout();
            this.groupBoxAudioDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAudioDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // videoView
            // 
            this.videoView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoView.BackColor = System.Drawing.Color.Black;
            this.videoView.Location = new System.Drawing.Point(0, 0);
            this.videoView.MediaPlayer = null;
            this.videoView.Name = "videoView";
            this.videoView.Size = new System.Drawing.Size(640, 360);
            this.videoView.TabIndex = 0;
            this.videoView.TabStop = false;
            this.videoView.Text = "videoView1";
            // 
            // seekBar
            // 
            this.seekBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seekBar.LargeChange = 0;
            this.seekBar.Location = new System.Drawing.Point(0, 366);
            this.seekBar.Maximum = 0;
            this.seekBar.Name = "seekBar";
            this.seekBar.Size = new System.Drawing.Size(640, 45);
            this.seekBar.SmallChange = 0;
            this.seekBar.TabIndex = 2;
            this.seekBar.TabStop = false;
            this.seekBar.TickFrequency = 1000;
            this.seekBar.Scroll += new System.EventHandler(this.seekBar_Scroll);
            this.seekBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseDown);
            this.seekBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseMove);
            this.seekBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseUp);
            // 
            // volumeBarAudio
            // 
            this.volumeBarAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeBarAudio.LargeChange = 25;
            this.volumeBarAudio.Location = new System.Drawing.Point(220, 405);
            this.volumeBarAudio.Maximum = 100;
            this.volumeBarAudio.Name = "volumeBarAudio";
            this.volumeBarAudio.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.volumeBarAudio.Size = new System.Drawing.Size(45, 80);
            this.volumeBarAudio.SmallChange = 5;
            this.volumeBarAudio.TabIndex = 9;
            this.volumeBarAudio.TabStop = false;
            this.volumeBarAudio.TickFrequency = 25;
            this.volumeBarAudio.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.volumeBarAudio.Scroll += new System.EventHandler(this.volumeBarAudio_Scroll);
            // 
            // volumeBarVideo
            // 
            this.volumeBarVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeBarVideo.LargeChange = 25;
            this.volumeBarVideo.Location = new System.Drawing.Point(375, 405);
            this.volumeBarVideo.Maximum = 100;
            this.volumeBarVideo.Name = "volumeBarVideo";
            this.volumeBarVideo.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.volumeBarVideo.Size = new System.Drawing.Size(45, 80);
            this.volumeBarVideo.SmallChange = 5;
            this.volumeBarVideo.TabIndex = 10;
            this.volumeBarVideo.TabStop = false;
            this.volumeBarVideo.TickFrequency = 25;
            this.volumeBarVideo.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.volumeBarVideo.Scroll += new System.EventHandler(this.volumeBarVideo_Scroll);
            // 
            // labelVideo
            // 
            this.labelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVideo.AutoSize = true;
            this.labelVideo.Location = new System.Drawing.Point(379, 482);
            this.labelVideo.Name = "labelVideo";
            this.labelVideo.Size = new System.Drawing.Size(34, 13);
            this.labelVideo.TabIndex = 13;
            this.labelVideo.Text = "Video";
            // 
            // labelAudio
            // 
            this.labelAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAudio.AutoSize = true;
            this.labelAudio.Location = new System.Drawing.Point(225, 482);
            this.labelAudio.Name = "labelAudio";
            this.labelAudio.Size = new System.Drawing.Size(34, 13);
            this.labelAudio.TabIndex = 14;
            this.labelAudio.Text = "Audio";
            // 
            // openAVDialog
            // 
            this.openAVDialog.DefaultExt = "mp4";
            this.openAVDialog.FileName = "openFileDialog1";
            this.openAVDialog.Filter = "Video files|*.mp4|All files|*.*";
            this.openAVDialog.Title = "Open Audio/Video file";
            // 
            // groupBoxTrimVideo
            // 
            this.groupBoxTrimVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTrimVideo.Controls.Add(this.labelTo);
            this.groupBoxTrimVideo.Controls.Add(this.labelFrom);
            this.groupBoxTrimVideo.Controls.Add(this.textBoxTrimTo);
            this.groupBoxTrimVideo.Controls.Add(this.textBoxTrimFrom);
            this.groupBoxTrimVideo.Location = new System.Drawing.Point(426, 399);
            this.groupBoxTrimVideo.Name = "groupBoxTrimVideo";
            this.groupBoxTrimVideo.Size = new System.Drawing.Size(98, 96);
            this.groupBoxTrimVideo.TabIndex = 15;
            this.groupBoxTrimVideo.TabStop = false;
            this.groupBoxTrimVideo.Text = "Trim Video";
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(4, 55);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(23, 13);
            this.labelTo.TabIndex = 18;
            this.labelTo.Text = "To:";
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(4, 14);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(33, 13);
            this.labelFrom.TabIndex = 17;
            this.labelFrom.Text = "From:";
            // 
            // textBoxTrimTo
            // 
            this.textBoxTrimTo.Location = new System.Drawing.Point(6, 70);
            this.textBoxTrimTo.Name = "textBoxTrimTo";
            this.textBoxTrimTo.Size = new System.Drawing.Size(86, 20);
            this.textBoxTrimTo.TabIndex = 1;
            this.textBoxTrimTo.TabStop = false;
            // 
            // textBoxTrimFrom
            // 
            this.textBoxTrimFrom.Location = new System.Drawing.Point(6, 29);
            this.textBoxTrimFrom.Name = "textBoxTrimFrom";
            this.textBoxTrimFrom.Size = new System.Drawing.Size(86, 20);
            this.textBoxTrimFrom.TabIndex = 0;
            this.textBoxTrimFrom.TabStop = false;
            // 
            // groupBoxAudioDelay
            // 
            this.groupBoxAudioDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAudioDelay.Controls.Add(this.labelMS);
            this.groupBoxAudioDelay.Controls.Add(this.numericAudioDelay);
            this.groupBoxAudioDelay.Location = new System.Drawing.Point(116, 399);
            this.groupBoxAudioDelay.Name = "groupBoxAudioDelay";
            this.groupBoxAudioDelay.Size = new System.Drawing.Size(98, 49);
            this.groupBoxAudioDelay.TabIndex = 16;
            this.groupBoxAudioDelay.TabStop = false;
            this.groupBoxAudioDelay.Text = "Audio Delay";
            // 
            // labelMS
            // 
            this.labelMS.AutoSize = true;
            this.labelMS.Location = new System.Drawing.Point(69, 22);
            this.labelMS.Name = "labelMS";
            this.labelMS.Size = new System.Drawing.Size(20, 13);
            this.labelMS.TabIndex = 1;
            this.labelMS.Text = "ms";
            // 
            // numericAudioDelay
            // 
            this.numericAudioDelay.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericAudioDelay.Location = new System.Drawing.Point(6, 19);
            this.numericAudioDelay.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericAudioDelay.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.numericAudioDelay.Name = "numericAudioDelay";
            this.numericAudioDelay.Size = new System.Drawing.Size(57, 20);
            this.numericAudioDelay.TabIndex = 0;
            this.numericAudioDelay.TabStop = false;
            this.numericAudioDelay.ThousandsSeparator = true;
            this.numericAudioDelay.ValueChanged += new System.EventHandler(this.numericAudioDelay_ValueChanged);
            this.numericAudioDelay.Enter += new System.EventHandler(this.numericAudioDelay_Enter);
            // 
            // checkBoxAOT
            // 
            this.checkBoxAOT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAOT.AutoSize = true;
            this.checkBoxAOT.Location = new System.Drawing.Point(116, 454);
            this.checkBoxAOT.Name = "checkBoxAOT";
            this.checkBoxAOT.Size = new System.Drawing.Size(98, 17);
            this.checkBoxAOT.TabIndex = 17;
            this.checkBoxAOT.TabStop = false;
            this.checkBoxAOT.Text = "Always On Top";
            this.checkBoxAOT.UseVisualStyleBackColor = true;
            this.checkBoxAOT.CheckedChanged += new System.EventHandler(this.checkBoxAOT_CheckedChanged);
            // 
            // checkBoxPLMenu
            // 
            this.checkBoxPLMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxPLMenu.AutoSize = true;
            this.checkBoxPLMenu.Location = new System.Drawing.Point(116, 478);
            this.checkBoxPLMenu.Name = "checkBoxPLMenu";
            this.checkBoxPLMenu.Size = new System.Drawing.Size(88, 17);
            this.checkBoxPLMenu.TabIndex = 18;
            this.checkBoxPLMenu.TabStop = false;
            this.checkBoxPLMenu.Text = "Playlist Menu";
            this.checkBoxPLMenu.UseVisualStyleBackColor = true;
            // 
            // saveVideoDialog
            // 
            this.saveVideoDialog.DefaultExt = "mp4";
            this.saveVideoDialog.Filter = "MP4 video|*.mp4|All files|*.*";
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.Image = global::PhilClipHelper.Properties.Resources.play_solid_full;
            this.buttonPlay.Location = new System.Drawing.Point(271, 405);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(98, 90);
            this.buttonPlay.TabIndex = 5;
            this.buttonPlay.TabStop = false;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.TextChanged += new System.EventHandler(this.buttonPlay_TextChanged);
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Image = global::PhilClipHelper.Properties.Resources.download_solid_full;
            this.buttonSave.Location = new System.Drawing.Point(530, 405);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(98, 90);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.TabStop = false;
            this.buttonSave.Text = "Save Video";
            this.buttonSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoad.Image = global::PhilClipHelper.Properties.Resources.upload_solid_full;
            this.buttonLoad.Location = new System.Drawing.Point(12, 405);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(98, 90);
            this.buttonLoad.TabIndex = 3;
            this.buttonLoad.TabStop = false;
            this.buttonLoad.Text = "Load A/V";
            this.buttonLoad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTime.Location = new System.Drawing.Point(258, 389);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(121, 13);
            this.labelTime.TabIndex = 19;
            this.labelTime.Text = "0:00.000 / 0:00.000";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 507);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.checkBoxPLMenu);
            this.Controls.Add(this.checkBoxAOT);
            this.Controls.Add(this.groupBoxAudioDelay);
            this.Controls.Add(this.groupBoxTrimVideo);
            this.Controls.Add(this.labelAudio);
            this.Controls.Add(this.labelVideo);
            this.Controls.Add(this.volumeBarVideo);
            this.Controls.Add(this.volumeBarAudio);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.seekBar);
            this.Controls.Add(this.videoView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(656, 546);
            this.Name = "MainForm";
            this.Text = "Phil(C)lipHelper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seekBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBarAudio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBarVideo)).EndInit();
            this.groupBoxTrimVideo.ResumeLayout(false);
            this.groupBoxTrimVideo.PerformLayout();
            this.groupBoxAudioDelay.ResumeLayout(false);
            this.groupBoxAudioDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAudioDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView;
        private System.Windows.Forms.TrackBar seekBar;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.TrackBar volumeBarAudio;
        private System.Windows.Forms.TrackBar volumeBarVideo;
        private System.Windows.Forms.Label labelVideo;
        private System.Windows.Forms.Label labelAudio;
        private System.Windows.Forms.OpenFileDialog openAVDialog;
        private System.Windows.Forms.GroupBox groupBoxTrimVideo;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.TextBox textBoxTrimTo;
        private System.Windows.Forms.TextBox textBoxTrimFrom;
        private System.Windows.Forms.GroupBox groupBoxAudioDelay;
        private System.Windows.Forms.Label labelMS;
        private System.Windows.Forms.NumericUpDown numericAudioDelay;
        private System.Windows.Forms.CheckBox checkBoxAOT;
        private System.Windows.Forms.CheckBox checkBoxPLMenu;
        private System.Windows.Forms.SaveFileDialog saveVideoDialog;
        private System.Windows.Forms.Label labelTime;
    }
}

