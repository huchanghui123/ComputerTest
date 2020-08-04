using System;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// SerialTestView.xaml 的交互逻辑
    /// </summary>
    public partial class SerialTestView : UserControl
    {
        public SerialTestView()
        {
            InitializeComponent();
        }

        private void OpenSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OpenSerialPort_Click");
        }
        private void CloseSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("CloseSerialPort_Click");
        }
        private void OpenAllSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OpenAllSerialPort_Click");
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnLoaded......................");
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnUnloaded......................");
        }
    }
}
