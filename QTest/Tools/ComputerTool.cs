using QTest.instances;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Windows;

namespace QTest.Tools
{
    public class ComputerTool
    {
        //系统类型
        public static string GetSystemType(string value)
        {
            try
            {
                var st = string.Empty;
                var mos = new ManagementObjectSearcher("Select * from Win32_ComputerSystem");
                foreach (var o in mos.Get())
                {
                    var mo = (ManagementObject)o;
                    st = mo[value].ToString();

                }
                mos.Dispose();
                return st;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }

        //系统版本
        public static string GetSystemVersion()
        {
            try
            {
                var os_version = string.Empty;
                var mos = new ManagementObjectSearcher("Select * from Win32_OperatingSystem");
                foreach (var o in mos.Get())
                {
                    var mo = (ManagementObject)o;
                    os_version += mo["Caption"].ToString() + " ";
                    os_version += mo["Version"].ToString();
                }
                mos.Dispose();
                return os_version;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }
        
        //CPU型号
        public static string GetCpuName()
        {
            try
            {
                var st = string.Empty;
                var mos = new ManagementObjectSearcher("Select * from Win32_Processor");
                foreach (var o in mos.Get())
                {
                    var mo = (ManagementObject)o;
                    st = mo["Name"].ToString();
                }
                mos.Dispose();
                return st;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }

        //内存大小
        public static string GetMemerySize()
        {
            try
            {
                var st = string.Empty;
                double size = 0;
                ManagementClass mc = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject m in moc)
                {
                    if (m.Properties["Capacity"] != null)
                    {
                        size += Convert.ToDouble(m.Properties["Capacity"].Value);
                    }
                    
                }
                st = (size / 1024 / 1024 / 1024).ToString("f1") + " GB";
                //Console.WriteLine(st);
                mc = null;
                moc.Dispose();
                return st;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }

        //磁盘大小
        public static string GetDiskSize()
        {
            try
            {
                var st = string.Empty;
                double size = 0;
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject m in moc)
                {
                    if (m.Properties["Size"].Value != null)
                    {
                        size += Convert.ToDouble(m.Properties["Size"].Value);
                    }
                }
                st = (size / 1024 / 1024 / 1024).ToString("f1") + " GB";
                mc = null;
                moc.Dispose();
                return st;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }

        //通过系统日志获取开关机时间等
        #pragma warning disable CS0618 // entry.EventID 类型或成员已过时
        public static Dictionary<string, string> GetUpTime()
        {
            Dictionary<string, string> timedic = new Dictionary<string, string>();
            EventLog eventlog = new EventLog();
            //"Application"应用程序, "Security"安全, "System"系统
            eventlog.Log = "System";
            EventLogEntryCollection eventLogEntryCollection = eventlog.Entries;

            //中间的事件太多了，不需要，只需要最近的开机关机事件
            EventLogEntry[] logEntry = new EventLogEntry[eventLogEntryCollection.Count];
            eventLogEntryCollection.CopyTo(logEntry, 0);
            //最后一次开机时间
            DateTime lastPowerOn = DateTime.Now;
            //最后一次关机时间
            DateTime lastPowerOff = DateTime.Now;
            //上次关机时长
            double lastDownTime = 0;
            //已运行时间
            double upTime = 0;

            bool onstatus = false;
            bool offstatus = false;
            for (int i = logEntry.Length-1; i >= 0; i--)
            {
                if (logEntry[i].EventID == 6005 && !onstatus)
                {
                    lastPowerOn = logEntry[i].TimeGenerated;
                    timedic.Add("lastPowerOn", lastPowerOn.ToString());
                    onstatus = true;
                } else if (logEntry[i].EventID == 6006 && !offstatus)
                {
                    lastPowerOff = logEntry[i].TimeGenerated;
                    timedic.Add("lastPowerOff", lastPowerOff.ToString());
                    offstatus = true;
                }
                if (onstatus && offstatus)
                {
                    //Console.WriteLine("poweron:{0} poweroff:{1}", lastPowerOn, lastPowerOff);
                    break;
                }
            }
            lastDownTime = Math.Ceiling(ExecDateDiff(lastPowerOn, lastPowerOff));
            upTime = Math.Ceiling(ExecDateDiff(DateTime.Now, lastPowerOn));
            timedic.Add("lastDownTime", lastDownTime.ToString());
            timedic.Add("upTime", upTime.ToString());

            //Console.WriteLine(upTime.ToString());
            //Console.WriteLine(lastDownTime.ToString());

            //首次开机时间
            DateTime firsetPowerOn = DateTime.Now;
            //首次关机时间
            DateTime firsetPowerOff = DateTime.Now;
            //总运行时间
            double totalUpTime = 0;

            bool first_onstatus = false;
            bool first_offstatus = false;
            foreach (EventLogEntry entry in eventLogEntryCollection)
            {
                if (entry.EventID == 6005 && !first_onstatus)//开机
                {
                    firsetPowerOn = entry.TimeGenerated;
                    timedic.Add("firsetPowerOn", firsetPowerOn.ToString());
                    first_onstatus = true;
                }
                else if (entry.EventID == 6006 && !first_offstatus)//关机
                {
                    firsetPowerOff = entry.TimeGenerated;
                    timedic.Add("firsetPowerOff", firsetPowerOff.ToString());
                    first_offstatus = true;
                }
                if (first_onstatus && first_offstatus)
                {
                    //Console.WriteLine("poweron:{0} poweroff:{1}", firsetPowerOn, firsetPowerOff);
                    break;
                }
            }
            totalUpTime = Math.Ceiling(ExecDateDiff(DateTime.Now, firsetPowerOn));
            timedic.Add("totalUpTime", totalUpTime.ToString());
            //Console.WriteLine(totalUpTime);
            return timedic;
        }

        public static double ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3.TotalSeconds;
        }

        //主板类型
        public static string GetBoardType()
        {
            try
            {
                var st = string.Empty;
                var mos = new ManagementObjectSearcher("Select * from Win32_BaseBoard");
                foreach (var o in mos.Get())
                {
                    var mo = (ManagementObject)o;
                    st = mo["Product"].ToString();
                }
                mos.Dispose();
                return st;
            }

            catch (Exception)
            {
                return "unknow";
            }
        }

        public static string GetBios()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_BIOS");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = null;
                foreach (ManagementObject mo in moc)
                {
                    strID += mo.Properties["Manufacturer"].Value.ToString() + " ";
                    //strID += mo.Properties["Version"].Value.ToString() + " ";
                    strID += mo.Properties["Name"].Value.ToString() + " ";
                    string data = mo.Properties["ReleaseDate"].Value.ToString().Substring(0, 8);
                    strID += DateTime.ParseExact(data, "yyyyMMdd", null).ToString("yyyy/MM/dd") + " ";
                }
                mc = null;
                moc.Dispose();
                return strID;
            }
            catch (Exception)
            {
                return "unknow";
            }
        }

        //内存信息
        public static List<BaseData> GetMemoryInfo()
        {
            try
            {
                List<BaseData> mem_list = new List<BaseData>();
                ManagementClass mc = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc = mc.GetInstances();
                
                double capacity = 0;
                string size = "";
                foreach (ManagementObject m in moc)
                {
                    //Console.WriteLine(m.Properties["Manufacturer"].Value.ToString() + " " +
                    //    m.Properties["Name"].Value.ToString() + " " +
                    //    Convert.ToInt32(m.Properties["Speed"].Value) + " " +
                    //    Convert.ToDouble(m.Properties["Capacity"].Value));
                    mem_list.Add(new BaseData(m.Properties["Name"].Value.ToString(), "..\\Resources\\memory.png", 0));
                    if (m.Properties["Manufacturer"].Value.Equals("0710") ||
                        m.Properties["Manufacturer"].Value.Equals("1310"))
                    {
                        mem_list.Add(new BaseData("厂家", "Kimtigo"));
                    }
                    else
                    {
                        mem_list.Add(new BaseData("厂家", m.Properties["Manufacturer"].Value.ToString()));
                    }
                    mem_list.Add(new BaseData("主频", m.Properties["Speed"].Value.ToString()+" MHz"));
                    capacity = Convert.ToDouble(m.Properties["Capacity"].Value);
                    size = (capacity / 1024 / 1024 / 1024).ToString("f1") + " GB";
                    mem_list.Add(new BaseData("大小", size));
                }
                mc = null;
                moc.Dispose();
                return mem_list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //磁盘信息
        public static List<BaseData> GetDiskInfo()
        {
            try
            {
                List<BaseData> disk_list = new List<BaseData>();
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();

                double disksize = 0;
                string size = "";
                string name = "";
                foreach (ManagementObject m in moc)
                {
                    if (m.Properties["Size"].Value != null)
                    {
                        //Console.WriteLine(m.Properties["Caption"].Value.ToString() + " " +
                        //    m.Properties["InterfaceType"].Value.ToString() + " " +
                        //     Convert.ToDouble(m.Properties["Size"].Value));
                        name = "存储设备";
                        disk_list.Add(new BaseData(name, "..\\Resources\\hdd.png", 0));
                        disk_list.Add(new BaseData("型号", m.Properties["Caption"].Value.ToString()));
                        disk_list.Add(new BaseData("类型", m.Properties["InterfaceType"].Value.ToString()));
                        disksize = Convert.ToDouble(m.Properties["Size"].Value);
                        size = (disksize / 1024 / 1024 / 1024).ToString("f1") + " GB";
                        disk_list.Add(new BaseData("大小", size));
                    }
                }
                mc = null;
                moc.Dispose();
                return disk_list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //网络适配器
        public static List<BaseData> GetNetWorkInfo()
        {
            try
            {
                List<BaseData> net_list = new List<BaseData>();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
                ManagementObjectCollection moc = mc.GetInstances();
                String Name = String.Empty;
                String MACAddress = String.Empty;
                string type = String.Empty;
                int flag = 0;
                string status = String.Empty;
                bool netEnabled = false;
                //string temp = String.Empty;
                foreach (ManagementObject m in moc)
                {
                    try
                    {
                        type = m.Properties["PNPDeviceID"].Value.ToString();
                    }
                    catch (Exception)
                    {
                    }

                    //temp += type + " " + m.Properties["PhysicalAdapter"].Value.ToString() + "\r\n";
                    //Console.WriteLine(type + " " + m.Properties["PhysicalAdapter"].Value.ToString());
                    
                    if (type.Length>3 && 
                        (type.Substring(0,3) == "PCI"|| 
                        type.Substring(0, 3) == "USB"|| 
                        type.Substring(0, 3) == "BTH") && 
                        Boolean.Parse(m.Properties["PhysicalAdapter"].Value.ToString()))
                    {
                        try
                        {
                            Name = m.Properties["Name"].Value.ToString();
                            MACAddress = m.Properties["MACAddress"].Value.ToString();
                            flag = Convert.ToUInt16(m.Properties["NetConnectionStatus"].Value);
                            netEnabled = Convert.ToBoolean(m.Properties["NetEnabled"].Value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        NetWorkInfo nwi = GetNetWorkInfoForAdapter(Name);
                        if (nwi != null)
                        {
                            if (type.Substring(0, 3) == "PCI")
                            {
                                if(nwi.Type.ToLower().IndexOf("wireless") > -1)
                                {
                                    net_list.Add(new BaseData("PCI无线网卡", "..\\Resources\\WIFI_32px.png", 0));
                                }
                                else
                                {
                                    net_list.Add(new BaseData("PCI有线网卡", "..\\Resources\\Network.png", 0));
                                }
                                
                            }
                            else if (type.Substring(0, 3) == "USB")
                            {
                                if (nwi.Type.ToLower().IndexOf("wireless") > -1)
                                {
                                    net_list.Add(new BaseData("USB无线网卡", "..\\Resources\\WIFI_32px.png", 0));
                                }
                                else
                                {
                                    net_list.Add(new BaseData("USB有线网卡", "..\\Resources\\Network.png", 0));
                                }
                                    
                            }
                            else if (type.Substring(0, 3) == "BTH")
                            {
                                net_list.Add(new BaseData("蓝牙适配器", "..\\Resources\\Bluetooth_32px.png", 0));
                            }

                            net_list.Add(new BaseData("适配器", Name));
                            net_list.Add(new BaseData("MAC", MACAddress));

                            switch (flag)
                            {
                                case 2:
                                    status = "已连接";
                                    break;
                                case 7:
                                    status = "已断开";
                                    break;
                                default:
                                    status = "已断开";
                                    break;
                            }
                            net_list.Add(new BaseData("状态", status));

                            if (netEnabled)
                            {
                                net_list.Add(new BaseData("IPv4地址", nwi.Ip));
                            }
                        }
                    }
                }
                //MessageBox.Show(temp);

                mc = null;
                moc.Dispose();
                return net_list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static List<NetWork> GetNetWorkAdpter()
        {
            try
            {
                List<NetWork> net_list = new List<NetWork>();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
                ManagementObjectCollection moc = mc.GetInstances();
                String adapter = String.Empty;

                foreach (ManagementObject m in moc)
                {
                    try
                    {
                        adapter = m.Properties["Name"].Value.ToString();
                        if (Boolean.Parse(m.Properties["PhysicalAdapter"].Value.ToString()) == true)
                        {
                            if(AdapterIsWireless(adapter))
                            {
                                net_list.Add(new NetWork(adapter, "..\\Resources\\WIFI_32px.png"));
                            }
                            else
                            {
                                if (adapter.ToLower().IndexOf("bluetooth") > -1)
                                {
                                    net_list.Add(new NetWork(adapter, "..\\Resources\\Bluetooth_32px.png"));
                                }
                                else
                                {
                                    net_list.Add(new NetWork(adapter, "..\\Resources\\ethernet_32px.png"));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                return net_list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<NetWork> GetNetWorkAdpterInfo(string adapter)
        {
            try
            {
                List<NetWork> net_list_info = new List<NetWork>();

                ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
                ManagementObjectCollection moc = mc.GetInstances();
                String name = String.Empty;

                foreach (ManagementObject m in moc)
                {
                    try
                    {
                        name = m.Properties["Name"].Value.ToString();
                        if (Boolean.Parse(m.Properties["PhysicalAdapter"].Value.ToString()) == true)
                        {
                            if (adapter.Equals(name))
                            {
                                NetWorkInfo nwi = GetNetWorkInfoForAdapter(adapter);

                                net_list_info.Add(new NetWork("网络适配器信息", "..\\Resources\\ethernet_32px.png"));
                                net_list_info.Add(new NetWork("网络适配器", "..\\Resources\\ethernet_32px.png", name));
                                if (nwi != null)
                                {
                                    net_list_info.Add(new NetWork("接口类型", "..\\Resources\\ethernet_32px.png", nwi.Type));
                                    //net_list_info.Add(new NetWork("硬件地址(MAC)", "..\\Resources\\ethernet_32px.png", m.Properties["MACAddress"].Value.ToString()));
                                    net_list_info.Add(new NetWork("硬件地址(MAC)", "..\\Resources\\ethernet_32px.png", nwi.Mac));
                                    net_list_info.Add(new NetWork("连接名称", "..\\Resources\\ethernet_32px.png", nwi.Name));

                                    int flag = Convert.ToUInt16(m.Properties["NetConnectionStatus"].Value);
                                    string status = "已断开";
                                    if (flag == 2)
                                    {
                                        status = "已连接";
                                    }
                                    net_list_info.Add(new NetWork("连接状态", "..\\Resources\\ethernet_32px.png", status));

                                    net_list_info.Add(new NetWork("网络适配器地址", "..\\Resources\\ethernet_32px.png"));
                                    if (!string.IsNullOrEmpty(nwi.Ip) && nwi.Ip.Length > 0)
                                    {
                                        net_list_info.Add(new NetWork("IP 地址", "..\\Resources\\ethernet_32px.png", nwi.Ip));
                                    }
                                    if (!string.IsNullOrEmpty(nwi.Mask) && nwi.Mask.Length > 0)
                                    {
                                        net_list_info.Add(new NetWork("子网掩码", "..\\Resources\\ethernet_32px.png", nwi.Mask));
                                    }
                                    if (!string.IsNullOrEmpty(nwi.GateWay) && nwi.GateWay.Length > 0)
                                    {
                                        net_list_info.Add(new NetWork("网关地址", "..\\Resources\\ethernet_32px.png", nwi.GateWay));
                                    }
                                    if (!string.IsNullOrEmpty(nwi.DNS1) && nwi.DNS1.Length > 0)
                                    {
                                        net_list_info.Add(new NetWork("主DNS地址", "..\\Resources\\ethernet_32px.png", nwi.DNS1));
                                    }
                                    if (!string.IsNullOrEmpty(nwi.DNS2) && nwi.DNS2.Length > 0)
                                    {
                                        net_list_info.Add(new NetWork("备用DNS地址", "..\\Resources\\ethernet_32px.png", nwi.DNS2));
                                    }
                                }
                                

                                net_list_info.Add(new NetWork("网络适配器制造商", "..\\Resources\\ethernet_32px.png"));
                                string Manufacturer = m.Properties["Manufacturer"].Value.ToString();
                                net_list_info.Add(new NetWork("公司名称", "..\\Resources\\ethernet_32px.png", Manufacturer));
                                if(Manufacturer.ToLower().IndexOf("vmware") > -1)
                                {
                                    net_list_info.Add(new NetWork("产品信息", "..\\Resources\\ethernet_32px.png", "https://www.vmware.com"));
                                    net_list_info.Add(new NetWork("驱动程序下载", "..\\Resources\\ethernet_32px.png", "https://www.vmware.com"));
                                }
                                else if(Manufacturer.ToLower().IndexOf("virtual") > -1)
                                {
                                    net_list_info.Add(new NetWork("产品信息", "..\\Resources\\ethernet_32px.png", "https://www.virtualbox.org"));
                                    net_list_info.Add(new NetWork("驱动程序下载", "..\\Resources\\ethernet_32px.png", "https://www.virtualbox.org"));
                                }
                                else if (Manufacturer.ToLower().IndexOf("intel") > -1)
                                {
                                    net_list_info.Add(new NetWork("产品信息", "..\\Resources\\ethernet_32px.png", "https://www.intel.com/content/www/us/en/products/network-io.html"));
                                    net_list_info.Add(new NetWork("驱动程序下载", "..\\Resources\\ethernet_32px.png", "https://www.intel.com/support/network"));
                                }
                                else if (Manufacturer.ToLower().IndexOf("realtek") > -1)
                                {
                                    net_list_info.Add(new NetWork("产品信息", "..\\Resources\\ethernet_32px.png", "https://www.realtek.com/products"));
                                    net_list_info.Add(new NetWork("驱动程序下载", "..\\Resources\\ethernet_32px.png", "https://www.realtek.com/downloads"));
                                }
                                else
                                {
                                    //net_list_info.Add(new NetWork("产品信息", "..\\Resources\\ethernet_32px.png", "unknow"));
                                    //net_list_info.Add(new NetWork("驱动程序下载", "..\\Resources\\ethernet_32px.png", "unknow"));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                return net_list_info;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static NetWorkInfo GetNetWorkInfoForAdapter(string adapterName)
        {
            NetWorkInfo nwi = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface adapter in nics)
            {
                //太网连接
                bool isEthernet = (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
                //无线连接
                bool isWireless = (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
                Console.WriteLine("Name:"+adapter.Name + " Description:" + adapter.Description + 
                    " isEthernet:" + isEthernet + " isWireless:" + isWireless);
                if((isEthernet || isWireless) && adapter.Description.Equals(adapterName))
                {
                    nwi = new NetWorkInfo
                    {
                        Name = adapter.Name,
                        Mac = adapter.GetPhysicalAddress().ToString()
                    };
                    if (isEthernet)
                    {
                        if(adapter.Description.ToLower().IndexOf("bluetooth") > -1)
                        {
                            nwi.Type = "Bluetooth Ethernet";
                        }
                        else
                        {
                            nwi.Type = "Gigabit Ethernet";
                        }
                    }
                    if (isWireless)
                    {
                        nwi.Type = "802.11 Wireless Ethernet";
                    }
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if(ip.UnicastAddresses.Count > 0)
                    {
                        foreach(var item in ip.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                nwi.Ip = item.Address.ToString();
                                nwi.Mask = item.IPv4Mask.ToString();
                            }
                        }
                    }
                    if(ip.GatewayAddresses.Count > 0)
                    {
                        foreach(var item in ip.GatewayAddresses)
                        {
                            if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                nwi.GateWay = item.Address.ToString();
                            }
                        }
                    }
                    int DnsCount = ip.DnsAddresses.Count;
                    if(DnsCount == 1)
                    {
                        nwi.DNS1 = ip.DnsAddresses[0].ToString();
                    }
                    if (DnsCount == 2)
                    {
                        nwi.DNS1 = ip.DnsAddresses[0].ToString();
                        nwi.DNS2 = ip.DnsAddresses[1].ToString();
                    }
                }
            }
            return nwi;
        }

        public static bool AdapterIsWireless(String adapterName)
        {
            bool isWireless =  false;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if(adapter.Description.Equals(adapterName))
                {
                    if(adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        isWireless = false;
                    }
                    else if(adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        isWireless = true;
                    }
                }
            }
            return isWireless;
        }

    }
}
