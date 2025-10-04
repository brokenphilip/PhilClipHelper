using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhilClipHelper
{
    public class MediaPlayerManager
    {
        static private LibVLC _vlc = null;

        private class MediaPlayerInstance
        {
            public MediaPlayer MediaPlayer;
            public string FileName;
            public int Volume;

            public MediaPlayerInstance(int volume)
            {
                MediaPlayer = new MediaPlayer(_vlc);
                Volume = volume;
            }
        }

        private MediaPlayerInstance _video;
        private MediaPlayerInstance _audio;

        public string VideoFile
        { 
            get => _video.FileName;
        }

        public string AudioFile
        {
            get => _audio.FileName;
        }

        public int VideoVolume
        {
            get => _video.Volume;
            set => _video.Volume = _video.MediaPlayer.Volume = value;
        }

        public int AudioVolume
        {
            get => _audio.Volume;
            set => _audio.Volume = _audio.MediaPlayer.Volume = value;
        }

        public long VideoTime
        {
            get => _video.MediaPlayer.Time;
        }

        public long VideoDuration
        {
            get
            {
                Media media = _video.MediaPlayer.Media;
                if (media == null)
                {
                    return -2;
                }
                return media.Duration;
            }
        }

        public float VideoFrameTime
        {
            get => 1000 / _video.MediaPlayer.Fps;
        }

        public MediaPlayer VideoPlayer
        { 
            get => _video.MediaPlayer;
        }

        private int _audioDelayDelta = 0;

        private int _audioDelay = 0;
        public int AudioDelay
        {
            get => _audioDelay;
            set => _audioDelay = value;
        }

        public event EventHandler<MediaPlayerBufferingEventArgs> VideoBuffering
        {
            add => _video.MediaPlayer.Buffering += value;
            remove => _video.MediaPlayer.Buffering -= value;
        }

        public event EventHandler<EventArgs> VideoEndReached
        {
            add => _video.MediaPlayer.EndReached += value;
            remove => _video.MediaPlayer.EndReached -= value;
        }

        public event EventHandler<EventArgs> VideoOpening
        {
            add => _video.MediaPlayer.Opening += value;
            remove => _video.MediaPlayer.Opening -= value;
        }

        public event EventHandler<EventArgs> VideoPaused
        {
            add => _video.MediaPlayer.Paused += value;
            remove => _video.MediaPlayer.Paused -= value;
        }

        public event EventHandler<EventArgs> VideoPlaying
        {
            add => _video.MediaPlayer.Playing += value;
            remove => _video.MediaPlayer.Playing -= value;
        }

        public event EventHandler<EventArgs> VideoStopped
        {
            add => _video.MediaPlayer.Stopped += value;
            remove => _video.MediaPlayer.Stopped -= value;
        }

        public event EventHandler<MediaPlayerTimeChangedEventArgs> VideoTimeChanged
        {
            add => _video.MediaPlayer.TimeChanged += value;
            remove => _video.MediaPlayer.TimeChanged -= value;
        }

        public MediaPlayerManager()
        {
            if (_vlc == null)
            {
                LibVLCSharp.Shared.Core.Initialize();

                // BUG: DirectSound is "slower", but MMDevice (default) makes it so both players use the same volume control
                _vlc = new LibVLC("--aout=directsound");
            }

            _video = new MediaPlayerInstance(50);
            _audio = new MediaPlayerInstance(100);

            _audio.MediaPlayer.EncounteredError += new EventHandler<EventArgs>(AudioPlayerEncounteredError);

            _video.MediaPlayer.Buffering += new EventHandler<MediaPlayerBufferingEventArgs>(VideoPlayerBuffering);
            _video.MediaPlayer.EndReached += new EventHandler<EventArgs>(VideoPlayerEndReached);
            _video.MediaPlayer.EncounteredError += new EventHandler<EventArgs>(VideoPlayerEncounteredError);
            _video.MediaPlayer.Opening += new EventHandler<EventArgs>(VideoPlayerOpening);
            _video.MediaPlayer.Paused += new EventHandler<EventArgs>(VideoPlayerPaused);
            _video.MediaPlayer.Playing += new EventHandler<EventArgs>(VideoPlayerPlaying);
            _video.MediaPlayer.Stopped += new EventHandler<EventArgs>(VideoPlayerStopped);
            _video.MediaPlayer.TimeChanged += new EventHandler<MediaPlayerTimeChangedEventArgs>(VideoPlayerTimeChanged);
        }

        public bool Open(string videoFile, string audioFile)
        {
            Media mediaVideo = new Media(_vlc, videoFile);
            mediaVideo.Parse();
            if (mediaVideo.IsParsed)
            {
                if (mediaVideo.ParsedStatus != MediaParsedStatus.Done)
                {
                    MessageBox.Show("Failed to parse video: " + mediaVideo.ParsedStatus.ToString(), "Phi(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            Media mediaAudio = new Media(_vlc, audioFile);
            mediaAudio.Parse();
            if (mediaAudio.IsParsed)
            {
                if (mediaAudio.ParsedStatus != MediaParsedStatus.Done)
                {
                    MessageBox.Show("Failed to parse audio: " + mediaAudio.ParsedStatus.ToString(), "Phi(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            _video.FileName = videoFile;
            _audio.FileName = audioFile;

            _video.MediaPlayer.Media = mediaVideo;
            _audio.MediaPlayer.Media = mediaAudio;

            return true;
        }

        public void Play()
        {
            _video.MediaPlayer.Play();
            _audio.MediaPlayer.Play();

            PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(false, false);

            _video.MediaPlayer.Volume = _video.Volume;
        }

        // This is synchronous, and will block until all VLC threads have been joined.
        // Calling this from a VLC callback is a bound to cause a deadlock.
        public void Stop()
        {
            _video.MediaPlayer.Stop();
            _audio.MediaPlayer.Stop();
        }

        public void Pause()
        {
            _video.MediaPlayer.SetPause(true);
            _audio.MediaPlayer.SetPause(true);
        }

        public void Unpause()
        {
            _video.MediaPlayer.SetPause(false);
            PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(false, false);

            _video.MediaPlayer.Volume = _video.Volume;
        }

        // This has no effect if no media is being played. Not all formats and protocols support this.
        public void Seek(long ms)
        {
            _video.MediaPlayer.SeekTo(TimeSpan.FromMilliseconds(ms));
            _audio.MediaPlayer.SeekTo(TimeSpan.FromMilliseconds(ms - _audioDelay));
        }

        // The balance of the Universe rests in the hands of this function
        private void PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(bool preventDeadlocks, bool preventAudioSync)
        {
            // It's not the audio's time to be played yet...
            if (_video.MediaPlayer.Time < _audioDelay)
            {
                // Just in case we've missed any possible edge cases
                // There's hundreds of them, and I am NOT about to handle all of them on an individual event basis. Fuck that shit
                _audio.MediaPlayer.SetPause(true);
                return;
            }

            // Only resume audio playback if the video player is also playing
            // Make sure our video is in the "Playing" state (and not "Paused", "Opening", etc...)
            if (_video.MediaPlayer.State == VLCState.Playing)
            {
                // What state are we coming back from?
                if (_audio.MediaPlayer.State == VLCState.Paused)
                {
                    _audio.MediaPlayer.SetPause(false);
                    _audio.MediaPlayer.Volume = _audio.Volume;
                }
                else if (_audio.MediaPlayer.State == VLCState.Stopped)
                {
                    _audio.MediaPlayer.Play();
                    _audio.MediaPlayer.Volume = _audio.Volume;
                }
                else if (_audio.MediaPlayer.State == VLCState.Ended || _audio.MediaPlayer.State == VLCState.Error)
                {
                    if (preventDeadlocks)
                    {
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            _audio.MediaPlayer.Stop();
                            _audio.MediaPlayer.Play();
                            _audio.MediaPlayer.Volume = _audio.Volume;
                        });
                    }
                    else
                    {
                        _audio.MediaPlayer.Stop();
                        _audio.MediaPlayer.Play();
                        _audio.MediaPlayer.Volume = _audio.Volume;
                    }
                }
            }

            if (!preventAudioSync)
            {
                // This is the time we want our audio to be at
                long targetAudioTime = _video.MediaPlayer.Time - _audioDelay;

                // Since constantly fixating the audio playtime on the exact value of the target time will cause audio hiccups...
                // ...we need to give it a little bit more clearance, by subtracting (or adding) a small delta we choose
                long targetAudioTimeLowerBound = targetAudioTime - _audioDelayDelta;
                long targetAudioTimeHigherBound = targetAudioTime + _audioDelayDelta;

                // If we're outside of our allowed bounds, quickly try to compensate
                if (_audio.MediaPlayer.Time <= targetAudioTimeLowerBound || _audio.MediaPlayer.Time >= targetAudioTimeHigherBound)
                {
                    // Quickly fix our audio time to match the video time, in case any unfortunate buffering happened...
                    // ...or anything else that could've potentially, MAGICALLY caused a desync
                    _audio.MediaPlayer.SeekTo(TimeSpan.FromMilliseconds(targetAudioTime));
                }
            }
        }

        private void VideoPlayerBuffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            // Video is buffering
            if (e.Cache < 100.0)
            {
                _audio.MediaPlayer.SetPause(true);
            }
            else // Video has finished buffering
            {
                PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(true, false);
            }
        }

        private void VideoPlayerEndReached(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                _video.MediaPlayer.Stop();
                _audio.MediaPlayer.Stop();
            });
        }

        private void AudioPlayerEncounteredError(object sender, EventArgs e)
        {
            MessageBox.Show("An unknown error occurred during audio playback.", "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void VideoPlayerEncounteredError(object sender, EventArgs e)
        {
            MessageBox.Show("An unknown error occurred during video playback.", "Phil(C)lipHelper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void VideoPlayerOpening(object sender, EventArgs e)
        {
            _audio.MediaPlayer.SetPause(true);
        }

        private void VideoPlayerPaused(object sender, EventArgs e)
        {
            _audio.MediaPlayer.SetPause(true);
        }

        private void VideoPlayerPlaying(object sender, EventArgs e)
        {
            PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(true, false);
        }

        private void VideoPlayerStopped(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ => _audio.MediaPlayer.Stop());
        }

        private void VideoPlayerTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            PlayOrUnpauseAudioIfAppropriateWhileUpdatingAudioTimeIfNecessary(true, true);
        }
    }
}
