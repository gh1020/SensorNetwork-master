using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace System
{
    /// <summary>
    /// 数据转换接口
    /// </summary>
    public interface IBytesConverter
    {
        /// <summary>
        /// 转换的数据
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();

        /// <summary>
        /// 解析数据 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Parse(byte[] data);

        /// <summary>
        /// 转换的数据
        /// </summary>
        /// <returns></returns>
        bool ToBytes(BinaryWriter bw);

        /// <summary>
        /// 解析数据 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Parse(BinaryReader br);
    }

    public abstract class BytesConverter : IBytesConverter
    {
        /// <summary>
        /// 转换数据
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bool b = ToBytes(bw);
                if (b)
                {
                    return ms.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return false;
            }
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                BinaryReader br = new BinaryReader(ms);
                return Parse(br);
            }
        }
        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="bw"></param>
        /// <returns></returns>
        public abstract bool ToBytes(BinaryWriter bw);

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public abstract bool Parse(BinaryReader br);
    }

    public static class IBytesConverterEx
    {
        /// <summary>
        /// 将字节数组写入到字节流中，包含字节长度标记
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="data"></param>
        public static void WriteStingWithFixLength(this BinaryWriter bw, string data, int len, byte fillByte = 0)
        {
            if (string.IsNullOrEmpty(data))
                bw.WriteBytesWithFixLength(null, len, fillByte);
            else
                bw.WriteBytesWithFixLength(Encoding.ASCII.GetBytes(data), len, fillByte);
        }

        /// <summary>
        /// 将字节数组写入到字节流中，包含字节长度标记
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="data"></param>
        public static void WriteBytesWithFixLength(this BinaryWriter bw, byte[] data, int len, byte fillByte = 0)
        {
            if (data == null || data.Length == 0)
                bw.Write(Enumerable.Range(0, len).Select(o => fillByte).ToArray());
            else if (data.Length > len)
                bw.Write(data, 0, len);
            else if (data.Length < len)
            {
                bw.Write(data);
                bw.Write(Enumerable.Range(0, len - data.Length).Select(o => fillByte).ToArray());
            }
            else
                bw.Write(data);
        }

        /// <summary>
        /// 将字节数组写入到字节流中，包含字节长度标记
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="data"></param>
        public static void WriteBytesWithLengthTag(this BinaryWriter bw, byte[] data)
        {
            bw.Write(data == null ? 0 : data.Length);
            if (data != null)
            {
                bw.Write(data);
            }
        }
        /// <summary>
        /// 将字节数组写入到字节流中，包含字节长度标记
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="data"></param>
        public static void WriteBytesWithLength16Tag(this BinaryWriter bw, byte[] data)
        {
            bw.Write(data == null ? 0 : (ushort)data.Length);
            if (data != null)
            {
                bw.Write(data);
            }
        }
        /// <summary>
        /// 读取字节流中指定长度的字节，不包括字节长度标记
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static byte[] ReadBytesWithLengthTag(this BinaryReader br)
        {
            return br.ReadBytes(br.ReadInt32());
        }
        /// <summary>
        /// 读取字节流中指定长度的字节，不包括字节长度标记
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static byte[] ReadBytesWithLength16Tag(this BinaryReader br)
        {
            return br.ReadBytes(br.ReadInt16());//br.ReadInt16() 执行完成会自动向前移动2个字节
        }

        public static string ReadStingWithFixLength(this BinaryReader br, int len)
        {
            var by = br.ReadBytes(len);
            return Encoding.ASCII.GetString(by).TrimEnd('\0');
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static int GetLength(this BinaryReader br)
        {
            return (int)(br.BaseStream.Length - br.BaseStream.Position);
        }
        /// <summary>
        /// 读取字节流，返回16进制串
        /// </summary>
        /// <param name="br"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ReadHexStingWithFixLength(this BinaryReader br, int len)
        {
            var by = br.ReadBytes(len);
            string returnStr = "";
            if (by != null)
            {
                for (int i = 0; i < by.Length; i++)
                {
                    returnStr += by[i].ToString("X2");
                }
            }
            return returnStr;
        }

        public static void Write(this BinaryWriter bw, DateTime time)
        {
            bw.Write(time.Ticks);
        }
        public static DateTime ReadDateTime(this BinaryReader br)
        {
            return new DateTime(br.ReadInt64());
        }

        public static void Write(this BinaryWriter bw, Guid guid)
        {
            bw.Write(guid.ToByteArray());
        }
        public static Guid ReadGuid(this BinaryReader br)
        {
            return new Guid(br.ReadBytes(16));
        }


    }
}
