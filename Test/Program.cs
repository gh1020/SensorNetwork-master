using SensorNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var s = "0xFF, 0xAA, 0x3E, 0x00, 0x3C, 0xBB, 0x7A, 0x59, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x32, 0x31, 0x34, 0x30, 0x39, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x04, 0x00, 0x01, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1E, 0x00, 0x32, 0x00, 0x01, 0x00, 0x00, 0x00";

            var s = "FF AA 50 00 AD 16 F0 60 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 33 04 00 09 00 26 00 00 00 00 00 4C 54 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC FD CE B3 56 FE 01 00 00 00";
            var arr = s.Replace("0x", "").Replace("  ", " ").Trim().Split(" ,".ToArray(),StringSplitOptions.RemoveEmptyEntries).Select(o => byte.Parse(o, NumberStyles.AllowHexSpecifier)).ToArray();
            var p = new TPKGHead();
           var b= p.Parse(arr);
            // string requestText = "{\"Serv_Code2\":1,\"Equip_Id\":\"12345678\",\"Data\":50,\"Type\":2,\"Serv_Type\":6,\"Serv_Code\":1}";

            //  SensorNetwork.Common.ZeroMQ.ZMQRequest("tcp://192.168.0.128:56789", requestText);
            // SensorNetwork.Common.ZeroMQ.ZMQRequest("tcp://202.103.209.2:6789", requestText);


            //传感器硬件设备数据主动上报
            //string strHex = "FF AA 53 00 08 3A 31 04 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 30 06 00 05 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 0E 11 5F C4 01 00 53 4E 06 00 04 03 00 17 24 00 00 00 00";
            string strHex = "FF AA 5B 00 99 66 5F 9F 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 30 06 00 05 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 0E 11 5F C4 11 00 53 4E 0E 00 04 04 0F 06 11 13 28 00 2E 00 01 41 E7 02 00 00 00";
            
            
            //login
         //  string strHex = "FF AA 6E 00 8F D7 1D EC 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 30 02 00 01 00 20 00 00 00 00 00 33 32 31 34 6D 79 6E 61 6D 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 6D 79 70 61 73 73 77 6F 72 64 00 00 00 00 00 00 6D 79 41 70 70 55 73 65 72 49 44 00 00 00 00 00 01 00 00 00";
            // string strHex = "FF AA 3D 00 08 9C 96 C9 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 30 06 00 01 00 20 00 00 00 00 00 31 32 33 34 35 36 37 38 00 00 00 00 00 00 00 00 1E 00 00 01 00 00 00";
            string strHex2 = strHex;
            strHex = strHex.Replace(" ", "");
            Console.WriteLine("Frame_Head:FFAA");//HexToStr(strHex.Substring(0,2*2))
            Console.WriteLine("Total_Len:" + HexToInt32(strHex.Substring(4, 2 * 2).TrimEnd('0')));
            Console.WriteLine("CRC32:" + HexToInt32(strHex.Substring(8, 4 * 2).TrimEnd('0')));
            Console.WriteLine("Seq_Id:" + HexToInt32(strHex.Substring(16, 4 * 2).TrimEnd('0')));
            Console.WriteLine("Comm_Type:" + HexToInt32(strHex.Substring(24, 1 * 2)));
            Console.WriteLine("Enc_Type:" + HexToInt32(strHex.Substring(26, 1 * 2)));
            Console.WriteLine("Major_Ver:" + HexToInt32(strHex.Substring(28, 1 * 2).TrimEnd('0')));
            Console.WriteLine("Minor_Ver:" + HexToInt32(strHex.Substring(30, 1 * 2).TrimEnd('0')));
            Console.WriteLine("Term_Code:" + HexToStr(strHex.Substring(32, 12 * 2)));
            Console.WriteLine("Serv_Type:" + HexToInt32(strHex.Substring(56, 2 * 2).TrimEnd('0')));
            Console.WriteLine("Serv_Code:" + HexToInt32(strHex.Substring(60, 2 * 2).TrimEnd('0')));
            Console.WriteLine("Flags:" + HexToInt32(strHex.Substring(64, 1 * 2)));
            Console.WriteLine("Priority:" + HexToInt32(strHex.Substring(66, 1 * 2)));
            Console.WriteLine("Reserved:" + HexToStr(strHex.Substring(68, 4 * 2)));

            //#region 用户登录
            //Console.WriteLine("Cloud_Code:" + HexToStr(strHex.Substring(76, 4 * 2)));
            //Console.WriteLine("Username:" + HexToStr(strHex.Substring(84, 32 * 2)));
            //Console.WriteLine("Password:" + HexToStr(strHex.Substring(148, 16 * 2)));
            //Console.WriteLine("AppUserID:" + HexToStr(strHex.Substring(180, 16 * 2)));
            //#endregion
            #region 蓝牙传感设备数据（主动上报）
            Console.WriteLine("Equip_UID:" + HexToStr(strHex.Substring(76, 24 * 2)));
            Console.WriteLine("Equip_MAC:" + strHex.Substring(124, 6 * 2));
            Console.WriteLine("Frame_Num:" + HexToInt32(strHex.Substring(136, 2 * 2).TrimEnd('0')));
            //   Console.WriteLine("Equ_Data:" + HexToStr(strHex.Substring(140, 9*2)));//53 4E 06 00 04 03 00 17 24
            #endregion


            //  string strHex = "48 65 6C 6C 6F 20 57 6F 72 6C 64 21";
            //string[] hexValuesSplit = strHex2.Split(' ');
            //foreach (String hex in hexValuesSplit)
            //{
            //    // Convert the number expressed in base-16 to an integer.
            //    int value = Convert.ToInt32(hex, 16);
            //    // Get the character corresponding to the integral value.
            //    string stringValue = Char.ConvertFromUtf32(value);
            //    char charValue = (char)value;
            //    Console.WriteLine("hexadecimal value = {0}, int value = {1}, char value = {2} or {3}", hex, value, stringValue, charValue);
            //}

            // Console.WriteLine(HexToStr(strHex));


            SocketSend("www.gosafenet.net",20000,strHex);


            Console.ReadLine();
        }
 
        //public static string GetHexStr(string strHex)
        //{
        //    byte Result  ;
        //    byte.TryParse(strHex, NumberStyles.HexNumber, null, out Result);
        //    return Result.ToString();
        //}

        public static string StrToHex(string mStr) //返回处理后的十六进制字符串
        {
            return BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");
        } /* StrToHex */
        public static string HexToStr(string mHex) // 返回十六进制代表的字符串
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
            //string result = ASCIIEncoding.Default.GetChars(arrResult).ToString();
          //  return result;

            //List<byte> buffer = new List<byte>();
            //for (int i = 0; i < mHex.Length; i += 2)
            //{
            //    string temp = mHex.Substring(i, 2);
            //    byte value = Convert.ToByte(temp, 16);
            //   // string stringValue = Char.ConvertFromUtf32(value);
            //    buffer.Add(value);
            //}
            //string str = System.Text.Encoding.ASCII.GetString(buffer.ToArray());//.TrimEnd('\0');

            //return str;

        } /* HexToStr */
        public static Int32 HexToInt32(string hex)
        {
            return Convert.ToInt32(hex==""?"0":hex, 16);
        }






        /// <summary>   
        /// 16进制字符串转字节数组   
        /// </summary>   
        /// <param name="hexString"></param>   
        /// <returns></returns>   
        private static byte[] HexStrToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }
        /// <summary>   
        /// 字节数组转16进制字符串   
        /// </summary>   
        /// <param name="bytes"></param>   
        /// <returns></returns>   
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static string SocketSend(string host, int port, string strHexData)
        {
            string result = string.Empty;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host, port);
            clientSocket.Send(HexStrToByte(strHexData));//字符串
            //  clientSocket.Send(encode.GetBytes(data));//字符串
            Console.WriteLine("Send：" + strHexData);
            result = SocketReceive(clientSocket, 5000 * 2); //5*2 seconds timeout.
            Console.WriteLine("Receive：" + result);
            DestroySocket(clientSocket);
            return result;
        }
        
        /// <summary>
           /// 接收数据
           /// </summary>
           /// <param name="socket"></param>
           /// <param name="timeout"></param>
           /// <returns></returns>
        private static string SocketReceive(Socket socket, int timeout)
        {
            string result = string.Empty;
            socket.ReceiveTimeout = timeout;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[1024];
            int length = 0;
            try
            {
                while ((length = socket.Receive(buffer)) > 0)
                {
                    for (int j = 0; j < length; j++)
                    {
                        data.Add(buffer[j]);
                    }
                    if (length < buffer.Length)
                    {
                        break;
                    }
                }
            }
            catch { }
            if (data.Count > 0)
            {
                result = Encoding.Default.GetString(data.ToArray(), 0, data.Count);
            }
            return result;
        }
        /// <summary>
        /// 销毁Socket对象
        /// </summary>
        /// <param name="socket"></param>
        private static void DestroySocket(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
    }
}
