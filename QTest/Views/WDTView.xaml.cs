using QTest.Tools;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace QTest.Views
{
    /// <summary>
    /// WDTView.xaml 的交互逻辑
    /// </summary>
    public partial class WDTView : UserControl
    {
        private WatchDogUtils watchdog = null;
        //0:second 1:minute
        private int timeUint = 0;
        private DispatcherTimer wdttimer;
        private ushort feedTime = 60;
        private ushort wtimer = 1;
        public WDTView()
        {
            InitializeComponent();

            wdttimer = new DispatcherTimer
            {
                IsEnabled = false
            };
            wdttimer.Tick += FeedTimerTick;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("watchdog OnLoaded!");
            wdt_info.Text = "Watchdog Timer for ITE IT87xx Environment Control - Low Pin Count Input / Output." +
                " Support of the IT8786、IT8784、IT8772、IT8728...";
            string[] types = {"Q300", "Q500"};
            minipc_type.ItemsSource = types;
        }

        private void Minipc_type_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Minipc_type_DropDownClosed........." + minipc_type.Text);
            if(feed_button.IsEnabled == false)
            {
                feed_button.IsEnabled = true;
            }
            if(watchdog == null)
            {
                watchdog = new WatchDogUtils();
                InitWatchDogDriver();
            }
            watchdog.MinipcType = minipc_type.Text;
        }

        private void InitWatchDogDriver()
        {
            bool initResult = watchdog.Initialize();
            if(!initResult)
            {
                driver_status.Text = "驱动加载失败!";
                driver_status.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                driver_status.Text = "驱动加载完成!";
                watchdog.InitSuperIO();
                chip_name.Text = "芯片型号：ITE " + watchdog.GetChipName();
                watchdog.ExitSuperIo();
            }
        }

        private void Feed_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //WDT的可编程超时范围为1~65535秒/分
            ushort time = 60;
            try
            {
                time = Convert.ToUInt16(time_val.Text);
                if(time < 10 || time > 65535)
                {
                    MessageBox.Show("请输入10~65536之间的整数！");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请输入10~65536之间的整数！");
                return;
            }
            
            if(watchdog == null)
            {
                MessageBox.Show("未选择迷你电脑型号！");
                return;
            }
            interval.Text = "喂狗间隔：" + time + "s";
            feedTime = time;
            wtimer = Convert.ToUInt16(Math.Ceiling(time * 0.8));//定时器必须提前喂狗
            Console.WriteLine("feedTime:{0} wtimer:{1}", feedTime, wtimer);

            watchdog.InitSuperIO();
            watchdog.InitLogicDevice();//LDN 07
            
            if (watchdog.MinipcType.Equals("Q500"))
            {
                //SMI# Control Register 2 (Index=F1h, Default=00h)
                //bit 6:触发方式 0边沿触发/1电平触发
                //bit 2 SMI# WDT功能开关 1开/0关
                if (timeUint == 0)
                {
                    watchdog.EnableWatchDog(0xf1, 0x44);//0100 0100
                }
            }
            else
            {
                //Watch Dog Timer Configuration Register (Index=72h, Default=001s0000b)
                //bit 7:喂狗时间单位 1秒/0分
                //bit 4:WDT功能开关 1开/0关
                if(timeUint == 0)
                {
                    watchdog.EnableWatchDog(0x72, 0x90);//1001 0000
                }
            }
            watchdog.ExitSuperIo();

            WatchDogManager tm = WatchDogManager.Instance;
            if (tm.Timer != null)
            {
                Console.WriteLine("关闭之前的定时器！");
                tm.Timer.Stop();
                tm.Timer = null;
                tm.WatchDog = null;
            }

            //设定喂狗时间，即在设定时间内必须喂狗，
            watchdog.FeedDog(feedTime);
            if (!wdttimer.IsEnabled)
            {
                wdttimer.Interval = TimeSpan.FromMilliseconds(wtimer * 1000);
                wdttimer.Start();
            }

            tm.Timer = wdttimer;
            tm.WatchDog = watchdog;
        }

        private void FeedTimerTick(object sender, EventArgs e)
        {
            Console.WriteLine("timer feed dog:" +
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") +
                        " feedTime:" + feedTime + " wtimer:" + wtimer);
            watchdog.FeedDog(feedTime);
        }

        private void Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            time_val.Text = "";
            if (wdttimer.IsEnabled)
            {
                wdttimer.Stop();
            }
            if(watchdog != null)
            {
                if (watchdog.MinipcType.Equals("Q500"))
                {
                    watchdog.StopWatchDog(0xf1, 0x40);
                }
                else
                {
                    watchdog.StopWatchDog(0x72, 0x80);
                }
            }
            WatchDogManager.Instance.Timer = null;
            WatchDogManager.Instance.WatchDog = null;
        }

        private void WDTUnloaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("watchdog exit..........");
            //已经喂狗，退出是弹出提示
            if(wdttimer.IsEnabled)
            {
                MessageBoxResult result = MessageBox.Show("是否终止看门狗？", "提示",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    wdttimer.Stop();
                    wdttimer = null;
                    WatchDogManager.Instance.Timer = null;
                    WatchDogManager.Instance.WatchDog = null;
                    if (watchdog != null)
                    {
                        if (watchdog.MinipcType.Equals("Q500"))
                        {
                            watchdog.StopWatchDog(0xf1, 0x40);
                        }
                        else
                        {
                            watchdog.StopWatchDog(0x72, 0x80);
                        }
                        watchdog.SysDispose();
                        watchdog = null;
                    }
                }
            }
        }

        private void Time_val_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void Time_val_TextChanged(object sender, TextChangedEventArgs e)
        {
            time_val.Text = time_val.Text.Replace(" ", "");
        }

        private void Second_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (second.IsChecked == true)
            {
                timeUint = 0;
            }
        }

        private void Minute_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (minute.IsChecked == true)
            {
                timeUint = 1;
            }
        }
    }
}
