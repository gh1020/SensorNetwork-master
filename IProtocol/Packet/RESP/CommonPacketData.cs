using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.RESP
{
    public class CommonPacketData : BasePacketData, IRequestPacketData, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.RESP; } }

        public ServiceType Serv_Type_Resp { get; set; }

        public ushort Serv_Code_Resp { get; set; }



        public int ErrorCode { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Serv_Type_Resp = (ServiceType)br.ReadUInt16();
            Serv_Code_Resp = br.ReadUInt16();

            ErrorCode = br.ReadInt32();

            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write((ushort)this.Serv_Type_Resp);
            bw.Write((ushort)this.Serv_Code_Resp);
            bw.Write(ErrorCode);
            return true;
        }
    }
}
