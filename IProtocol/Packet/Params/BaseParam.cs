using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    public class BaseParam : BytesConverter
    { 
        public string Term_Code { get; set; }
        public string Term_Desc { get; set; }
        public string Addr_Desc { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Term_Code = br.ReadStingWithFixLength(12);
            Term_Desc = br.ReadStingWithFixLength(32);
            Addr_Desc = br.ReadStingWithFixLength(64);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Term_Code, 12);
            bw.WriteStingWithFixLength(Term_Desc, 32);
            bw.WriteStingWithFixLength(Addr_Desc, 64);
            return true;
        }
    }
}
