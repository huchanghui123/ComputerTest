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
        private List<Panel> recevie_panel_list = new List<Panel>();
        private List<Panel> state_panel_list = new List<Panel>();
        private List<SerialPort> serial_list = new List<SerialPort>();

        private int baudRate = 115200;
        private int dataBits = 8;
        private String stopBits = "One";
        private String parity = "None";
        private String handshake = "None";

        public SerialPortWindow(string rate, string dbits, String sbits, String sparity, String shandshake)
        {
            InitializeComponent();

            baudRate = int.Parse(rate.ToUpperInvariant());
            dataBits = int.Parse(dbits.ToUpperInvariant());
            stopBits = sbits;
            parity = sparity;
            handshake = shandshake;

            Console.WriteLine("BaudRate:{0} DataBits:{1} StopBits:{2} Parity:{3} Handshake:{4}"
                , baudRate, dataBits, stopBits, parity, handshake);
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
        }

        private void CreatePortInfo()
        {
            String infoText = "BaudRate:" + baudRate + " DataBits:" + dataBits + " StopBits:" + stopBits
                + " Parity:" + parity + " Handshake:" + handshake;
            
            TextBlock tx = CreateTextBlock("宋体", 14F, infoText);
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
            TextBlock tx = CreateTextBlock("宋体", 14F, panel.Name);
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

            TextBlock textBlock = CreateTextBlock("楷体", 14F, "State：Close");
            textBlock.Name = "STATE";
            TextBlock ctsBlock = CreateTextBlock("宋体", 14F, "CTS");
            TextBlock dsrBlock = CreateTextBlock("宋体", 14F, "DSR");
            TextBlock dcdBlock = CreateTextBlock("宋体", 14F, "DCD");
            ctsBlock.Width = dsrBlock.Width = dcdBlock.Width = 20;
            ctsBlock.Background = dsrBlock.Background = dcdBlock.Background = Brushes.LightPink;
            ctsBlock.Margin = dsrBlock.Margin = dcdBlock.Margin = new Thickness(10, 0, 0, 0);

            panel.Children.Add(textBlock);
            panel.Children.Add(ctsBlock);
            panel.Children.Add(dsrBlock);
            panel.Children.Add(dcdBlock);

            return panel;
        }

        private TextBlock CreateTextBlock(string family, double size, string txt)
        {
            TextBlock tx = new TextBlock
            {
                FontFamily = new FontFamily("宋体"),
                FontSize = size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = txt
            };
            return tx;
        }

        private TextBox CreateTextBox()
        {
            TextBox tx = new TextBox
            {
                FontFamily = new FontFamily("宋体"),
                FontSize = 11F,
                Name = "recevie",
                Width = defaultWidth - 20,
                Height = defaultHeight - 20,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true
            };
            return tx;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("serial port window Closed!!!");
        }
    }
}
