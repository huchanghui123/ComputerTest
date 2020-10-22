using QTest.instances;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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
        public static List<BaseData> GetMemeryInfo()
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
                    mem_list.Add(new BaseData(m.Properties["Name"].Value.ToString()));
                    mem_list.Add(new BaseData("厂家", m.Properties["Manufacturer"].Value.ToString()));
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
                        disk_list.Add(new BaseData(name));
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
                string ipaddress = String.Empty;
                foreach (ManagementObject m in moc)
                {
                    try
                    {
                        type = m.Properties["PNPDeviceID"].Value.ToString();
                    }
                    catch (Exception)
                    {
                    }
                    
                    if (type.Length>3 && type.Substring(0,3) == "PCI")
                    {
                        try
                        {
                            Name = m.Properties["Name"].Value.ToString();
                            MACAddress = m.Properties["MACAddress"].Value.ToString();
                            flag = Convert.ToUInt16(m.Properties["NetConnectionStatus"].Value);
                            netEnabled = Convert.ToBoolean(m.Properties["NetEnabled"].Value);
                        }
                        catch (Exception)
                        {
                        }
                        
                        net_list.Add(new BaseData("PCI物理网卡"));
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
                            ipaddress = GetIPv4Address(Name);
                            net_list.Add(new BaseData("IPv4地址", ipaddress));
                        }
                    }
                }
                
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

        public static string GetIPv4Address(string adapterName)
        {
            String IPAddress = String.Empty;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (NetworkInterface adapter in adapters)
                {
                    //Console.WriteLine(adapter.Description);
                    //Console.WriteLine(adapter.NetworkInterfaceType.ToString());
                    if (adapter.Description.Equals(adapterName))
                    {
                        UnicastIPAddressInformationCollection unicastIPAddressInformation = adapter.GetIPProperties().UnicastAddresses;
                        if (unicastIPAddressInformation.Count > 0)
                        {
                            foreach (var item in unicastIPAddressInformation)
                            {
                                if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    IPAddress = item.Address.ToString();
                                }
                            }
                        }
                    }
                }
                return IPAddress;
            }
            catch (Exception)
            {
                return String.Empty;
            }
            
        }

    }
}
