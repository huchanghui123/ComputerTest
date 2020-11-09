using QTest.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.GPIO
{
    public class Q300P
    {
        public static char[] ReadGpioModel(SuperIO gpio)
        {
            byte mval = gpio.ReadGpioPortByte(0xcf);
            char[] models_q300p = Utils.ByteToBinaryStr(mval).ToCharArray();
            return models_q300p;
        }

        public static char[] ReadGpioValues(SuperIO gpio)
        {
            byte val = gpio.ReadGpioPortByte(0xa07);
            Console.WriteLine("gpio value {0}", Utils.ByteToBinaryStr(val));
            char[] val_q300p = Utils.ByteToBinaryStr(val).ToCharArray();
            return val_q300p;
        }

        public static void SetGpioModels(SuperIO gpio, string[] arr)
        {
            string gpio_m = string.Join("", arr);
            byte data = Convert.ToByte(gpio_m, 2);
            gpio.SetGpioFunction(0xcf, data);
        }

        public static void SetGpioValues(SuperIO gpio, string[] arr)
        {
            string gpio_v = string.Join("", arr);
            byte data = Convert.ToByte(gpio_v, 2);
            gpio.SetGpioFunction(0xa07, data);
        }

    }
}
