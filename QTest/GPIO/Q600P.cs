using QTest.Tools;
using System;
using System.Text;

namespace QTest.GPIO
{
    public class Q600P
    {
        public static char[] ReadGpioModel(SuperIO gpio)
        {
            StringBuilder sb = new StringBuilder();
            byte m1 = gpio.ReadGpioPortByte(0xcc);
            string gp56 = Utils.ByteToBinaryStr(m1).Substring(1, 1);//gp56
            sb.Append(gp56);
            string gp57 = Utils.ByteToBinaryStr(m1).Substring(0, 1);//gp57
            sb.Append(gp57);
            byte m2 = gpio.ReadGpioPortByte(0xcd);
            string gp60 = Utils.ByteToBinaryStr(m2).Substring(7, 1);//gp60
            sb.Append(gp60);
            string gp61 = Utils.ByteToBinaryStr(m2).Substring(6, 1);//gp61
            sb.Append(gp61);
            string gp65 = Utils.ByteToBinaryStr(m2).Substring(2, 1);//gp65
            sb.Append(gp65);
            byte m3 = gpio.ReadGpioPortByte(0xcb);
            string gp40 = Utils.ByteToBinaryStr(m3).Substring(7, 1);//gp40
            sb.Append(gp40);
            byte m4 = gpio.ReadGpioPortByte(0xc9);
            string gp22 = Utils.ByteToBinaryStr(m4).Substring(5, 1);//gp22
            sb.Append(gp22);
            string gp23 = Utils.ByteToBinaryStr(m4).Substring(4, 1);//gp23
            sb.Append(gp23);
            Console.WriteLine("Q600P GPIO Model:" + sb.ToString());

            char[] models = sb.ToString().ToCharArray();
            return models;
        }

        public static char[] ReadGpioValues(SuperIO gpio)
        {
            StringBuilder sb = new StringBuilder();

            byte v1 = gpio.ReadGpioPortByte(0xb4);
            string gp56 = Utils.ByteToBinaryStr(v1).Substring(1, 1);//gp56
            sb.Append(gp56);
            string gp57 = Utils.ByteToBinaryStr(v1).Substring(0, 1);//gp57
            sb.Append(gp57);

            //byte v2 = gpio.ReadGpioPortByte(0xa05);
            byte v2 = gpio.ReadGpioPortByte(0xb5);
            string gp60 = Utils.ByteToBinaryStr(v2).Substring(7, 1);//gp60
            sb.Append(gp60);
            string gp61 = Utils.ByteToBinaryStr(v2).Substring(6, 1);//gp61
            sb.Append(gp61);
            string gp65 = Utils.ByteToBinaryStr(v2).Substring(2, 1);//gp65
            sb.Append(gp65);

            byte v3 = gpio.ReadGpioPortByte(0xb3);
            string gp40 = Utils.ByteToBinaryStr(v3).Substring(7, 1);//gp40
            sb.Append(gp40);
            byte v4 = gpio.ReadGpioPortByte(0xb1);
            string gp22 = Utils.ByteToBinaryStr(v4).Substring(5, 1);//gp22
            sb.Append(gp22);
            string gp23 = Utils.ByteToBinaryStr(v4).Substring(4, 1);//gp23
            sb.Append(gp23);

            Console.WriteLine("Q600P GPIO Value:" + sb.ToString());

            char[] vals = sb.ToString().ToCharArray();
            return vals;
        }

        public static void SetGpioModels(SuperIO gpio, string[] arr)
        {
            //先取出原始数据，然后将输入框内的数据替换进来
            //enable gp56 57 60 61 29h<6> gp65 29<7>
            //output/input gp56:cc<6> gp57:cc<7>
            gpio.SetGpioFunction(0x29, 0xc0);
            byte b1 = gpio.ReadGpioPortByte(0xcc);
            string m1 = Utils.ByteToBinaryStr(b1).Remove(0, 2).Insert(0, arr[1] + arr[0]);
            byte gp_data1 = Convert.ToByte(m1, 2);
            gpio.SetGpioFunction(0xcc, gp_data1);
            Console.WriteLine("-----------gp56 57 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcc)));
            Console.WriteLine("-----------m1:" + m1);

            //output/input gp60:cd<0> gp61:cd<1> gp65:cd<5>
            byte b2 = gpio.ReadGpioPortByte(0xcd);
            string m2 = Utils.ByteToBinaryStr(b2).Remove(2, 1).Insert(2, arr[4]).Remove(6, 2).Insert(6, arr[3] + arr[2]);
            byte gp_data2 = Convert.ToByte(m2, 2);
            gpio.SetGpioFunction(0xcd, gp_data2);
            Console.WriteLine("-----------gp60 61 65 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcd)));
            Console.WriteLine("-----------m2:" + m2);

            //enable gp40 28h<default=00>
            gpio.SetGpioFunction(0x28, 0x01);
            byte b3 = gpio.ReadGpioPortByte(0xcb);
            string m3 = Utils.ByteToBinaryStr(b3).Remove(7, 1).Insert(7, arr[5]);
            byte gp_data3 = Convert.ToByte(m3, 2);
            gpio.SetGpioFunction(0xcb, gp_data3);
            Console.WriteLine("-----------gp40 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcb)));
            Console.WriteLine("-----------m3:" + m3);

            //enable gp22,23 26h<default=F3>
            gpio.SetGpioFunction(0x26, 0xff);
            byte b4 = gpio.ReadGpioPortByte(0xc9);
            string m4 = Utils.ByteToBinaryStr(b4).Remove(4, 2).Insert(4, arr[7] + arr[6]);
            byte gp_data4 = Convert.ToByte(m4, 2);
            gpio.SetGpioFunction(0xc9, gp_data4);
            Console.WriteLine("-----------gp22 23 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xc9)));
            Console.WriteLine("-----------m4:" + m4);
        }

        public static void SetGpioValues(SuperIO gpio, string[] arr)
        {
            //先取出原始数据，然后将输入框内的数据替换进来
            gpio.SetGpioFunction(0x29, 0xc0);//gp56 56 60 61 62 65 enable
            //pin polarity gp56 57 b4<6 7>
            byte b1 = gpio.ReadGpioPortByte(0xb4);
            string v1 = Utils.ByteToBinaryStr(b1).Remove(0, 2).Insert(0, arr[1] + arr[0]);
            byte gp_data1 = Convert.ToByte(v1, 2);
            gpio.SetGpioFunction(0xb4, gp_data1);
            Console.WriteLine("-----------gp56 57:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb4)));
            Console.WriteLine("-----------v1:" + v1);

            //pin polarity gp60 61 65 b5<0 1 5>
            //gpio.SetGpioFunction(0x2a, 0x0b);
            //byte b2 = gpio.ReadGpioPortByte(0xa05);
            byte b2 = gpio.ReadGpioPortByte(0xb5);
            string v2 = Utils.ByteToBinaryStr(b2).Remove(2, 1).Insert(2, arr[4]).Remove(6, 2).Insert(6, arr[3] + arr[2]);
            byte gp_data2 = Convert.ToByte(v2, 2);
            //gpio.SetGpioFunction(0xa05, gp_data2);
            gpio.SetGpioFunction(0xb5, gp_data2);
            Console.WriteLine("-----------gp60 61 65:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xa05)));
            Console.WriteLine("-----------v2:" + v2);

            //pin polarity gp40 28h<0>
            gpio.SetGpioFunction(0x28, 0x01);
            byte b3 = gpio.ReadGpioPortByte(0xb3);
            string v3 = Utils.ByteToBinaryStr(b3).Remove(7, 1).Insert(7, arr[5]);
            byte gp_data3 = Convert.ToByte(v3, 2);
            gpio.SetGpioFunction(0xb3, gp_data3);
            Console.WriteLine("-----------gp40:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb3)));
            Console.WriteLine("-----------v3:" + v3);

            //pin polarity gp22,23 26h<2 3>
            gpio.SetGpioFunction(0x26, 0xff);
            byte b4 = gpio.ReadGpioPortByte(0xb1);
            string v4 = Utils.ByteToBinaryStr(b4).Remove(4, 2).Insert(4, arr[7] + arr[6]);
            byte gp_data4 = Convert.ToByte(v4, 2);
            gpio.SetGpioFunction(0xb1, gp_data4);
            Console.WriteLine("-----------gp22 23:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb1)));
            Console.WriteLine("-----------v4:" + v4);

        }
    }
}
