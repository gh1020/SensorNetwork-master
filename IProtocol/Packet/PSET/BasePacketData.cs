using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM;

namespace SensorNetwork.Protocol.Packet.PSET
{
    public abstract class BasePacketData<TParam> : IPacketData
        where TParam : IBytesConverter, new()
    {
        public override ServiceType Serv_Type { get { return ServiceType.PSET; } }
        public override ushort Serv_Code { get { return (ushort)Serv_Code2; } }

        public abstract ServiceCode Serv_Code2 { get; }


        public TParam Param { get; set; }




        public override bool Parse(BinaryReader br)
        {
            Param = new TParam();
            return Param.Parse(br);

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            if (Param == null)
                Param = new TParam();
            return Param.ToBytes(bw);
        }

        public override string ToString()
        {
            return $"ServiceType: {Serv_Type.GetDescription()}, ServiceCode:{Serv_Code2.GetDescription()}";
        }
    }
}
