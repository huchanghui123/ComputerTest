using NAudio.Gui;
using System;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// AudioTestView.xaml 的交互逻辑
    /// </summary>
    public partial class AudioTestView : UserControl
    {
        private VolumeSlider volumeSlider;
        private VolumeMeter volumeMeter1;
        private VolumeMeter volumeMeter2;
        private WaveformPainter waveformPainter1;
        private WaveformPainter waveformPainter2;
        public AudioTestView()
        {
            InitializeComponent();

            volumeSlider = new VolumeSlider();
            this.NAudioVolumeSlider.Child = volumeSlider;

            volumeMeter1 = new VolumeMeter();
            this.NAudioVolumeMeter1.Child = volumeMeter1;
            volumeMeter2 = new VolumeMeter();
            this.NAudioVolumeMeter2.Child = volumeMeter2;

            waveformPainter1 = new WaveformPainter();
            waveformPainter2 = new WaveformPainter();
            waveformPainter1.BackColor = waveformPainter2.BackColor = 
                System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            waveformPainter1.ForeColor = waveformPainter2.ForeColor = 
                System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NAudioPainter1.Child = waveformPainter1;
            this.NAudioPainter2.Child = waveformPainter2;
        }

        private void Audio_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int[] Latencys = { 25, 50, 100, 150, 200, 300, 400, 500 };
            latncyBox.ItemsSource = Latencys;
            latncyBox.SelectedIndex = 5;
            int[] sampleRates = { 8000, 16000, 22050, 32000, 44100, 48000 };
            rateBox.ItemsSource = sampleRates;
            rateBox.SelectedIndex = 0;
            string[] channels = { "Mono", "Stereo" };
            channelBox.ItemsSource = channels;
            channelBox.SelectedIndex = 1;
        }

        private void SolidRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("SolidRadioChecked!!");
        }

        private void LeftRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("LeftRadioChecked!!");
        }

        private void RightRadio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("RightRadioChecked!!");
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


        private void Audio_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("AudioUnloaded!!");
        }
    }
}
