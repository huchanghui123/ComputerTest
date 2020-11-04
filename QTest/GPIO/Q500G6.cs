using QTest.Tools;
using System;
using System.Text;

namespace QTest.GPIO
{
    public class Q500G6
    {
        //output/input
        public static char[] ReadGpioModel(SuperIO gpio)
        {
            StringBuilder sb = new StringBuilder();
            //gp51 cc<1>
            byte m1 = gpio.ReadGpioPortByte(0xcc);
            string gp51 = Utils.ByteToBinaryStr(m1).Substring(6, 1);
            sb.Append(gp51);
            //gp37  ca<7>
            byte m2 = gpio.ReadGpioPortByte(0xca);
            string gp37 = Utils.ByteToBinaryStr(m2).Substring(0, 1);
            sb.Append(gp37);
            //gp36 ca<6>
            string gp36 = Utils.ByteToBinaryStr(m2).Substring(1, 1);
            sb.Append(gp36);
            //gp23 c9<3>
            byte m3 = gpio.ReadGpioPortByte(0xc9);
            string gp23 = Utils.ByteToBinaryStr(m3).Substring(4, 1);
            sb.Append(gp23);
            //gp12 c8<2>
            byte m4 = gpio.ReadGpioPortByte(0xc8);
            string gp12 = Utils.ByteToBinaryStr(m4).Substring(5, 1);
            sb.Append(gp12);
            //gp62 cd<2>
            byte m5 = gpio.ReadGpioPortByte(0xcd);
            string gp62 = Utils.ByteToBinaryStr(m5).Substring(5, 1);
            sb.Append(gp62);
            //gp40 cb<0>
            byte m6 = gpio.ReadGpioPortByte(0xcb);
            string gp40 = Utils.ByteToBinaryStr(m6).Substring(7, 1);
            sb.Append(gp40);
            //gp10 cb<0>
            byte m7 = gpio.ReadGpioPortByte(0xc8);
            string gp10 = Utils.ByteToBinaryStr(m7).Substring(7, 1);
            sb.Append(gp10);
            Console.WriteLine("Q500G6 GPIO Model:" + sb.ToString());

            char[] models = sb.ToString().ToCharArray();
            return models;
        }

        //pin polarity
        public static char[] ReadGpioValues(SuperIO gpio)
        {
            StringBuilder sb = new StringBuilder();
            //gp51 b4<1>
            byte v1 = gpio.ReadGpioPortByte(0xb4);
            string gp56 = Utils.ByteToBinaryStr(v1).Substring(6, 1);
            sb.Append(gp56);
            //gp37 b2<7>
            byte v2 = gpio.ReadGpioPortByte(0xb2);
            string gp37 = Utils.ByteToBinaryStr(v1).Substring(0, 1);
            sb.Append(gp37);
            //gp36 b2<6>
            string gp36 = Utils.ByteToBinaryStr(v2).Substring(1, 1);
            sb.Append(gp36);
            //gp23 b1<3>
            byte v3 = gpio.ReadGpioPortByte(0xb1);
            string gp23 = Utils.ByteToBinaryStr(v3).Substring(4, 1);
            sb.Append(gp23);
            //gp12 b0<2>
            byte v4 = gpio.ReadGpioPortByte(0xb0);
            string gp12 = Utils.ByteToBinaryStr(v4).Substring(5, 1);
            sb.Append(gp12);
            //gp62 b5<2>
            byte v5 = gpio.ReadGpioPortByte(0xb5);
            string gp62 = Utils.ByteToBinaryStr(v5).Substring(5, 1);
            sb.Append(gp62);
            //gp40 b3<0>
            byte v6 = gpio.ReadGpioPortByte(0xb3);
            string gp40 = Utils.ByteToBinaryStr(v6).Substring(7, 1);
            sb.Append(gp40);
            //gp10 b0<0>
            byte v7 = gpio.ReadGpioPortByte(0xb0);
            string gp10 = Utils.ByteToBinaryStr(v7).Substring(7, 1);
            sb.Append(gp10);

            Console.WriteLine("Q500G6 GPIO Value:" + sb.ToString());
            char[] vals = sb.ToString().ToCharArray();
            return vals;
        }

        public static void SetGpioModels(SuperIO gpio, string[] arr)
        {
            //gp51 29h<1> default=46
            //output/input cch<1>
            gpio.SetGpioFunction(0x29, 0x46);
            byte b1 = gpio.ReadGpioPortByte(0xcc);
            string m1 = Utils.ByteToBinaryStr(b1).Remove(6, 1).Insert(6, arr[0]);
            byte data1 = Convert.ToByte(m1, 2);
            gpio.SetGpioFunction(0xcc, data1);
            Console.WriteLine("-----------gp51 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcc)));
            Console.WriteLine("-----------m1:" + m1);

            //gp37 gp36 27h<7 6> default=c0
            //output/input cah<7 6>
            gpio.SetGpioFunction(0x27, 0xc0);
            byte b2 = gpio.ReadGpioPortByte(0xca);
            string m2 = Utils.ByteToBinaryStr(b2).Remove(0, 2).Insert(0, arr[1] + arr[2]);
            byte data2 = Convert.ToByte(m2, 2);
            gpio.SetGpioFunction(0xca, data2);
            Console.WriteLine("-----------gp37 gp36 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xca)));
            Console.WriteLine("-----------m2:" + m2);

            //gp23 26h<3> default=08
            //output/input c9h<3>
            gpio.SetGpioFunction(0x26, 0x08);
            byte b3 = gpio.ReadGpioPortByte(0xc9);
            string m3 = Utils.ByteToBinaryStr(b3).Remove(4, 1).Insert(4, arr[3]);
            byte data3 = Convert.ToByte(m3, 2);
            gpio.SetGpioFunction(0xc9, data3);
            Console.WriteLine("-----------gp23 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xc9)));
            Console.WriteLine("-----------m3:" + m3);

            //gp12 gp10 25h<2 0> default=05
            //output/input c8h<2 0>
            gpio.SetGpioFunction(0x25, 0x05);
            byte b4 = gpio.ReadGpioPortByte(0xc8);
            string m4 = Utils.ByteToBinaryStr(b4).Remove(5, 1).Insert(5, arr[4]).Remove(5, 1).Insert(7, arr[7]);
            byte data4 = Convert.ToByte(m4, 2);
            gpio.SetGpioFunction(0xc8, data4);
            Console.WriteLine("-----------gp12 gp10 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xc8)));
            Console.WriteLine("-----------m4:" + m4);

            //gp62 29h<6> default=46
            //output/input cdh<1>
            gpio.SetGpioFunction(0x29, 0x46);
            byte b5 = gpio.ReadGpioPortByte(0xcd);
            string m5 = Utils.ByteToBinaryStr(b5).Remove(5, 1).Insert(6, arr[5]);
            byte data5 = Convert.ToByte(m5, 2);
            gpio.SetGpioFunction(0xcd, data5);
            Console.WriteLine("-----------gp51 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcd)));
            Console.WriteLine("-----------m5:" + m5);

            //gp40 28h<0> default=01
            //output/input cbh<0>
            gpio.SetGpioFunction(0x28, 0x01);
            byte b6 = gpio.ReadGpioPortByte(0xcb);
            string m6 = Utils.ByteToBinaryStr(b6).Remove(7, 1).Insert(7, arr[6]);
            byte data6 = Convert.ToByte(m6, 2);
            gpio.SetGpioFunction(0xcb, data6);
            Console.WriteLine("-----------gp40 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xcb)));
            Console.WriteLine("-----------m6:" + m6);
        }

        public static void SetGpioValues(SuperIO gpio, string[] arr)
        {
            //gp51 29h<1> default=46
            //pin polarity b4h<1>
            gpio.SetGpioFunction(0x29, 0x46);
            byte b1 = gpio.ReadGpioPortByte(0xb4);
            string v1 = Utils.ByteToBinaryStr(b1).Remove(6, 1).Insert(6, arr[0]);
            byte data1 = Convert.ToByte(v1, 2);
            gpio.SetGpioFunction(0xb4, data1);
            Console.WriteLine("-----------gp51 value:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb4)));
            Console.WriteLine("-----------v1:" + v1);

            //gp37 gp36 27h<7 6> default=c0
            //pin polarity b2h<7 6>
            gpio.SetGpioFunction(0x27, 0xc0);
            byte b2 = gpio.ReadGpioPortByte(0xb2);
            string v2 = Utils.ByteToBinaryStr(b2).Remove(0, 2).Insert(0, arr[1] + arr[2]);
            byte data2 = Convert.ToByte(v2, 2);
            gpio.SetGpioFunction(0xb2, data2);
            Console.WriteLine("-----------gp37 gp36 value:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb2)));
            Console.WriteLine("-----------v2:" + v2);

            //gp23 26h<3> default=08
            //pin polarity  b1h<3>
            gpio.SetGpioFunction(0x26, 0x08);
            byte b3 = gpio.ReadGpioPortByte(0xb1);
            string v3 = Utils.ByteToBinaryStr(b3).Remove(4, 1).Insert(4, arr[3]);
            byte data3 = Convert.ToByte(v3, 2);
            gpio.SetGpioFunction(0xb1, data3);
            Console.WriteLine("-----------gp23 value:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb1)));
            Console.WriteLine("-----------v3:" + v3);

            //gp12 gp10 25h<2 0> default=05
            //pin polarity b0h<2 0>
            gpio.SetGpioFunction(0x25, 0x05);
            byte b4 = gpio.ReadGpioPortByte(0xb0);
            string v4 = Utils.ByteToBinaryStr(b4).Remove(5, 1).Insert(5, arr[4]).Remove(5, 1).Insert(7, arr[7]);
            byte data4 = Convert.ToByte(v4, 2);
            gpio.SetGpioFunction(0xb0, data4);
            Console.WriteLine("-----------gp12 gp10 value:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb0)));
            Console.WriteLine("-----------v4:" + v4);

            //gp62 29h<6> default=46
            //pin polarity b5h<2>
            gpio.SetGpioFunction(0x29, 0x46);
            byte b5 = gpio.ReadGpioPortByte(0xb5);
            string v5 = Utils.ByteToBinaryStr(b5).Remove(5, 1).Insert(6, arr[5]);
            byte data5 = Convert.ToByte(v5, 2);
            gpio.SetGpioFunction(0xb5, data5);
            Console.WriteLine("-----------gp62 value:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb5)));
            Console.WriteLine("-----------v5:" + v5);

            //gp40 28h<0> default=01
            //pin polarity b3h<0>
            gpio.SetGpioFunction(0x28, 0x01);
            byte b6 = gpio.ReadGpioPortByte(0xb3);
            string v6 = Utils.ByteToBinaryStr(b6).Remove(7, 1).Insert(7, arr[6]);
            byte data6 = Convert.ToByte(v6, 2);
            gpio.SetGpioFunction(0xb3, data6);
            Console.WriteLine("-----------gp40 model:" + Utils.ByteToBinaryStr(gpio.ReadGpioPortByte(0xb3)));
            Console.WriteLine("-----------v6:" + v6);

        }


    }
}
