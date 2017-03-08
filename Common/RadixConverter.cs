using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Common
{
   public class RadixConverter
    {


        public static Int32 HexStrToInt32(string hexString)
        {
           // string hexString = "8E2";
            int num = Int32.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            // Console.WriteLine(num);
            return num;
        }

        public static float HexStrToFloat(string hexString)
        {
           // string hexString = "43480170";
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);

            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            //Console.WriteLine("float convert = {0}", f);
            return f;
        }

        public static string ByteToHexStr(byte[] vals)
        {
            //byte[] vals = { 0x01, 0xAA, 0xB1, 0xDC, 0x10, 0xDD };

            string str = BitConverter.ToString(vals);
            Console.WriteLine(str);

            str = BitConverter.ToString(vals).Replace("-", "");
            return str;
        }




        /// <summary>
        /// 16进制转字符串
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns></returns>
        public static string HexToStr(string mHex)
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] arrResult = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
            {
                string temp = mHex.Substring(i, 2);
                if (!byte.TryParse(temp, NumberStyles.HexNumber, null, out arrResult[i / 2]))
                {
                    arrResult[i / 2] = 0;
                }
            }
            return Encoding.ASCII.GetString(arrResult).TrimEnd('\0');
        }
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="mStr"></param>
        /// <returns></returns>
        public static string StrToHex(string mStr) //返回处理后的十六进制字符串
        {
            return BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");
        } /* StrToHex */



    }
}
