using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.3.2	网络参数设置
    /// </summary>
    public class NetParam : BytesConverter
    {

        public NetworkConfig LAN { get; set; }

        public NetworkConfig WIFI { get; set; }

        public byte[] DNS1 { get; set; }
        public byte[] DNS2 { get; set; }

        public override bool Parse(BinaryReader br)
        {
            LAN = new NetworkConfig(br);
            WIFI = new NetworkConfig(br);
            DNS1 = br.ReadBytes(4);
            DNS2 = br.ReadBytes(4);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            (LAN ?? new NetworkConfig()).ToBytes(bw);
            (WIFI ?? new NetworkConfig()).ToBytes(bw);
            bw.WriteBytesWithFixLength(DNS1, 4);
            bw.WriteBytesWithFixLength(DNS2, 4);
            return true;
        }
    }

    public class NetworkConfig
    {
        public byte[] MAC_Addr { get; set; }
        public byte[] IP_Addr { get; set; }
        public byte[] IP_Mask { get; set; }
        public byte[] GW_Addr { get; set; }
        public byte AutoIP { get; set; }

        public NetworkConfig() { }

        public NetworkConfig(BinaryReader br)
        {
            Parse(br);
        }

        public bool Parse(BinaryReader br)
        {
            MAC_Addr = br.ReadBytes(6);
            IP_Addr = br.ReadBytes(4);
            IP_Mask = br.ReadBytes(4);
            GW_Addr = br.ReadBytes(4);
            AutoIP = br.ReadByte();
            return true;

        }


        public bool ToBytes(BinaryWriter bw)
        {
            bw.WriteBytesWithFixLength(MAC_Addr, 6);
            bw.WriteBytesWithFixLength(IP_Addr, 4);
            bw.WriteBytesWithFixLength(IP_Mask, 4);
            bw.WriteBytesWithFixLength(GW_Addr, 4);
            bw.Write(AutoIP);
            return true;
        }
    }
}
