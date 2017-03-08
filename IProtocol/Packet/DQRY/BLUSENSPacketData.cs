using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class BLUSENSPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BLUSENS; } }
        public string Equip_Id { get; set; }
        public string Name { get; set; }
        public string MAC { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Equip_Id = br.ReadStingWithFixLength(16);
            MAC = br.ReadStingWithFixLength(6);
            Name = br.ReadStingWithFixLength(32);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Equip_Id, 16);
            bw.WriteStingWithFixLength(MAC, 6);
            bw.WriteStingWithFixLength(Name, 32);
            return true;
        }
    }
}
