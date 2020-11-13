using QTest.instances;
using QTest.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// NetWorkInfo.xaml 的交互逻辑
    /// </summary>
    public partial class NetWorkInfoView : UserControl
    {
        private List<NetWork> networkList;
        private List<NetWork> networkInfoList;
        ObservableCollection<NetWork> NetWorkDataList = new ObservableCollection<NetWork>();
        ObservableCollection<NetWork> NetWorkInfoDataList = new ObservableCollection<NetWork>();

        public NetWorkInfoView()
        {
            InitializeComponent();

            LoadNetWorkDevice();
        }

        private void LoadNetWorkDevice()
        {
            networkList = ComputerTool.GetNetWorkAdpter();
            if (networkList != null)
            {
                if(NetWorkView.Items.Count == 0)
                {
                    foreach(NetWork net in networkList)
                    {
                        NetWorkDataList.Add(net);
                    }
                    NetWorkView.ItemsSource = NetWorkDataList;
                }
                //NetWorkView.Focus();
                NetWorkView.SelectedIndex = 0;
            }
            
        }

        private void NetWorkView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("NetWorkView_SelectionChanged...");
            NetInfoView.ItemsSource = null;
            NetInfoView.Items.Clear();
            NetWorkInfoDataList.Clear();

            ListView netList = (ListView)sender;
            NetWork network = netList.SelectedItem as NetWork;
            Console.WriteLine(network.NetDevice);

            ComputerTool.GetNetWorkInfoForAdapter(network.NetDevice);

            networkInfoList = ComputerTool.GetNetWorkAdpterInfo(network.NetDevice);
            if (networkInfoList != null)
            {
                if (NetInfoView.Items.Count == 0)
                {
                    foreach (NetWork net in networkInfoList)
                    {
                        NetWorkInfoDataList.Add(net);
                    }
                    NetInfoView.ItemsSource = NetWorkInfoDataList;
                }
            }

        }
    }
}
