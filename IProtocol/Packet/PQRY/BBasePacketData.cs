using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PQRY
{
    public class BBasePacketData : BasePacketData<BaseParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BASE; } }

    }
}
