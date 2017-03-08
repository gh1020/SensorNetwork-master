using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.3.3	接口参数设置
    /// </summary>
    public class IntfParam : BytesConverter
    {
        /// <summary>
        /// 主服务器IP地址
        /// </summary>
        public uint Master_Ip { get; set; }
        /// <summary>
        /// 主服务器侦听端口
        /// </summary>
        public ushort Master_Port { get; set; }
        /// <summary>
        /// 备用服务器IP地址
        /// </summary>
        public uint Slave_Ip { get; set; }
        /// <summary>
        /// 备用服务器侦听端口
        /// </summary>
        public ushort Slave_Port { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 通信类型
        /// </summary>
        public sbyte Comm_Type { get; set; }
        /// <summary>
        /// 心跳周期
        /// </summary>
        public sbyte Hb_Period { get; set; }
        /// <summary>
        /// 重发次数
        /// </summary>
        public sbyte Resend_Num { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public sbyte Timeout { get; set; }


        public override bool Parse(BinaryReader br)
        {
            Master_Ip = br.ReadUInt32();
            Master_Port = br.ReadUInt16();
            Slave_Ip = br.ReadUInt32();
            Slave_Port = br.ReadUInt16();
            Username = br.ReadStingWithFixLength(32);
            Password = br.ReadStingWithFixLength(16);
            Comm_Type = br.ReadSByte();
            Hb_Period = br.ReadSByte();
            Resend_Num = br.ReadSByte();
            Timeout = br.ReadSByte();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write(Master_Ip);
            bw.Write(Master_Port);
            bw.Write(Slave_Ip);
            bw.Write(Slave_Port);
            bw.WriteStingWithFixLength(Username, 32);
            bw.WriteStingWithFixLength(Password, 16);
            bw.Write(Comm_Type);
            bw.Write(Hb_Period);
            bw.Write(Resend_Num);
            bw.Write(Timeout);
            return true;
        }
    }

}
