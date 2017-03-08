using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class FileListResponsePacketData : BasePacketData, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.QRYFILELIST; } }

        public List<string> files;
        public override bool Parse(BinaryReader br)
        {
            files = new List<string>();
            while(br.BaseStream.CanRead)
            {
                files.Add(br.ReadStingWithFixLength(32));
            }
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            if (files?.Count > 0)
                files.ForEach(o => bw.WriteStingWithFixLength(o, 32));
            return true;
        }
    }
}
