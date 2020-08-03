using QTest.instances;
using QTest.Tools;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// SummaryView.xaml 的交互逻辑
    /// </summary>
    public partial class SummaryView : UserControl
    {
        ObservableCollection<BaseData> baseDataList = new ObservableCollection<BaseData>();

        public SummaryView()
        {
            InitializeComponent();
            Console.WriteLine("SummaryView......");

            ShowDataList();

            Thread t = new Thread(WorkThread);
            t.IsBackground = true;
            t.Start();

        }


        private void ShowDataList()
        {
            baseDataList.Add(new BaseData("计算机类型", ""));
            baseDataList.Add(new BaseData("操作系统",""));
            baseDataList.Add(new BaseData("系统语言",""));
            baseDataList.Add(new BaseData("计算机名", ""));
            baseDataList.Add(new BaseData("处理器", ""));
            baseDataList.Add(new BaseData("内存",""));
            baseDataList.Add(new BaseData("硬盘",""));
            baseDataList.Add(new BaseData("日期/时间", DateTime.Now.ToLocalTime().ToString()));

            summaryListView.ItemsSource = baseDataList;
        }

        void WorkThread()
        {
            //Computer.GetTest();
            string systemType = ComputerTool.GetSystemType("SystemType");
            string systemVer = ComputerTool.GetSystemVersion();
            string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name + " " +
                System.Globalization.CultureInfo.InstalledUICulture.NativeName;
            string account = ComputerTool.GetSystemType("Name");
            string cpu = ComputerTool.GetCpuName();
            string mem = ComputerTool.GetMemerySize();
            string disk = ComputerTool.GetDiskSize();
            //string time = DateTime.Now.ToLocalTime().ToString();
            
            this.Dispatcher.Invoke((Action)delegate ()
            {
                baseDataList[0].Value = systemType;
                baseDataList[1].Value = systemVer;
                baseDataList[2].Value = language;
                baseDataList[3].Value = account;
                baseDataList[4].Value = cpu;
                baseDataList[5].Value = mem;
                baseDataList[6].Value = disk;
                //baseDataList[7].Value = time;
            });

        }


    }

}
