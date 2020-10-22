using QTest.Tools;
using System;
using System.Text;
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
            if(combobox_type.Text.Length == 0)
            {
                return;
            }
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
                    LoadGpioModel(TypeEnum.Q300P);
                    LoadGpioValue(TypeEnum.Q300P);
                    break;
                case TypeEnum.Q500G6:
                    Console.WriteLine("Q500G6...............");
                    break;
                case TypeEnum.Q500X:
                    Console.WriteLine("Q500X...............");
                    break;
                case TypeEnum.Q600P:
                    Console.WriteLine("Q600P...............");
                    LoadGpioModel(TypeEnum.Q600P);
                    LoadGpioValue(TypeEnum.Q600P);

                    break;
                default:
                    break;
            }
        }

        private void LoadGpioModel(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    byte mval = gpio.ReadGpioPortByte(0xcf);
                    Console.WriteLine("gpio model {0}", Utils.ByteToBinaryStr(mval));
                    char[] models_q300p = Utils.ByteToBinaryStr(mval).ToCharArray();
                    FormatGpioModel(models_q300p);
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    StringBuilder sb = new StringBuilder();
                    byte m1 = gpio.ReadGpioPortByte(0xcc);
                    string gp56 = Utils.ByteToBinaryStr(m1).Substring(1, 1);//gp56
                    sb.Append(gp56);
                    string gp57 = Utils.ByteToBinaryStr(m1).Substring(0, 1);//gp57
                    sb.Append(gp57);
                    byte m2 = gpio.ReadGpioPortByte(0xcd);
                    string gp60 = Utils.ByteToBinaryStr(m2).Substring(7,1);//gp60
                    sb.Append(gp60);
                    string gp61 = Utils.ByteToBinaryStr(m2).Substring(6, 1);//gp61
                    sb.Append(gp61);
                    string gp65 = Utils.ByteToBinaryStr(m2).Substring(2,1);//gp65
                    sb.Append(gp65);
                    byte m3 = gpio.ReadGpioPortByte(0xcb);
                    string gp40 = Utils.ByteToBinaryStr(m3).Substring(7,1);//gp40
                    sb.Append(gp40);
                    byte m4 = gpio.ReadGpioPortByte(0xc9);
                    string gp22 = Utils.ByteToBinaryStr(m4).Substring(5,1);//gp22
                    sb.Append(gp22);
                    string gp23 = Utils.ByteToBinaryStr(m4).Substring(4, 1);//gp23
                    sb.Append(gp23);
                    Console.WriteLine("Q600P GPIO Model:" + sb.ToString());
                    char[] models_q600p = sb.ToString().ToCharArray();
                    FormatGpioModel(models_q600p);

                    break;
                default:
                    break;
            }
        }

        private void LoadGpioValue(TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Q300P:
                    byte val = gpio.ReadGpioPortByte(0xa07);
                    Console.WriteLine("gpio value {0}", Utils.ByteToBinaryStr(val));
                    char[] val_q300p = Utils.ByteToBinaryStr(val).ToCharArray();
                    FormatGpioValue(val_q300p);
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    StringBuilder sb = new StringBuilder();
                    byte v1 = gpio.ReadGpioPortByte(0xb4);
                    string gp56 = Utils.ByteToBinaryStr(v1).Substring(1, 1);//gp56
                    sb.Append(gp56);
                    string gp57 = Utils.ByteToBinaryStr(v1).Substring(0, 1);//gp57
                    sb.Append(gp57);
                    byte v2 = gpio.ReadGpioPortByte(0xa05);
                    string gp60 = Utils.ByteToBinaryStr(v2).Substring(7, 1);//gp60
                    sb.Append(gp60);
                    string gp61 = Utils.ByteToBinaryStr(v2).Substring(6, 1);//gp61
                    sb.Append(gp61);
                    string gp65 = Utils.ByteToBinaryStr(v2).Substring(2, 1);//gp65
                    sb.Append(gp65);
                    byte v3 = gpio.ReadGpioPortByte(0xb3);
                    string gp40 = Utils.ByteToBinaryStr(v3).Substring(7, 1);//gp40
                    sb.Append(gp40);
                    byte v4 = gpio.ReadGpioPortByte(0xb1);
                    string gp22 = Utils.ByteToBinaryStr(v4).Substring(5, 1);//gp22
                    sb.Append(gp22);
                    string gp23 = Utils.ByteToBinaryStr(v4).Substring(4, 1);//gp23
                    sb.Append(gp23);

                    Console.WriteLine("Q600P GPIO Value:" + sb.ToString());
                    char[] val_q600p = sb.ToString().ToCharArray();
                    FormatGpioValue(val_q600p);
                    break;
                default:
                    break;
            }
        }

        private void Model_btn_Click(object sender, RoutedEventArgs e)
        {
            TypeEnum type = (TypeEnum)Enum.Parse(typeof(TypeEnum),
                    combobox_type.SelectedItem.ToString(), false);
            string[] arr = { gpio1_m.Text, gpio2_m.Text, gpio3_m.Text, gpio4_m.Text,
                gpio5_m.Text, gpio6_m.Text, gpio7_m.Text, gpio8_m.Text};
            string gpio_m = string.Join("", arr);
            Console.WriteLine("GPIO Model click:{0}", gpio_m);
            gpio.InitSuperIO();
            switch (type)
            {
                case TypeEnum.Q300P:
                    byte data = Convert.ToByte(gpio_m, 2);
                    gpio.SetGpioFunction(0xcf, data);
                    LoadGpioModel(TypeEnum.Q300P);
                    gpio.ExitSuperIo();
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    //先取出原始数据，然后将输入框内的数据替换进来
                    //enable gp56 57 29h<default=00>
                    gpio.SetGpioFunction(0x29, 0x40);
                    byte b1 = gpio.ReadGpioPortByte(0xcc);
                    Console.WriteLine("-----------cc:" + Utils.ByteToBinaryStr(b1));
                    string m1 = Utils.ByteToBinaryStr(b1).Remove(0, 2).Insert(0, arr[1] + arr[0]);
                    Console.WriteLine("-----------m1:" + m1);
                    byte gp_data1 = Convert.ToByte(m1, 2);
                    gpio.SetGpioFunction(0xcc, gp_data1);

                    //enable gp60 61 65 2a<default=00>
                    gpio.SetGpioFunction(0x2a, 0x0b);
                    byte b2 = gpio.ReadGpioPortByte(0xcd);
                    Console.WriteLine("-----------cd:" + Utils.ByteToBinaryStr(b2));
                    string m2 = Utils.ByteToBinaryStr(b2).Remove(2, 1).Insert(2, arr[4]).Remove(6, 2).Insert(6, arr[3] + arr[2]);
                    Console.WriteLine("-----------m2:" + m2);
                    byte gp_data2 = Convert.ToByte(m2, 2);
                    gpio.SetGpioFunction(0xcd, gp_data2);

                    //enable gp40 28h<default=00>
                    gpio.SetGpioFunction(0x28, 0x01);
                    byte b3 = gpio.ReadGpioPortByte(0xcb);
                    Console.WriteLine("-----------cb:" + Utils.ByteToBinaryStr(b3));
                    string m3 = Utils.ByteToBinaryStr(b3).Remove(7, 1).Insert(7, arr[5]);
                    Console.WriteLine("-----------m3:" + m3);
                    byte gp_data3 = Convert.ToByte(m3, 2);
                    gpio.SetGpioFunction(0xcb, gp_data3);

                    //enable gp22,23 26h<default=F3>
                    gpio.SetGpioFunction(0x26, 0xff);
                    byte b4 = gpio.ReadGpioPortByte(0xc9);
                    Console.WriteLine("-----------cb:" + Utils.ByteToBinaryStr(b4));
                    string m4 = Utils.ByteToBinaryStr(b4).Remove(4, 2).Insert(4, arr[7] + arr[6]);
                    Console.WriteLine("-----------m4:" + m4);
                    byte gp_data4 = Convert.ToByte(m4, 2);
                    gpio.SetGpioFunction(0xc9, gp_data4);

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
                    LoadGpioValue(TypeEnum.Q300P);
                    gpio.ExitSuperIo();
                    break;
                case TypeEnum.Q500G6:
                    break;
                case TypeEnum.Q500X:
                    break;
                case TypeEnum.Q600P:
                    //先取出原始数据，然后将输入框内的数据替换进来
                    //enable gp56 57 29h<default=00>
                    gpio.SetGpioFunction(0x29, 0x40);
                    byte b1 = gpio.ReadGpioPortByte(0xb4);
                    Console.WriteLine("-----------b4:" + Utils.ByteToBinaryStr(b1));
                    string v1 = Utils.ByteToBinaryStr(b1).Remove(0, 2).Insert(0, arr[1] + arr[0]);
                    Console.WriteLine("-----------v1:" + v1);
                    byte gp_data1 = Convert.ToByte(v1, 2);
                    gpio.SetGpioFunction(0xb4, gp_data1);

                    //enable gp60 61 65 2a<default=00>
                    gpio.SetGpioFunction(0x2a, 0x0b);
                    byte b2 = gpio.ReadGpioPortByte(0xa05);
                    Console.WriteLine("-----------a05:" + Utils.ByteToBinaryStr(b2));
                    string v2 = Utils.ByteToBinaryStr(b2).Remove(2, 1).Insert(2, arr[4]).Remove(6, 2).Insert(6, arr[3] + arr[2]);
                    Console.WriteLine("-----------v2:" + v2);
                    byte gp_data2 = Convert.ToByte(v2, 2);
                    gpio.SetGpioFunction(0xa05, gp_data2);

                    //enable gp40 28h<default=00>
                    gpio.SetGpioFunction(0x28, 0x01);
                    byte b3 = gpio.ReadGpioPortByte(0xb3);
                    Console.WriteLine("-----------b3:" + Utils.ByteToBinaryStr(b3));
                    string v3 = Utils.ByteToBinaryStr(b3).Remove(7, 1).Insert(7, arr[5]);
                    Console.WriteLine("-----------v3:" + v3);
                    byte gp_data3 = Convert.ToByte(v3, 2);
                    gpio.SetGpioFunction(0xb3, gp_data3);

                    //enable gp22,23 26h<default=F3>
                    gpio.SetGpioFunction(0x26, 0xff);
                    byte b4 = gpio.ReadGpioPortByte(0xb1);
                    Console.WriteLine("-----------cb:" + Utils.ByteToBinaryStr(b4));
                    string v4 = Utils.ByteToBinaryStr(b4).Remove(4, 2).Insert(4, arr[7] + arr[6]);
                    Console.WriteLine("-----------v4:" + v4);
                    byte gp_data4 = Convert.ToByte(v4, 2);
                    gpio.SetGpioFunction(0xb1, gp_data4);

                    gpio.ExitSuperIo();
                    break;
                default:
                    break;
            }
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
