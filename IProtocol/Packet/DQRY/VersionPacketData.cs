using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class VersionPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.VER; } }

        public byte[] SysVer { get; set; }

        public override bool Parse(BinaryReader br)
        {
            SysVer = br.ReadBytes(24);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteBytesWithFixLength(SysVer, 24);
            return true;
        }
    }
}
