using QTest.Tools;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QTest.Views
{
    /// <summary>
    /// GPIOView.xaml 的交互逻辑
    /// </summary>
    public partial class GPIOView : UserControl
    {
        public enum TypeEnum
        {
            Q300P = 0,
            Q500G6,
            Q500X,
            Q600P
        }
        private bool dirverStatus = false;
        private SuperIO gpio = null;

        public GPIOView()
        {
            InitializeComponent();
        }

        private void GPIO_Loaded(object sender, RoutedEventArgs e)
        {
            combobox_type.ItemsSource = System.Enum.GetNames(typeof(TypeEnum));
        }

        private bool InitGPIODriver()
        {
            bool initResult = gpio.Initialize();
            if (!initResult)
            {
                driver_status.Text = "驱动加载失败!";
                driver_status.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                driver_status.Text = "驱动加载完成!";
                gpio.InitSuperIO();
                chip_name.Text = "芯片型号：ITE " + gpio.GetChipName();
                gpio.ExitSuperIo();
            }
            return initResult;
        }

        private void Minipc_type_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Minipc_type_DropDownClosed........." + combobox_type.Text);
            if (gpio == null)
            {
                gpio = new SuperIO();
                dirverStatus = InitGPIODriver();
            }
            gpio.MinipcType = combobox_type.Text;
            if(!dirverStatus)
            {
                return;
            }
            else
            {
                TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
                gpio.InitSuperIO();
                gpio.InitLogicDevice();
                LoadGpioData(type);
                gpio.ExitSuperIo();
            }
        }

        private void LoadGpioData(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    Console.WriteLine("Q300P...............");
                    gpio.SetGpioFunction(0x2c, 0x89);
                    LoadGpioModel(0xcf);
                    LoadGpioValue(0xa07);
                    break;
                case TypeEnum.Q500G6:
                    Console.WriteLine("Q500G6...............");
                    break;
                case TypeEnum.Q500X:
                    Console.WriteLine("Q500X...............");
                    break;
                case TypeEnum.Q600P:
                    Console.WriteLine("Q600P...............");
                    //test code
                    //gp56 57
                    gpio.SetGpioFunction(0x29, 0x40);
                    gpio.SetGpioFunction(0xcc, 0xc0);
                    //gpio.SetGpioFunction(0xb4, 0xc0);
                    gpio.SetGpioFunction(0xb4, 0x00);

                    //gp60 61 65
                    gpio.SetGpioFunction(0x2a, 0x0b);
                    gpio.SetGpioFunction(0xcd, 0x23);
                    //gpio.SetGpioFunction(0xa05, 0x23);
                    gpio.SetGpioFunction(0xa05, 0x00);

                    //gp40
                    gpio.SetGpioFunction(0x28, 0x01);
                    gpio.SetGpioFunction(0xcb, 0x01);
                    //gpio.SetGpioFunction(0xb3, 0x01);
                    gpio.SetGpioFunction(0xb3, 0x00);

                    //gp22 23
                    gpio.SetGpioFunction(0x26, 0xff);
                    gpio.SetGpioFunction(0xc9, 0xff);
                    //gpio.SetGpioFunction(0xb1, 0x0c);
                    gpio.SetGpioFunction(0xb1, 0x00);

                    break;
                default:
                    break;
            }
        }

        private void LoadGpioModel(byte val)
        {
            //ushort iobase = (ushort)gpio.SuperIoInw(0x62);
            //iobase += 7;
            //Console.WriteLine("iobase {0:x}", iobase);
            //Output/Input Selection (Index CFh) 
            byte mval = gpio.ReadGpioPortByte(val);
            Console.WriteLine("gpio model {0}", Utils.ByteToBinaryStr(mval));
            char[] models = Utils.ByteToBinaryStr(mval).ToCharArray();
            FormatGpioModel(models);
        }

        private void LoadGpioValue(ushort data)
        {
            byte val = gpio.ReadGpioPortByte(data);
            Console.WriteLine("gpio value {0}", Utils.ByteToBinaryStr(val));
            char[] values = Utils.ByteToBinaryStr(val).ToCharArray();
            FormatGpioValue(values);
        }

        private void FormatGpioModel(char[] arr)
        {
            gpio1_m.Text = arr[0].ToString();
            gpio2_m.Text = arr[1].ToString();
            gpio3_m.Text = arr[2].ToString();
            gpio4_m.Text = arr[3].ToString();
            gpio5_m.Text = arr[4].ToString();
            gpio6_m.Text = arr[5].ToString();
            gpio7_m.Text = arr[6].ToString();
            gpio8_m.Text = arr[7].ToString();
        }

        private void FormatGpioValue(char[] arr)
        {
            gpio1_v.Text = arr[0].ToString();
            gpio2_v.Text = arr[1].ToString();
            gpio3_v.Text = arr[2].ToString();
            gpio4_v.Text = arr[3].ToString();
            gpio5_v.Text = arr[4].ToString();
            gpio6_v.Text = arr[5].ToString();
            gpio7_v.Text = arr[6].ToString();
            gpio8_v.Text = arr[7].ToString();
        }

        private void Model_btn_Click(object sender, RoutedEventArgs e)
        {
            TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
            string[] arr = { gpio1_m.Text, gpio2_m.Text, gpio3_m.Text, gpio4_m.Text,
                gpio5_m.Text, gpio6_m.Text, gpio7_m.Text, gpio8_m.Text};
            string str = string.Join("", arr);
            Console.WriteLine("GPIO Model click:{0}", str);
            gpio.InitSuperIO();
            switch (type)
            {
                case TypeEnum.Q300P:
                    byte data = Convert.ToByte(str, 2);
                    gpio.SetGpioFunction(0xcf, data);
                    LoadGpioModel(0xcf);
                    gpio.ExitSuperIo();
                    break;
                default:
                    break;
            }
        }

        private void Val_btn_Click(object sender, RoutedEventArgs e)
        {
            TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
            string[] arr = { gpio1_v.Text, gpio2_v.Text, gpio3_v.Text, gpio4_v.Text,
                gpio5_v.Text, gpio6_v.Text, gpio7_v.Text, gpio8_v.Text};
            string str = string.Join("", arr);
            Console.WriteLine("GPIO Value click:{0}", str);
            gpio.InitSuperIO();
            switch (type)
            {
                case TypeEnum.Q300P:
                    byte data = Convert.ToByte(str, 2);
                    gpio.SetGpioFunction(0xa07, data);
                    LoadGpioValue(0xa07);
                    gpio.ExitSuperIo();
                    break;
                default:
                    break;
            }
        }

        private void Gpio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-1]$");
            e.Handled = re.IsMatch(e.Text);
        }

        private void GPIO_Unloaded(object sender, RoutedEventArgs e)
        {
            if(gpio != null)
            {
                gpio.SysDispose();
            }
        }
    }
}
