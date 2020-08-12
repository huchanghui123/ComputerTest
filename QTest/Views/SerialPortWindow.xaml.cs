using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QTest.Views
{
    /// <summary>
    /// SerialPortWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortWindow : Window
    {
        private String[] sps = null;
        private int serialCount = 1;
        private int defaultWidth = 240;
        private int defaultHeight = 300;

        private int panelX = 10;
        private int panelY = 10;
        private int statePanelX = 10;
        private int statePanelY = 310;

        private StringBuilder sb = new StringBuilder();
        private List<StackPanel> recevie_panel_list = new List<StackPanel>();
        private List<StackPanel> state_panel_list = new List<StackPanel>();
        private List<SerialPort> serial_list = new List<SerialPort>();

        private int baudRate = 115200;
        private int dataBits = 8;
        private String stopBits = "One";
        private String parity = "None";
        private String handshake = "None";

        //测试间隔100毫秒
        double timeDelay = 100;
        //定时更新检测状态
        TextBlock tb;
        TextBlock cts;
        TextBlock dsr;
        TextBlock dcd;

        private DispatcherTimer timer;

        public SerialPortWindow(string rate, string dbits, 
            String sbits, String sparity, String shandshake)
        {
            InitializeComponent();

            baudRate = int.Parse(rate.ToUpperInvariant());
            dataBits = int.Parse(dbits.ToUpperInvariant());
            stopBits = sbits;
            parity = sparity;
            handshake = shandshake;

            Console.WriteLine("BaudRate:{0} DataBits:{1} StopBits:{2} Parity:{3} Handshake:{4}"
                , baudRate, dataBits, stopBits, parity, handshake);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(timeDelay),
                IsEnabled = false
            };
            timer.Tick += Timer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("serial port window Loaded!!!");
            sps = SerialPort.GetPortNames();
            serialCount = sps.Length;
            int count = 0;
            int formWidth = 0;
            int formHeight = 380;
            count = (int)Math.Ceiling((decimal)serialCount / 2);
            if (serialCount > 1)
                formHeight += 360;
            else
                formHeight = 400;
            for (int i = 0; i < count; i++)
                formWidth += defaultWidth;

            this.Width = formWidth + 20;
            this.Height = formHeight;

            //串口参数信息
            CreatePortInfo();
            //创建接收区
            CreateReceivePanelList();
            //创建状态区
            CreateStatePanelList();
            //创建串口
            CreateSerialPort();
            OpenSerialPort();
        }

        private void CreatePortInfo()
        {
            String infoText = "BaudRate:" + baudRate + " DataBits:" + dataBits + " StopBits:" + stopBits
                + " Parity:" + parity + " Handshake:" + handshake;
            
            TextBlock tx = CreateTextBlock("宋体", 14F, infoText, "INFO");
            tx.VerticalAlignment = VerticalAlignment.Top;
            tx.Margin = new Thickness(5,5,0,0);

            SpGrid.Children.Add(tx);
        }

        private void CreateReceivePanelList()
        {
            recevie_panel_list.Clear();
            bool isodd = true;
            for (int i = 1; i <= serialCount; i++)
            {
                isodd = Convert.ToBoolean(i % 2);
                //上排
                if (isodd)
                {
                    StackPanel panel = CreateReceivePanel(i, 1);
                    recevie_panel_list.Add(panel);
                }
                else
                {
                    //下排
                    StackPanel panel = CreateReceivePanel(i, 0);
                    recevie_panel_list.Add(panel);
                }
            }
            foreach (StackPanel panel in recevie_panel_list)
            {
                SpGrid.Children.Add(panel);
            }
        }

        private StackPanel CreateReceivePanel(int i, int type)
        {
            panelY = 25;
            StackPanel panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.VerticalAlignment = VerticalAlignment.Top;
            //panel.Orientation = Orientation.Vertical;
            //用端口名当接收面板的Name
            panel.Name = sps[i - 1];
            
            int index = 0;
            if(type==1)
            {
                //上排，向下取整，1、3、5、7、9——>0、1、2、3、4
                index = (int)Math.Floor((decimal)i / 2);
            }
            else
            {
                //下排，2、4、6、8、10——>0、1、2、3、4
                panelY += 335;
                index = (i - 2) / 2;
            }
            TextBlock tx = CreateTextBlock("宋体", 14F, panel.Name, "PORTNAME");
            TextBox textBox = CreateTextBox();

            panelX = index * defaultWidth == 0 ? 10 : index * defaultWidth + 10;
            Console.WriteLine("panelX:{0} panelY:{1}", panelX, panelY);
            panel.Margin = new Thickness(panelX, panelY, 0 , 0);
            panel.Width = defaultWidth - 10;
            panel.Height = defaultHeight;
            panel.Background = Brushes.LightGray;

            panel.Children.Add(tx);
            panel.Children.Add(textBox);

            return panel;
        }

        private void CreateStatePanelList()
        {
            state_panel_list.Clear();
            bool isodd = true;
            for (int i = 1; i <= serialCount; i++)
            {
                isodd = Convert.ToBoolean(i % 2);
                if (isodd)
                {
                    StackPanel panel = CreateStatePanel(i, 1);
                    state_panel_list.Add(panel);
                }
                else
                {
                    StackPanel panel = CreateStatePanel(i, 0);
                    state_panel_list.Add(panel);
                }
            }
            foreach (StackPanel panel in state_panel_list)
            {
                SpGrid.Children.Add(panel);
            }
        }

        private StackPanel CreateStatePanel(int i, int type)
        {
            statePanelY = 330;
            StackPanel panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.VerticalAlignment = VerticalAlignment.Top;
            panel.Orientation = Orientation.Horizontal;
            panel.Name = sps[i - 1] + "_State";
            int index = 0;
            if (type == 1)
            {
                index = (int)Math.Floor((decimal)i / 2);
            }
            else
            {
                statePanelY += 335;
                index = (i - 2) / 2;
            }

            statePanelX = index * defaultWidth == 0 ? 10 : index * defaultWidth + 10;
            panel.Margin = new Thickness(statePanelX, statePanelY, 0 ,0);
            panel.Width = defaultWidth - 10;
            panel.Height = 25;
            panel.Background = Brushes.LightGray;

            TextBlock textBlock = CreateTextBlock("楷体", 14F, "State：Close", "STATE");
            TextBlock ctsBlock = CreateTextBlock("宋体", 14F, "CTS", "CTS");
            TextBlock dsrBlock = CreateTextBlock("宋体", 14F, "DSR", "DSR");
            TextBlock dcdBlock = CreateTextBlock("宋体", 14F, "DCD", "DCD");
            ctsBlock.Width = dsrBlock.Width = dcdBlock.Width = 22;
            ctsBlock.Background = dsrBlock.Background = dcdBlock.Background = Brushes.LightPink;
            ctsBlock.Margin = dsrBlock.Margin = dcdBlock.Margin = new Thickness(10, 0, 0, 0);

            panel.Children.Add(textBlock);
            panel.Children.Add(ctsBlock);
            panel.Children.Add(dsrBlock);
            panel.Children.Add(dcdBlock);

            return panel;
        }

        private TextBlock CreateTextBlock(string family, double size, string txt, string name)
        {
            TextBlock tx = new TextBlock
            {
                FontFamily = new FontFamily("宋体"),
                FontSize = size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = txt,
                Name = name
            };
            return tx;
        }

        private TextBox CreateTextBox()
        {
            TextBox tx = new TextBox
            {
                FontFamily = new FontFamily("宋体"),
                FontSize = 11F,
                Name = "recevieBox",
                Width = defaultWidth - 20,
                Height = defaultHeight - 20,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            return tx;
        }

        private void CreateSerialPort()
        {
            serial_list.Clear();
            for(int i=0; i< serialCount; i++)
            {
                SerialPort serialPort = new SerialPort
                {
                    PortName = sps[i],
                    BaudRate = baudRate,
                    DataBits = dataBits,
                    StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits, true),
                    Parity = (Parity)Enum.Parse(typeof(Parity), parity, true),
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), handshake, true),
                    Encoding = System.Text.Encoding.GetEncoding("GB2312"),

                    ReadTimeout = 500,
                    WriteTimeout = 500,
                    RtsEnable = true,
                    DtrEnable = true
                };

                serialPort.DataReceived += new SerialDataReceivedEventHandler(MySerialDataReceived);
                serial_list.Add(serialPort);
            }
        }

        private void OpenSerialPort()
        {
            foreach(SerialPort sp in serial_list)
            {
                try
                {
                    sp.Open();
                }
                catch (Exception ex)
                {
                    sp.Close();
                    Console.WriteLine("OpenSerialPort:" + ex.Message.ToString());
                }
            }
            if(!timer.IsEnabled)
            {
                timer.Start();
            }
            
        }

        private void MySerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            String recevieName = "";
            String message = "";
            try
            {
                recevieName = serialPort.PortName;
                if(!serialPort.IsOpen)
                { return; }
                message = serialPort.ReadLine();
                Console.WriteLine("recevieName:"+ recevieName + " Received data:" + message);
                sb.Clear();
                sb.Append(message);
                this.Dispatcher.BeginInvoke((Action)delegate () {
                    foreach (StackPanel recevie_panel in recevie_panel_list)
                    {
                        //Console.WriteLine("recevieName:{0} recevie_panel name:{1}",
                        //    recevieName, recevie_panel.Name);
                        if (recevieName.Equals(recevie_panel.Name))
                        {
                            TextBox tv = (TextBox)recevie_panel.Children[1];
                            //Console.WriteLine("recevie_panel textBox name:{0}", tv.Name);
                            tv.AppendText(sb.ToString());
                            tv.ScrollToEnd();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Received:" + ex.Message.ToString());
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int index = 0;
            foreach(StackPanel state_panel in state_panel_list)
            {
                try
                {
                    index = state_panel_list.IndexOf(state_panel);
        
                    //Console.WriteLine("timer tick state panel name:{0} serial port name:{1} serial port state:{2}",
                    //    state_panel.Name, serial_list[index].PortName , serial_list[index].IsOpen);
                    if (serial_list[index].IsOpen)
                    {
                        String str = serial_list[index].PortName + "测试中.";
                        serial_list[index].WriteLine(str);
                    }
                    this.Dispatcher.Invoke((Action)delegate ()
                    {
                        //Console.WriteLine("serialPort:{0} CtsHolding:{1} DsrHolding:{2} CDHolding:{3} state_panel:{4}",
                        //    serial_list[index].PortName, serial_list[index].CtsHolding,
                        //    serial_list[index].DsrHolding, serial_list[index].CDHolding, state_panel.Name);
                        
                        tb = (TextBlock)state_panel.Children[0];
                        cts = (TextBlock)state_panel.Children[1];
                        dsr = (TextBlock)state_panel.Children[2];
                        dcd = (TextBlock)state_panel.Children[3];

                        if (serial_list[index].IsOpen)
                        {
                            tb.Text = "State：Open";
                        }
                        else
                        {
                            tb.Text = "State：Closed";
                        }
                        if (serial_list[index].CtsHolding)
                        {
                            cts.Background = Brushes.Green;
                        }
                        else
                        {
                            cts.Background = Brushes.LightGray;
                        }
                        if (serial_list[index].DsrHolding)
                        {
                            dsr.Background = Brushes.Green;
                        }
                        else
                        {
                            dsr.Background = Brushes.LightGray;
                        }
                        if (serial_list[index].CDHolding)
                        {
                            dcd.Background = Brushes.Green;
                        }
                        else
                        {
                            dcd.Background = Brushes.LightGray;
                        }
                    });
                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Timer_Tick:" + ex.Message);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("serial port window Closed!!!");
            foreach(SerialPort sp in serial_list)
            {
                sp.Close();
            }
            if(timer.IsEnabled)
            {
                timer.Stop();
            }

        }
    }
}
