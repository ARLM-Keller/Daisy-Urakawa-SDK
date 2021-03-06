using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

#if USE_SHARPDX
using SharpDX.DirectSound;
#else
using Microsoft.DirectX.DirectSound;
#endif

#if USE_SOUNDTOUCH
#if USE_SHARPDX
using SharpDX.Multimedia;
#endif
using SoundTouch;
#if true || SOUNDTOUCH_INTEGER_SAMPLES
using TSampleType = System.Int16; // short (System.Int32 == int)
using TLongSampleType = System.Int64; // long
#else
using TSampleType = System.Single;
using TLongSampleType = System.Double;
#endif

#endif //USE_SOUNDTOUCH

namespace AudioLib
{
    // This class underwent a major cleanup and simplification at revision 1488.
    // See:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/changeset/1488#file0
    // Just in case we need to restore some functionality:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/browser/trunk/csharp/audio/AudioLib/AudioPlayer.cs?rev=1487
    //#if NET40
    //    [SecuritySafeCritical]
    //#endif
    public partial class AudioPlayer
    {

        public AudioPlayer(bool keepStreamAlive, bool useSoundTouchIfAvailable)
            : this(keepStreamAlive)
        {
#if USE_SOUNDTOUCH
            UseSoundTouch = useSoundTouchIfAvailable;
#endif // USE_SOUNDTOUCH
        }
        private readonly bool m_KeepStreamAlive;
        public AudioPlayer(bool keepStreamAlive)
        {
#if DEBUG
            DebugFix.Assert(BitConverter.IsLittleEndian);
#endif // DEBUG

            m_KeepStreamAlive = keepStreamAlive;

            CurrentState = State.NotReady;

            initPreviewTimer();
        }

        /// <summary>
        /// The four states of the audio player.
        /// NotReady: the player has no output device set yet.
        /// Playing: sound is currently playing.
        /// Paused: playback was paused and can be resumed.
        /// Stopped: player is idle.
        /// </summary>
        public enum State
        {
            NotReady, Stopped,
            Playing, Paused
        };

        private State m_State;
        public State CurrentState
        {
            get
            {
                //if (mIsFwdRwd) return State.Playing; else
                return m_State;
            }
            private set
            {
                if (m_State == value)
                {
                    return;
                }

                State oldState = m_State;
                m_State = value;

                StateChangedHandler del = StateChanged;
                if (del != null && !mPreviewTimer.Enabled)
                    del(this, new StateChangedEventArgs(oldState));
                //var del = StateChanged;
                //if (del != null)
                //del(this, new StateChangedEventArgs(oldState));
            }
        }

#if USE_SOUNDTOUCH

        private bool m_UseSoundTouchBackup = false;

        private bool m_UseSoundTouch = false;
        public bool UseSoundTouch
        {
            get { return m_UseSoundTouch; }
            set
            {
                m_UseSoundTouch = value;
                m_UseSoundTouchBackup = m_UseSoundTouch;
            }
        }
#else
        public bool UseSoundTouch
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
#endif //USE_SOUNDTOUCH


        private bool NotNormalPlayFactor()
        {
            return ((int)Math.Round(m_FastPlayFactor * 100.0)) != 100;
        }


        private float m_FastPlayFactor = 1;
        /// <summary>
        /// Gets and sets fast play multiplying factor,suggested value is between 1.0 and 2.0
        /// </summary>
        public float FastPlayFactor
        {
            get
            {
                return m_FastPlayFactor;
            }
            set
            {
                Boolean wasPlaying = CurrentState == State.Playing;

                if (wasPlaying)
                {
                    Pause();
                }

                m_FastPlayFactor = value;

                if (wasPlaying)
                {
                    Resume();
                    return;
                }

#if USE_SOUNDTOUCH
                if (m_SoundTouch != null)
                {
                    m_SoundTouch.SetTempo(m_FastPlayFactor);
                    //m_SoundTouch.SetTempoChange(m_FastPlayFactor * 100);

                    m_SoundTouch.SetPitchSemiTones(0);
                    m_SoundTouch.SetRateChange(0);
                }

                if (!UseSoundTouch)
#endif //USE_SOUNDTOUCH
                {
                    if (m_CircularBuffer != null &&
#if USE_SHARPDX
 true //m_CircularBuffer.Capabilities.??
#else
 m_CircularBuffer.Caps.ControlFrequency
#endif
)
                    {
#if USE_SHARPDX
                        //m_CircularBuffer.Format.SampleRate
                        int sampleRatePerSecond = -1;

                        if (m_CurrentAudioPCMFormat != null)
                        {
                            sampleRatePerSecond = (int)m_CurrentAudioPCMFormat.SampleRate;
                        }
                        else
                        {
                            int size = 0;
                            try
                            {
                                m_CircularBuffer.GetFormat(null, 0, out size);
                            }
                            catch (Exception ex)
                            {
                                // Ignore
                                bool debug_ = true;
                            }

                            int size_ = size;
                            WaveFormat[] formats = new WaveFormat[1];
                            formats[0] = new WaveFormat(22500, 16, 2);
                            try
                            {
                                m_CircularBuffer.GetFormat(formats, size_, out size);

                                sampleRatePerSecond = formats[0].SampleRate;
                            }
                            catch (Exception ex)
                            {
                                // Ignore
                                bool debug_ = true;
                            }
                        }
#endif
                        
                        #if USE_SHARPDX
                             int sampleRate  = sampleRatePerSecond ;
                                               #else
                           int sampleRate  =   m_CircularBuffer.Format.SamplesPerSecond ;
                        
#endif
                        
                        DebugFix.Assert(sampleRate > 0);
                        if (sampleRate > 0)
                        {
                        int fastPlaySamplesPerSecond = (int)Math.Round(sampleRate  * m_FastPlayFactor);

#if DEBUG
                            if (m_CurrentAudioPCMFormat != null)
                            {
                                DebugFix.Assert(m_CurrentAudioPCMFormat.SampleRate ==
#if USE_SHARPDX
 sampleRatePerSecond
#else
 m_CircularBuffer.Format.SamplesPerSecond
#endif
);
                            }
#endif
                            //DEBUG
                            // accelerated samples per second
                            m_CircularBuffer.Frequency = fastPlaySamplesPerSecond;
                        }
                    }
                }
            }
        }


        public delegate Stream StreamProviderDelegate();
        private StreamProviderDelegate m_CurrentAudioStreamProvider;
        private Stream m_CurrentAudioStream;
        private long m_CurrentAudioDataLength;

        public bool EnsurePlaybackStreamIsDead()
        {
            if (CurrentState == State.NotReady)
            {
                return false;
            }

            bool I_Closed_The_Stream = m_CurrentAudioStream != null;

            if (I_Closed_The_Stream)
            {
                m_CurrentAudioStream.Close();
                m_CurrentAudioStream = null;
            }

            m_CurrentAudioPCMFormat = null;
            m_CurrentAudioDataLength = 0;
            m_CurrentAudioStreamProvider = null;

            return I_Closed_The_Stream;
        }

#if USE_SOUNDTOUCH
        SoundTouch<TSampleType, TLongSampleType> m_SoundTouch;
#endif //USE_SOUNDTOUCH

        private readonly Object LOCK_DEVICES = new object();
        public void ClearDeviceCache()
        {
            lock (LOCK_DEVICES)
            {
                m_CachedOutputDevices = null;
            }
        }

        public void SetOutputDevice(Control handle, string name)
        {
            lock (LOCK_DEVICES)
            {
                if (m_CachedOutputDevices != null
                    && OutputDevice != null && OutputDevice.Name == name && !OutputDevice.Device.
#if USE_SHARPDX
IsDisposed
#else
Disposed
#endif
)
                {
                    return;
                }

                if (m_CachedOutputDevices != null)
                {
                    OutputDevice foundCached =
                        m_CachedOutputDevices.Find(delegate(OutputDevice d) { return d.Name == name; });
                    if (foundCached != null && !foundCached.Device.
#if USE_SHARPDX
IsDisposed
#else
Disposed
#endif
)
                    {
                        SetOutputDevice(handle, foundCached);
                        return;
                    }
                }
            }

            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = null;
            lock (LOCK_DEVICES)
            {
                found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            }
            if (found != null)
            {
                SetOutputDevice(handle, found);
            }
            else if (devices.Count > 0)
            {
                SetOutputDevice(handle, devices[0]);

                Console.WriteLine("OutputDevice name not found, defaulting: [[" + name + "]] ==> [[" + OutputDevice.Name + "]]");
            }
            else
            {
                CurrentState = State.NotReady;

                //throw new Exception("No output device available.");
                Console.WriteLine("ERROR: OutputDevices empty!!");
            }
        }

        private List<OutputDevice> m_CachedOutputDevices;
        public List<OutputDevice> OutputDevices
        {
            get
            {
                Console.WriteLine("=== OutputDevices");

                lock (LOCK_DEVICES)
                {
#if USE_SHARPDX
                    List<DeviceInformation> devices = DirectSound.GetDevices();
#else
                    DevicesCollection devices = new DevicesCollection();
#endif
                    List<OutputDevice> outputDevices = new List<OutputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        Console.WriteLine("OutputDevice ModuleName:");
                        Console.WriteLine(info.ModuleName);

                        Console.WriteLine("OutputDevice Description:");
                        Console.WriteLine(info.Description);

                        Console.WriteLine("OutputDevice DriverGuid:");
                        Console.WriteLine(info.DriverGuid);
                        try
                        {
                            outputDevices.Add(new OutputDevice(info));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("OutputDevice FAILED:");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            continue;
                        }
                    }
                    m_CachedOutputDevices = outputDevices;

                    Console.WriteLine(">>> OutputDevices: " + outputDevices.Count);
                    return outputDevices;
                }
            }
        }

        private Control m_OutputDeviceControl = null;
        public Control OutputDeviceControl
        {
            get { return m_OutputDeviceControl; }
        }

        private void SetOutputDevice(Control handle, OutputDevice device)
        {
            m_OutputDeviceControl = handle;

            if (handle != null)
            {
#if USE_SHARPDX
                device.Device.SetCooperativeLevel(handle.Handle, CooperativeLevel.Priority);
#else
                device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
#endif
            }

            OutputDevice = device;
        }

        private OutputDevice m_OutputDevice;
        public OutputDevice OutputDevice
        {
            get
            {
                return m_OutputDevice;
            }
            private set
            {
                m_OutputDevice = value;

                CurrentState = State.Stopped;
            }
        }


        private AudioLibPCMFormat m_CurrentAudioPCMFormat;
        public AudioLibPCMFormat CurrentAudioPCMFormat
        {
            get { return m_CurrentAudioPCMFormat; }
        }

        public void PlayBytes(StreamProviderDelegate currentAudioStreamProvider,
                            long dataLength, AudioLibPCMFormat pcmInfo,
                            long bytesFrom, long bytesTo)
        {
            if (pcmInfo == null)
            {
                throw new ArgumentNullException("PCM format cannot be null !");
            }

            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (dataLength <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration cannot be <= 0 !");
            }

            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                Debug.Fail("Attempting to play when not stopped ? " + CurrentState);
                return;
            }


#if USE_SOUNDTOUCH
            if (false && pcmInfo.NumberOfChannels > 1)
            {
                m_UseSoundTouch = false; //TODO: stereo all scrambled with SoundTouch !!
            }
            else
            {
                m_UseSoundTouch = m_UseSoundTouchBackup;
            }
#endif // USE_SOUNDTOUCH

            m_CurrentAudioStreamProvider = currentAudioStreamProvider;
            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioPCMFormat = pcmInfo;
            m_CurrentAudioDataLength = dataLength;



            long startPosition = 0;
            if (bytesFrom > 0)
            {
                startPosition = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(bytesFrom);
            }

            long endPosition = 0;
            if (bytesTo > 0)
            {
                endPosition = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(bytesTo);
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithMillisecondsTolerance(startPosition, 0))
            {
                startPosition = 0;
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithMillisecondsTolerance(endPosition, dataLength))
            {
                endPosition = dataLength;
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithMillisecondsTolerance(endPosition, 0))
            {
                endPosition = 0;
            }

            if (endPosition != 0
                && m_CurrentAudioPCMFormat.BytesAreEqualWithMillisecondsTolerance(endPosition, startPosition))
            {
                return;
            }

            if (startPosition >= 0 &&
                (endPosition == 0 || startPosition < endPosition) &&
                endPosition <= dataLength)
            {
                if (m_FwdRwdRate == 0)
                {
                    startPlayback(startPosition, endPosition);
                    Console.WriteLine("starting playback ");
                }
                else if (m_FwdRwdRate > 0)
                {
                    FastForward(startPosition);
                    Console.WriteLine("fast forward ");
                }
                else if (m_FwdRwdRate < 0)
                {
                    if (startPosition == 0) startPosition = m_CurrentAudioStream.Length;
                    Rewind(startPosition);
                    Console.WriteLine("Rewind ");
                }
            }
            else
            {
                //throw new Exception("Start/end positions out of bounds of audio asset.");
                DebugFix.Assert(false);
            }
        }

        //public void PlayTime(StreamProviderDelegate currentAudioStreamProvider,
        //                    double duration, AudioLibPCMFormat pcmInfo,
        //                    double from, double to)
        //{
        //    if (pcmInfo == null)
        //    {
        //        throw new ArgumentNullException("PCM format cannot be null !");
        //    }

        //    Play(currentAudioStreamProvider, duration, pcmInfo, pcmInfo.ConvertTimeToBytes(from), pcmInfo.ConvertTimeToBytes(to));
        //}


        private long m_ResumeStartPosition;

        public void Pause(long bytePos)
        {

            if (CurrentState == State.NotReady)
            {
                return;
            }

            //if (CurrentState != State.Playing)
            //{
            //return;
            //}

            m_ResumeStartPosition = bytePos;

            bool wasPlaying = CurrentState == State.Playing || m_FwdRwdRate != 0;

            if (m_FwdRwdRate != 0)
            {
                StopForwardRewind();
                m_PlaybackEndPositionInCurrentAudioStream = m_CurrentAudioDataLength;
            }
            CurrentState = State.Paused;

            if (wasPlaying) stopPlayback();
        }

        public void Pause()
        {

            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Playing && m_FwdRwdRate == 0)
            {
                return;
            }

            if (m_FwdRwdRate != 0)
            {
                StopForwardRewind();
                m_PlaybackEndPositionInCurrentAudioStream = m_CurrentAudioDataLength;
            }

            m_ResumeStartPosition = getCurrentBytePosition();

            CurrentState = State.Paused;

            stopPlayback();
        }

        public void Resume()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Paused)
            {
                return;
            }

            startPlayback(m_ResumeStartPosition, m_PlaybackEndPositionInCurrentAudioStream);
        }

        public void Stop()
        {
            StopForwardRewind();
            if (CurrentState == State.NotReady)
            {
                return;
            }

            //PcmDataBufferAvailableHandler deleg = PcmDataBufferAvailable;
            //if (deleg != null)
            //{
            //    for (int i = 0; i < m_PcmDataBuffer.Length; i++)
            //    {
            //        m_PcmDataBuffer[i] = 0;
            //    }
            //    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
            //    m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = m_PcmDataBufferLength;
            //    deleg(this, m_PcmDataBufferAvailableEventArgs);
            //}

            if (CurrentState == State.Stopped)
            {
                return;
            }
            CurrentState = State.Stopped;
            stopPlayback();
        }

        public long CurrentBytePosition
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return -1;
                }

                if (m_CurrentAudioPCMFormat == null)
                {
                    return 0; // Play was never called, or the stream was closed completely
                }

                return m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(getCurrentBytePosition());
            }
        }

        private long m_CurrentBytePosition = -1;
        private long getCurrentBytePosition()
        {
            if (m_CurrentAudioPCMFormat == null)
            {
                return 0;
            }

            if (CurrentState == State.Paused)
            {
                return m_ResumeStartPosition;
            }

            if (CurrentState == State.Stopped)
            {
                return 0;
            }

            if (CurrentState == State.Playing)
            {
                if (m_CurrentBytePosition < 0)
                {
                    //Console.WriteLine(String.Format("????? m_CurrentBytePosition < 0 [{0}]", m_CurrentBytePosition));
                }

                return m_CurrentBytePosition; //refreshed in the loop thread, so depends on the refresh rate.
            }

            return 0;
        }

        private long m_PlaybackStartPositionInCurrentAudioStream;
        private long m_PlaybackEndPositionInCurrentAudioStream;

        private Stopwatch m_PlaybackStopWatch;

        private void startPlayback(long startPosition, long endPosition)
        {
            initializeBuffers();

            m_PlaybackStartPositionInCurrentAudioStream = startPosition;
            m_PlaybackEndPositionInCurrentAudioStream = endPosition == 0
                ? m_CurrentAudioDataLength
                : endPosition;

            m_CircularBufferWritePosition = 0;

            //m_CircularBufferFlushTolerance = -1;
            //m_PredictedByteIncrement = -1;

            //m_PreviousCircularBufferPlayPosition = -1;
            //m_CircularBufferTotalBytesPlayed = -1;

            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioStream.Position = m_PlaybackStartPositionInCurrentAudioStream;

            int circularBufferLength = m_CircularBuffer.
#if USE_SHARPDX
Capabilities
#else
 Caps
#endif
.BufferBytes
;

            int bytesWrittenToCirularBuffer =
                transferBytesFromWavStreamToCircularBuffer(circularBufferLength);

            m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream;

            CurrentState = State.Playing;
            //if (AllowBackToBackPlayback && m_MonitoringTimer != null) m_MonitoringTimer.Start();
            try
            {
                m_CircularBuffer.Play(0,
#if USE_SHARPDX
 PlayFlags.Looping
#else
 BufferPlayFlags.Looping // this makes it a circular buffer (which we manage manually, by tracking playback versus writing positions)
#endif
);
                if (m_PlaybackStopWatch == null)
                {
                    m_PlaybackStopWatch = new Stopwatch();
                }

#if NET4
                m_PlaybackStopWatch.Restart();
#else
                m_PlaybackStopWatch.Stop();
                m_PlaybackStopWatch.Reset();
                m_PlaybackStopWatch.Start();
#endif //NET4
            }
            catch (Exception)
            {
                Debug.Fail("EmergencyStopForSoundBufferProblem !");

                CurrentState = State.Stopped;

                StopForwardRewind();
                stopPlayback();

                return;
            }

            ThreadStart threadDelegate = delegate()
                                             {

                                                 bool endOfAudioStream = false;
                                                 try
                                                 {
                                                     endOfAudioStream = circularBufferRefreshThreadMethod();
                                                 }
                                                 catch (ThreadAbortException ex)
                                                 {
                                                     //
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     Console.WriteLine(ex.Message);
                                                     Console.WriteLine(ex.StackTrace);
                                                 }
                                                 finally
                                                 {
                                                     if (m_PlaybackStopWatch != null)
                                                     {
                                                         m_PlaybackStopWatch.Stop();
                                                     }

                                                     if (mPreviewTimer.Enabled)
                                                     {
                                                         if (endOfAudioStream || CurrentState == State.Playing)
                                                         {
                                                             m_ResumeStartPosition = CurrentBytePosition;

                                                             CurrentState = State.Paused; // before stopPlayback(), doesn't kill the stream provider
                                                         }
                                                         lock (LOCK)
                                                         {
                                                             m_CircularBufferRefreshThread = null;
                                                         }

                                                         stopPlayback();
                                                     }
                                                     else
                                                     {
                                                         if (endOfAudioStream || CurrentState == State.Playing)
                                                         {
                                                             CurrentState = State.Stopped;
                                                         }
                                                         //if (CurrentState != State.Paused) CurrentState = State.Stopped;

                                                         lock (LOCK)
                                                         {
                                                             m_CircularBufferRefreshThread = null;
                                                         }

                                                         StopForwardRewind();
                                                         stopPlayback();

                                                         if (endOfAudioStream)
                                                         {
                                                             AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                                                             if (delFinished != null && !mPreviewTimer.Enabled)
                                                                 delFinished(this, new AudioPlaybackFinishEventArgs());
                                                         }
                                                     }
                                                 }

                                                 //Console.WriteLine("Player refresh thread exiting....");

                                                 //CurrentState = State.Stopped;

                                                 //lock (LOCK)
                                                 //{
                                                 //    //m_CircularBufferRefreshThreadIsAlive = false;
                                                 //    m_CircularBufferRefreshThread = null;
                                                 //}

                                                 //Console.WriteLine("Player refresh thread exit.");
                                             };


            int count = 0;
            while (m_CircularBufferRefreshThread != null)
            {
                Console.WriteLine(@"------------ m_CircularBufferRefreshThread NOT null!!: " + count++);
                Thread.Sleep(20);

                if (count > 10)
                {
                    Console.WriteLine(@"------------ m_CircularBufferRefreshThread NOT null!! ()BREAK(): " + count++);
                    break;
                }
            }

            if (m_CircularBufferRefreshThread != null)
            {
                stopPlayback();
            }


            DebugFix.Assert(m_CircularBufferRefreshThread == null);

            lock (LOCK)
            {
                m_CircularBufferRefreshThread = new Thread(threadDelegate);
                m_CircularBufferRefreshThread.Name = "Player Refresh Thread";
                m_CircularBufferRefreshThread.Priority = ThreadPriority.Normal;
                m_CircularBufferRefreshThread.IsBackground = true;
                m_CircularBufferRefreshThread.Start();
            }


            //Console.WriteLine("Player refresh thread start.");
        }

        private void stopPlayback()
        {

            //PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
            //if (del != null)
            //{
            //    for (int i = 0 ; i < m_PcmDataBuffer.Length; i++)
            //    {
            //        m_PcmDataBuffer[i] = 0;
            //    }
            //    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
            //    del(this, m_PcmDataBufferAvailableEventArgs);
            //}


            m_CircularBuffer.Stop();

            //lock (LOCK)
            //{
            //    if (m_CircularBufferRefreshThread != null)
            //    {
            //        m_CircularBufferRefreshThread.Abort();
            //    }
            //}
            int count = 0;
            while (m_CircularBufferRefreshThread != null
                //&& (m_CircularBufferRefreshThread.IsAlive
                //// NO NEED FOR AN EXTRA CHECK, AS THE THREAD POINTER IS RESET TO NULL
                ////|| m_CircularBufferRefreshThreadIsAlive
                //)
                )
            {
                //if (count % 5 == 0)
                //{
                //    Console.WriteLine(@"///// PLAYER m_CircularBufferRefreshThread.Abort(): " + count++);
                //    lock (LOCK)
                //    {
                //        if (m_CircularBufferRefreshThread != null)
                //        {
                //            m_CircularBufferRefreshThread.Abort();
                //        }
                //    }
                //}
                Console.WriteLine(@"///// PLAYER m_CircularBufferRefreshThread != null: " + count++);
                Thread.Sleep(REFRESH_INTERVAL_MS / 2);

                if (count > 15)
                {
                    //CurrentState = State.Stopped;

                    lock (LOCK)
                    {
                        if (m_CircularBufferRefreshThread != null)
                        {
                            m_CircularBufferRefreshThread.Join(100);
                        }
                        m_CircularBufferRefreshThread = null;
                    }
                    break;
                }
            }

            if (CurrentState == State.Paused)
            {
                return;
            }

            if (!m_KeepStreamAlive)
            {
                EnsurePlaybackStreamIsDead();
            }
        }



        public event AudioPlaybackFinishHandler AudioPlaybackFinished;
        public delegate void AudioPlaybackFinishHandler(object sender, AudioPlaybackFinishEventArgs e);
        public class AudioPlaybackFinishEventArgs : EventArgs
        {
        }

        public event StateChangedHandler StateChanged;
        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);
        public class StateChangedEventArgs : EventArgs
        {
            private State m_OldState;
            public State OldState
            {
                get
                {
                    return m_OldState;
                }
            }

            public StateChangedEventArgs(State oldState)
            {
                m_OldState = oldState;
            }
        }


        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] { 0, 0, 0, 0 }, 4);
        public event PcmDataBufferAvailableHandler PcmDataBufferAvailable;
        public delegate void PcmDataBufferAvailableHandler(object sender, PcmDataBufferAvailableEventArgs e);
        public class PcmDataBufferAvailableEventArgs : EventArgs
        {
            private byte[] m_PcmDataBuffer;
            public byte[] PcmDataBuffer
            {
                get
                {
                    return m_PcmDataBuffer;
                }
                set
                {
                    m_PcmDataBuffer = value;
                }
            }

            private int m_PcmDataBufferLength;
            public int PcmDataBufferLength
            {
                get
                {
                    return m_PcmDataBufferLength;
                }
                set
                {
                    m_PcmDataBufferLength = value;

                    if (m_PcmDataBuffer != null)
                    {
                        DebugFix.Assert(m_PcmDataBufferLength <= m_PcmDataBuffer.Length);
                    }
                }
            }

            public PcmDataBufferAvailableEventArgs(byte[] pcmDataBuffer, int length)
            {
                m_PcmDataBuffer = pcmDataBuffer;
                m_PcmDataBufferLength = length;
            }
        }
    }
}
