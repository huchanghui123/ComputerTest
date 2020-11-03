using QTest.Models;
using QTest.Tools;
using System;
using System.Windows;

namespace QTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Summary(sender, e);
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Console.WriteLine("MainWindow Closed!");
            //关闭程序前，释放掉资源
            WatchDogManager tm = WatchDogManager.Instance;
            if (tm.Timer != null)
            {
                MessageBoxResult result = MessageBox.Show("看门狗正在运行是否终止看门狗？", "提示",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    tm.Timer.Stop();
                    if(tm.WatchDog.MinipcType.Equals("Q500"))
                    {
                        tm.WatchDog.StopWatchDog(0xf1, 0x40);
                    }
                    else
                    {
                        tm.WatchDog.StopWatchDog(0x72, 0x80);
                    }
                }
                tm.WatchDog.SysDispose();
            }
            SensorManager sm = SensorManager.Instance;
            if (sm.CpuCelsius != null)
            {
                Console.WriteLine("CpuCelsius Dispose!!!");
                sm.CpuCelsius.Dispose();
            }
        }

        private void Summary(object sender, RoutedEventArgs e)
        {
            DataContext = new Summary();
        }

        private void UpTime(object sender, RoutedEventArgs e)
        {
            DataContext = new UpTime();
        }

        private void Sensor(object sender, RoutedEventArgs e)
        {
            DataContext = new Sensor();
        }

        private void GPIOTools(object sender, RoutedEventArgs e)
        {
            DataContext = new GPIOTools();
        }

        private void WdtTools(object sender, RoutedEventArgs e)
        {
            DataContext = new WDTTools();
        }

        private void Motherboard(object sender, RoutedEventArgs e)
        {
            DataContext = new Motherborad();
        }

        private void AudioTest(object sender, RoutedEventArgs e)
        {
            DataContext = new AudioTest();
        }

        private void SerialTest(object sender, RoutedEventArgs e)
        {
            DataContext = new SerialTest();
        }

    }
}
