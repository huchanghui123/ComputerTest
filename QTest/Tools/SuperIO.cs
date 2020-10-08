using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.Tools
{
    public class SuperIO
    {
        private OpenLibSys.Ols MyOls;
        private string minipcType;
        public string MinipcType { get => minipcType; set => minipcType = value; }

        public bool Initialize()
        {
            MyOls = new OpenLibSys.Ols();
            return MyOls.GetStatus() == (uint)OpenLibSys.Ols.Status.NO_ERROR;
        }

        public void SysDispose()
        {
            if(MyOls!=null)
            {
                MyOls.Dispose();
            }
        }

        public void InitSuperIO()
        {
            MyOls.WriteIoPortByte(0x2e, 0x87);
            MyOls.WriteIoPortByte(0x2e, 0x01);
            MyOls.WriteIoPortByte(0x2e, 0x55);
            MyOls.WriteIoPortByte(0x2e, 0x55);
        }

        public int SuperIoInw(byte data)
        {
            int val;
            MyOls.WriteIoPortByte(0x2e, data++);
            val = MyOls.ReadIoPortByte(0x2f) << 8;
            Console.WriteLine("SuperIo_Inw  val1:" + Convert.ToString(val, 16));
            MyOls.WriteIoPortByte(0x2e, data);
            val |= MyOls.ReadIoPortByte(0x2f);
            Console.WriteLine("SuperIo_Inw  val2:" + Convert.ToString(val, 16));
            return val;
        }

        public void ExitSuperIo()
        {
            MyOls.WriteIoPortByte(0x2e, 0x02);
            MyOls.WriteIoPortByte(0x2f, 0x02);
        }

        public string GetChipName()
        {
            ushort chip_type;
            chip_type = (ushort)SuperIoInw(0x20);
            Console.WriteLine("chip type :" + Convert.ToString(chip_type, 16));
            return "IT" + Convert.ToString(chip_type, 16);
        }

        public void InitLogicDevice()
        {
            //select logic device
            MyOls.WriteIoPortByte(0x2e, 0x07);
            MyOls.WriteIoPortByte(0x2f, 0x07);
        }

        //reg:看门狗配置寄存器内存地址 value：寄存器配置
        public void EnableWatchDog(byte reg, byte value)
        {
            MyOls.WriteIoPortByte(0x2e, reg);
            MyOls.WriteIoPortByte(0x2f, value);
        }

        public void FeedDog(ushort time)
        {
            InitSuperIO();
            //Watch Dog Timer Time-out Value (LSB) Register (Index=73h, Default=38h)
            MyOls.WriteIoPortByte(0x2e, 0x73);
            MyOls.WriteIoPortByte(0x2f, Convert.ToByte(time & 0xff));//取低8位数据
            //Watch Dog Timer Time-out Value (MSB) Register (Index=74h, Default=00h)
            if (time > 255)
            {
                MyOls.WriteIoPortByte(0x2e, 0x74);
                MyOls.WriteIoPortByte(0x2f, Convert.ToByte(time >> 8));//取高8位数据
            }
            else
            {
                MyOls.WriteIoPortByte(0x2e, 0x74);
                MyOls.WriteIoPortByte(0x2f, 0x00);
            }
            ExitSuperIo();
        }

        public void StopWatchDog(byte reg, byte value)
        {
            InitSuperIO();
            MyOls.WriteIoPortByte(0x2e, reg);
            MyOls.WriteIoPortByte(0x2f, value);
            Console.WriteLine("WatchDog stopped!!!");
            ExitSuperIo();
        }

        /*
         * GPIO 
         */
        public void SetGpioFunction(byte reg, byte value)
        {
            MyOls.WriteIoPortByte(0x2e, reg);
            MyOls.WriteIoPortByte(0x2f, value);
        }

        //Q300
        public void SetGpioFunction(ushort reg, byte value)
        {
            MyOls.WriteIoPortByte(reg, value);
        }

        public byte ReadGpioPortByte(byte date)
        {
            byte b = 0;
            try
            {
                MyOls.WriteIoPortByte(0x2e, date);
                b = MyOls.ReadIoPortByte(0x2f);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occured:\n" + ex.Message);
            }
            return b;
        }

        public byte ReadGpioPortByte(ushort data)
        {
            byte b = 0;
            try
            {
                b = MyOls.ReadIoPortByte(data);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occured:\n" + ex.Message);
            }
            return b;
        }
    }
}
