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
        private SuperIO gpio = null;

        public GPIOView()
        {
            InitializeComponent();
        }

        private void GPIO_Loaded(object sender, RoutedEventArgs e)
        {
            string[] types = { "Q300", "Q500" };
            minipc_type.ItemsSource = types;
        }

        private void Minipc_type_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Minipc_type_DropDownClosed........." + minipc_type.Text);
            if(gpio == null)
            {
                gpio = new SuperIO();
                InitGPIODriver();
            }
            gpio.MinipcType = minipc_type.Text;
        }

        private void InitGPIODriver()
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
        }


        private void Model_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Val_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Gpio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-1]$");
            e.Handled = re.IsMatch(e.Text);
        }

        private void GPIO_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
