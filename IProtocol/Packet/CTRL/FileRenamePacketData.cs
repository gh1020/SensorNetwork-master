using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class FileRenamePacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.FILERENAME; } }

        public string Old { get; set; }
        public string New { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Old = br.ReadStingWithFixLength(32);
            New = br.ReadStingWithFixLength(32);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Old, 32);
            bw.WriteStingWithFixLength(New, 32);
            return true;
        }
    }
}
