using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.Tools
{
    class Utils
    {
        //把byte转化成2进制字符串 
        public static String ByteToBinaryStr(byte b)
        {
            String result = "";
            byte a = b;
            for (int i = 0; i < 8; i++)
            {
                result = (a % 2) + result;
                a = (byte)(a / 2);
            }
            return result;
        }

        //把二进制字符串转为16进制字符串
        public static String BinaryStrToHexStr(string bytestr)
        {
            string c = bytestr;
            try
            {
                c = Convert.ToUInt16(c, 2).ToString("x2");
            }
            catch (Exception)
            {
                c = "";
            }
            return c;
        }
    }
}
