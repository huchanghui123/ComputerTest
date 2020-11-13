using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.instances
{
    public class NetWorkInfo
    {
        //连接名称
        public string Name { get; set; }
        //ip地址
        public string Ip { get; set; }
        //子网掩码
        public string Mask { get; set; }
        //网关地址
        public string GateWay { get; set; }
        //主DNS地址
        public string DNS1 { get; set; }
        //备DNS地址
        public string DNS2 { get; set; }
        //连接速度
        public string Speed { get; set; }
    }
}
