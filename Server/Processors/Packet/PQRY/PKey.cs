using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SensorNetwork.Protocol.Packet.PQRY;

namespace SensorNetwork.Server.Processors.Packet.PQRY
{
    public class PKey : IBase<PKeyParam>
    {
        public override ushort Serv_Code { get { return (ushort)ServiceCode.PKEY; } }
    }
}
