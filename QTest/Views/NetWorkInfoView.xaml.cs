using System;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// NetWorkInfo.xaml 的交互逻辑
    /// </summary>
    public partial class NetWorkInfoView : UserControl
    {
        public NetWorkInfoView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("NetWorkInfo ViewOnLoaded......................");
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("NetWorkInfoView OnUnloaded......................");
        }
    }
}
