using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class ClearPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.DATACLR; } }

        public byte Data_type { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Data_type = br.ReadByte();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write(Data_type);
            return true;
        }
    }
}
