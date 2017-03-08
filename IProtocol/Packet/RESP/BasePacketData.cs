using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM;

namespace SensorNetwork.Protocol.Packet.RESP
{
    public abstract class BasePacketData : IPacketData
    {
        public override ServiceType Serv_Type { get { return ServiceType.RESP; } }
        public override ushort Serv_Code { get { return (ushort)Serv_Code2; } }

        public abstract ServiceCode Serv_Code2 { get; }

        public override string ToString()
        {
            return $"ServiceType: {Serv_Type.GetDescription()}, ServiceCode:{Serv_Code2.GetDescription()}";
        }
    }
}
