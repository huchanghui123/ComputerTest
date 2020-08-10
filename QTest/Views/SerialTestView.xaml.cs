using System;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QTest.Views
{
    /// <summary>
    /// SerialTestView.xaml 的交互逻辑
    /// </summary>
    public partial class SerialTestView : UserControl
    {
        private long receive_count = 0; //接收字节计数
        private long send_count = 0;    //发送字节计数

        private string indata = "";
        private StringBuilder sb = new StringBuilder();
        private SerialPort serialPort;
        private DispatcherTimer timer;

        private SerialPortWindow spWindow;

        public SerialTestView()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
            serialPort.PinChanged += new SerialPinChangedEventHandler(SerialPort_PinChange);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.IsEnabled = false;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Send_Click(SendBtn, new RoutedEventArgs());
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnLoaded......................");
            portName.ItemsSource = SerialPort.GetPortNames();
            portName.SelectedIndex = 0;
            serialPort.PortName = portName.Text;
            string[] baudDatas = { "9600", "19200", "38400", "56000", "57600", "115200", "128000" };
            baudRate.ItemsSource = baudDatas;
            baudRate.SelectedIndex = 5;
            string[] stopBitData = { StopBits.One + "", StopBits.Two + "", StopBits.OnePointFive + "" };
            stopBits.ItemsSource = stopBitData;
            stopBits.SelectedIndex = 0;
            string[] parityData = { Parity.None + "", Parity.Odd + "", Parity.Even + "", Parity.Mark + "", Parity.Space + "" };
            parity.ItemsSource = parityData;
            parity.SelectedIndex = 0;
            string[] handShakeData = { Handshake.None + "", Handshake.XOnXOff + "", Handshake.RequestToSend + "", Handshake.RequestToSendXOnXOff + "" };
            handShake.ItemsSource = handShakeData;
            handShake.SelectedIndex = 0;

            serialPort.Encoding = System.Text.Encoding.GetEncoding("GB2312");
        }

        private void OpenSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OpenSerialPort_Click");
            try
            {
                serialPort.PortName = portName.Text;
                serialPort.BaudRate = int.Parse(baudRate.Text);
                serialPort.DataBits = int.Parse(dataBits.Text.ToUpperInvariant());
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits.Text, true);
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), parity.Text, true);

                if (rtsCheck.IsChecked.Value)
                {
                    serialPort.RtsEnable = true;
                }
                else
                {
                    serialPort.RtsEnable = false;
                }
                if (dtrCheck.IsChecked.Value)
                {
                    serialPort.DtrEnable = true;
                }
                else
                {
                    serialPort.DtrEnable = false;
                }

                EnableButton("open");

                serialPort.Open();
                Console.WriteLine("portname:{0} status:{1}", serialPort.PortName, serialPort.IsOpen);

                if (serialPort.CtsHolding)
                {
                    ctsStatus.Background = Brushes.Green;
                }
                if (serialPort.DsrHolding)
                {
                    dsrStatus.Background = Brushes.Green;
                }
                if (serialPort.CDHolding)
                {
                    dcdStatus.Background = Brushes.Green;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OpenSerialPort_Click Exception!!!!{0}" , ex.Message);
                MessageBox.Show(ex.Message);
                EnableButton("close");
            }
        }

        private void CloseSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("CloseSerialPort_Click");
            
            if (serialPort.IsOpen)
            {
                try
                {
                    timer.Stop();
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("close port Exception!!!!!!!!!!{0}", ex.Message);
                    MessageBox.Show(ex.Message);
                    serialPort = new SerialPort();
                }
                finally
                {
                    EnableButton("close");
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("SerialPort_DataReceived...");
            SerialPort serialPort1 = (SerialPort)sender;
            byte[] ReDatas = new byte[serialPort1.BytesToRead];
            
            try
            {
                indata = serialPort1.ReadLine();
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke((Action)delegate () {
                    recevierBox.AppendText(ex.Message + " Received:" + ReDatas.Length);
                    SendBtn.IsEnabled = false;
                });
            }
                
            int num = indata.Length;
            receive_count += num;

            sb.Clear();
            sb.Append(indata);
            indata = "";

            try
            {
                this.Dispatcher.BeginInvoke((Action)delegate () {
                    if (brBox.IsChecked.Value)
                    {
                        sb.Append("\r\n");
                    }
                    recevierBox.AppendText(sb.ToString());
                    recevierBytes.Text = receive_count.ToString() + "Bytes";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerialPort_DataReceived Exception!!!!{0}", ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void SerialPort_PinChange(object sender, SerialPinChangedEventArgs e)
        {
            SerialPort serialPort1 = sender as SerialPort;
            if (serialPort1.CtsHolding)
            {
                InvokeUpdateStatePanel(ctsStatus, Brushes.Green);
            }
            else
            {
                InvokeUpdateStatePanel(ctsStatus, Brushes.LightGray);
            }
            if (serialPort1.DsrHolding)
            {
                InvokeUpdateStatePanel(dsrStatus, Brushes.Green);
            }
            else
            {
                InvokeUpdateStatePanel(dsrStatus, Brushes.LightGray);
            }
            if (serialPort1.CDHolding)
            {
                InvokeUpdateStatePanel(dcdStatus, Brushes.Green);
            }
            else
            {
                InvokeUpdateStatePanel(dcdStatus, Brushes.LightGray);
            }
        }

        private void Send_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("Send_Click..............." + timer.IsEnabled);
            byte[] temp = new byte[1];
            try
            {
                if (serialPort.IsOpen)
                {
                    string message = sendTextBox.Text;
                    serialPort.WriteLine(message);

                    send_count += message.Length;
                    sendBytes.Text = send_count + "Bytes";

                    if (!timer.IsEnabled)
                    {
                        double value = Convert.ToDouble(delayNumer.Text);
                        timer.Interval = TimeSpan.FromMilliseconds(value);
                        timer.Start();
                        delayNumer.IsEnabled = false;
                    }
                    StopSendBtn.IsEnabled = true;
                    SendBtn.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send_Click!!!!{0}", ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void StopSend_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen && timer.IsEnabled)
            {
                timer.Stop();
                delayNumer.IsEnabled = true;
                StopSendBtn.IsEnabled = false;
                SendBtn.IsEnabled = true;
            }
        }

        private void RecevicerClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            recevierBox.Text = "";
            receive_count = 0;
            recevierBytes.Text = receive_count.ToString() + "Bytes";
        }

        private void SendClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            sendTextBox.Text = "";
            send_count = 0;
            sendBytes.Text = send_count.ToString() + "Bytes";
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnUnloaded......................");
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            timer.Stop();
        }

        private void EnableButton(string status)
        {
            Console.WriteLine("EnableButton----------------status:{0}", status);
            if (status.Equals("open"))
            {
                portName.IsEnabled = false;
                baudRate.IsEnabled = false;
                dataBits.IsEnabled = false;
                stopBits.IsEnabled = false;
                parity.IsEnabled = false;
                handShake.IsEnabled = false;

                rtsCheck.IsEnabled = false;
                dtrCheck.IsEnabled = false;
                SendBtn.IsEnabled = true;
                OpenBtn.IsEnabled = false;
                OpenAllBtn.IsEnabled = false;

                serialImage.Source = new BitmapImage(
                    new Uri("..\\Resources\\sun_128px_open.png", UriKind.Relative));
            }
            else if (status.Equals("close"))
            {
                portName.IsEnabled = true;
                baudRate.IsEnabled = true;
                dataBits.IsEnabled = true;
                stopBits.IsEnabled = true;
                parity.IsEnabled = true;
                handShake.IsEnabled = true;

                rtsCheck.IsEnabled = true;
                dtrCheck.IsEnabled = true;
                SendBtn.IsEnabled = false;
                OpenBtn.IsEnabled = true;
                OpenAllBtn.IsEnabled = true;

                delayNumer.IsEnabled = true;
                StopSendBtn.IsEnabled = false;

                ctsStatus.Background = Brushes.LightGray;
                dsrStatus.Background = Brushes.LightGray;
                dcdStatus.Background = Brushes.LightGray;

                serialImage.Source = new BitmapImage(
                    new Uri("..\\Resources\\sun_128px_close.png", UriKind.Relative));
            }
        }

        private void InvokeUpdateStatePanel(TextBlock tb, SolidColorBrush color)
        {
            this.Dispatcher.Invoke((Action)delegate ()
            {
                tb.Background = color;
            });
        }

        private void OpenAllSerialPort_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OpenAllSerialPort_Click");
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                EnableButton("close");
            }
            spWindow = new SerialPortWindow(baudRate.Text, dataBits.Text,
                stopBits.Text, parity.Text, handShake.Text)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 150,
                Top = 20
            };
            spWindow.ShowDialog();
        }
    }
}
