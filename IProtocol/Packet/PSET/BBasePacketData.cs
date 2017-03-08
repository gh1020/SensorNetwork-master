using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PSET
{
    public class BBasePacketData : BasePacketData<BaseParam>, IRequestPacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BASE; } }

    }
}
