using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.3.4	时钟参数设置
    /// </summary>
    public class NtpParam : BytesConverter
    {
        /// <summary>
        /// NTP服务器IP地址或域名
        /// </summary>
        public string NTP_Addr { get; set; }

        /// <summary>
        /// 通信类型
        /// </summary>
        public ushort NTP_Port { get; set; }
        /// <summary>
        /// 同步周期，单位：天
        /// </summary>
        public sbyte Sync_Period { get; set; }
        /// <summary>
        /// 使能标志
        /// </summary>
        public sbyte Enable_Flag { get; set; }
       


        public override bool Parse(BinaryReader br)
        {

            NTP_Addr = br.ReadStingWithFixLength(32);
            NTP_Port = br.ReadUInt16();
            Sync_Period = br.ReadSByte();
            Enable_Flag = br.ReadSByte();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {

            bw.WriteStingWithFixLength(NTP_Addr, 32);
            bw.Write(NTP_Port);
            bw.Write(Sync_Period);
            bw.Write(Enable_Flag);
            return true;
        }
    }

}
