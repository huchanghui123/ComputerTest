using QTest.instances;
using QTest.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;

namespace QTest.Views
{
    /// <summary>
    /// UpTimeView.xaml 的交互逻辑
    /// </summary>
    public partial class UpTimeView : UserControl
    {
        ObservableCollection<BaseData> baseDataList = new ObservableCollection<BaseData>();

        public UpTimeView()
        {
            InitializeComponent();

            ShowDataList();

            Thread t = new Thread(WorkThread);
            t.IsBackground = true;
            t.Start();
        }

        

        private void ShowDataList()
        {
            baseDataList.Add(new BaseData("上次开机", ""));
            baseDataList.Add(new BaseData("上次关机", ""));
            baseDataList.Add(new BaseData("上次关机时长", ""));
            baseDataList.Add(new BaseData("当前时间", DateTime.Now.ToString()));
            baseDataList.Add(new BaseData("已运行时间", ""));
            baseDataList.Add(new BaseData("首次开机", ""));
            baseDataList.Add(new BaseData("首次关机", ""));
            baseDataList.Add(new BaseData("总共运行", ""));

            uptimeListView.ItemsSource = baseDataList;
        }

        private void WorkThread()
        {
            Dictionary<string, string> timedic = ComputerTool.GetUpTime();
            string lastPowerOn = timedic["lastPowerOn"];
            string lastPowerOff = timedic["lastPowerOff"];
            string firsetPowerOn = timedic["firsetPowerOn"];
            string firsetPowerOff = timedic["firsetPowerOff"];

            string lastDownTime = timedic["lastDownTime"];
            string upTime = timedic["upTime"];
            string totalUpTime = timedic["totalUpTime"];

            int lastDownTimeInt = Convert.ToInt32(lastDownTime);
            int upTimeInt = Convert.ToInt32(upTime);
            int totalUpTimeInt = Convert.ToInt32(totalUpTime);

            lastDownTime = lastDownTime + "秒 - (" + GetClock(lastDownTimeInt) + ")";
            upTime = upTime + "秒 - (" + GetClock(upTimeInt) + ")";
            totalUpTime = totalUpTime + "秒 - (" + GetClock(totalUpTimeInt) + ")";

            this.Dispatcher.Invoke((Action)delegate ()
            {
                baseDataList[0].Value = lastPowerOn;
                baseDataList[1].Value = lastPowerOff;
                baseDataList[2].Value = lastDownTime;
                baseDataList[4].Value = upTime;
                baseDataList[5].Value = firsetPowerOn;
                baseDataList[6].Value = firsetPowerOff;
                baseDataList[7].Value = totalUpTime;
            });

        }

        private string GetClock(int sec)
        {
            int day = 0, hour = 0, min = 0;
            min = sec / 60;
            sec = sec % 60;
            hour = min / 60;
            min = min % 60;
            day = hour / 24;
            hour = hour % 24;
            return string.Format("{0}天{1:00}小时{2:00}分{3:00}秒", day, hour, min, sec);
        }
    }
}
