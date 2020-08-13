using NAudio.Gui;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using QTest.instances;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QTest.Views
{
    /// <summary>
    /// AudioTestView.xaml 的交互逻辑
    /// </summary>
    public partial class AudioTestView : UserControl
    {
        private IWavePlayer waveOut;
        private IWaveIn captureDevice;
        private WaveFileWriter waveWriter;
        private AudioFileReader audioFileReader;
        private AudioFileReader audioFileReader2;
        private MultiplexingWaveProvider waveProvider;
        private Action<float> setVolumeDelegate;

        private VolumeSlider volumeSlider;
        private VolumeMeter volumeMeter1;
        private VolumeMeter volumeMeter2;
        private WaveformPainter waveformPainter1;
        private WaveformPainter waveformPainter2;

        private readonly string outputFolder;
        private string fileName = System.Windows.Forms.Application.StartupPath + "\\Audio.mp3";

        private DispatcherTimer timer;
        public AudioTestView()
        {
            InitializeComponent();

            volumeSlider = new VolumeSlider();
            volumeSlider.VolumeChanged += new EventHandler(OnVolumeSliderChanged);
            this.NAudioVolumeSlider.Child = volumeSlider;

            volumeMeter1 = new VolumeMeter();
            volumeMeter2 = new VolumeMeter();
            volumeMeter1.ForeColor = volumeMeter2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.NAudioVolumeMeter1.Child = volumeMeter1;
            this.NAudioVolumeMeter2.Child = volumeMeter2;

            waveformPainter1 = new WaveformPainter();
            waveformPainter2 = new WaveformPainter();
            waveformPainter1.BackColor = waveformPainter2.BackColor = 
                System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            waveformPainter1.ForeColor = waveformPainter2.ForeColor = 
                System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NAudioPainter1.Child = waveformPainter1;
            this.NAudioPainter2.Child = waveformPainter2;

            outputFolder = Path.Combine(System.Windows.Forms.Application.StartupPath, "Recording");
            Directory.CreateDirectory(outputFolder);
        }

        private void Audio_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadAudioDevicesCombo();
            LoadWaveInDevicesCombo();

            int[] Latencys = { 25, 50, 100, 150, 200, 300, 400, 500 };
            latncyBox.ItemsSource = Latencys;
            latncyBox.SelectedIndex = 5;
            int[] sampleRates = { 8000, 16000, 22050, 32000, 44100, 48000 };
            rateBox.ItemsSource = sampleRates;
            rateBox.SelectedIndex = 0;
            string[] channels = { "Mono", "Stereo" };
            channelBox.ItemsSource = channels;
            channelBox.SelectedIndex = 1;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500),
                IsEnabled = true
            };
            timer.Tick += Timer_Tick;
        }

        private void OnVolumeSliderChanged(object sender, EventArgs e)
        {
            setVolumeDelegate?.Invoke(volumeSlider.Volume);
        }

        private void LoadAudioDevicesCombo()
        {
            try
            {
                var devices = Enumerable.Range(0, WaveOut.DeviceCount).Select(n => WaveOut.GetCapabilities(n)).ToArray();
                audioBox.ItemsSource = devices;
                audioBox.DisplayMemberPath = "ProductName";
                audioBox.SelectedIndex = 0;
            }
            catch (Exception)
            {
                audioBox.Text = "没有找到音频设备！";
            }
        }

        private void LoadWaveInDevicesCombo()
        {
            try
            {
                var devices = Enumerable.Range(0, WaveIn.DeviceCount).Select(n => WaveIn.GetCapabilities(n)).ToArray();
                recordBox.ItemsSource = devices;
                recordBox.DisplayMemberPath = "ProductName";
                recordBox.SelectedIndex = 0;
            }
            catch (Exception)
            {
                recordBox.Text = "没有找到麦克风！";
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!MyWaveOut.IsAvailable)
            {
                MessageBox.Show("The selected output driver is not available on this system!");
                return;
            }
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else if (waveOut.PlaybackState == PlaybackState.Paused)
                {
                    waveOut.Play();
                    return;
                }
            }
            try
            {
                CreateWaveOut();
            }
            catch (Exception driverCreateException)
            {
                MessageBox.Show(String.Format("{0}", driverCreateException.Message));
                return;
            }
            ISampleProvider sampleProvider;
            try
            {
                sampleProvider = CreateInputStream(fileName);
                Console.WriteLine("ISampleProvider:" + sampleProvider);
            }
            catch (Exception createException)
            {
                MessageBox.Show(String.Format("{0}", createException.Message), "Error Loading File");
                return;
            }
            total_time.Text = String.Format("{0:00}:{1:00}", (int)audioFileReader.TotalTime.TotalMinutes,
                audioFileReader.TotalTime.Seconds);
            try
            {
                waveOut.Init(sampleProvider);
            }
            catch (Exception initException)
            {
                MessageBox.Show(String.Format("{0}", initException.Message), "Error Initializing Output");
                return;
            }
            waveOut.Play();
        }

        private ISampleProvider CreateInputStream(string fileName)
        {
            audioFileReader = new AudioFileReader(fileName);
            audioFileReader2 = new AudioFileReader(fileName);//创建一个静音流
            audioFileReader2.Volume = 0.0f;
            //处理多声道音频,双声道需要声卡驱动支持
            waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] {
                audioFileReader, audioFileReader2}, 2);
            if (left_radio.IsChecked == true)
            {
                waveProvider.ConnectInputToOutput(2, 1);
                waveProvider.ConnectInputToOutput(3, 1);
            }
            else if (right_radio.IsChecked == true)
            {
                waveProvider.ConnectInputToOutput(2, 0);
                waveProvider.ConnectInputToOutput(3, 0);
            }
            var sampleChannel = new SampleChannel(waveProvider, false);
            setVolumeDelegate = vol => sampleChannel.Volume = vol;//设置音量
            var postVolumeMeter = new MeteringSampleProvider(sampleChannel);

            postVolumeMeter.StreamVolume += OnPostVolumeMeter;
            sampleChannel.PreVolumeMeter += OnPreVolumeMeter;//波形图
            return postVolumeMeter;
        }

        private void CreateWaveOut()
        {
            CloseWaveOut();
            var deviceNumber = (int)audioBox.Items.IndexOf(audioBox.SelectedItem);
            var latency = (int)latncyBox.SelectedItem;
            Console.WriteLine("CreateWaveOut deviceNumber:{0} latency:{1}", deviceNumber, latency);
            waveOut = new MyWaveOut().CreateDevice(deviceNumber, latency);
            waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message, "Playback Device Error");
            }
            if (audioFileReader != null)
            {
                audioFileReader.Position = 0;
            }
        }

        private void CloseWaveOut()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                setVolumeDelegate = null;
                audioFileReader = null;
            }
            if (waveOut != null)
            {
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    waveOut.Pause();
                }
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) => waveOut?.Stop();
        
        private void SolidRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if(waveOut != null && audioFileReader != null)
            {
                if (solid_radio.IsChecked == true)
                {
                    waveProvider.ConnectInputToOutput(0, 0);
                    waveProvider.ConnectInputToOutput(1, 1);
                }
            }
        }

        private void LeftRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (waveOut != null && audioFileReader != null)
            {
                if (left_radio.IsChecked == true)
                {
                    waveProvider.ConnectInputToOutput(0, 0);
                    //waveProvider.ConnectInputToOutput(1, 0);
                    waveProvider.ConnectInputToOutput(2, 1);
                    waveProvider.ConnectInputToOutput(3, 1);
                }
            }
        }

        private void RightRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (waveOut != null && audioFileReader != null)
            {
                if (right_radio.IsChecked == true)
                {
                    //waveProvider.ConnectInputToOutput(0, 1);
                    waveProvider.ConnectInputToOutput(1, 1);
                    waveProvider.ConnectInputToOutput(2, 0);
                    waveProvider.ConnectInputToOutput(3, 0);
                }
            }
        }

        private void StartRecordBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("StartRecordBtn_Click!!");
        }

        private void StopRecordBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("StopRecordBtn_Click!!");
        }

        private void RecordPlay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("RecordPlayClick!!");
        }

        private void RecordPause_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("RecordPauseClick!!");
        }

        private void RecordOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("RecordOpenClick!!");
        }

        private void RecordDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("RecordDeleteClick!!");
        }

        private void OnPostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            volumeMeter1.Amplitude = e.MaxSampleValues[0];
            volumeMeter2.Amplitude = e.MaxSampleValues[1];
        }

        private void OnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            Console.WriteLine(e.MaxSampleValues[0] + "---" + e.MaxSampleValues[1]);
            waveformPainter1.AddMax(e.MaxSampleValues[0]);
            waveformPainter2.AddMax(e.MaxSampleValues[1]);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(waveOut != null && audioFileReader != null)
            {
                TimeSpan currentTime = (waveOut.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : audioFileReader.CurrentTime;
                slider.Value = Math.Min(slider.Maximum, (int)(100 * currentTime.TotalSeconds / audioFileReader.TotalTime.TotalSeconds));
                current_time.Text = String.Format("{0:00}:{1:00}", (int)currentTime.TotalMinutes, currentTime.Seconds);
            }
            else
            {
                slider.Value = 0;
            }
           
        }

        private void OpenFileBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string allExtensions = "*.wav;*.aiff;*.mp3;*.aac";
            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                string[] arrs = fileName.Split('\\');
                file_name.Text = arrs[arrs.Length-1];
            }
        }

        private void Audio_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("NAudio Test Unloaded!!!");
            waveOut?.Dispose();
            captureDevice?.Dispose();
            if (timer.IsEnabled)
                timer.Stop();
        }
    }
}
