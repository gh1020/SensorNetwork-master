using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class RestorePacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.RESTORE; } }


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
