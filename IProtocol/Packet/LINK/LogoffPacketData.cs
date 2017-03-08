using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.LINK
{
    public class LogoffPacketData : BasePacketData, IRequestPacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.LOGOFF; } }

        public override bool Parse(BinaryReader br)
        {
            return true;
        }


        public override bool ToBytes(BinaryWriter bw)
        {
            return true;
        }
    }
}
