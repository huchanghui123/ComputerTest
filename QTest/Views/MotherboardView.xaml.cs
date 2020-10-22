using QTest.instances;
using QTest.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// MotherboardView.xaml 的交互逻辑
    /// </summary>
    public partial class MotherboardView : UserControl
    {
        ObservableCollection<BaseData> MBDataList = new ObservableCollection<BaseData>();
        List<BaseData> memList;
        List<BaseData> diskList;
        List<BaseData> netList;

        public MotherboardView()
        {
            InitializeComponent();

            ShowDataList();
        }

        private void ShowDataList()
        {
            MBDataList.Add(new BaseData("主板"));
            MBDataList.Add(new BaseData("型号", ComputerTool.GetBoardType()));
            MBDataList.Add(new BaseData("BIOS", ComputerTool.GetBios()));

            memList = ComputerTool.GetMemeryInfo();
            if (memList != null)
            {
                foreach (BaseData baseData in memList)
                {
                    MBDataList.Add(baseData);
                }
            }

            diskList = ComputerTool.GetDiskInfo();
            if (diskList != null)
            {
                foreach (BaseData baseData in diskList)
                {
                    MBDataList.Add(baseData);
                }
            }

            netList = ComputerTool.GetNetWorkInfo();
            if (netList != null)
            {
                foreach (BaseData baseData in netList)
                {
                    MBDataList.Add(baseData);
                }
            }

            MBListView.ItemsSource = MBDataList;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnLoaded......................");
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("OnUnloaded......................");
        }
    }
}
