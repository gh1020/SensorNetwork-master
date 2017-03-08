using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class FileOperationPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.FILEOPT; } }

        public FileOperationType Operation { get; set; }
        public string File { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Operation =(FileOperationType) br.ReadByte();
            File = br.ReadStingWithFixLength(32);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write((byte)Operation);
            bw.WriteStingWithFixLength(File, 32);
            return true;
        }
    }

    public enum FileOperationType
    {
        Delete,
        Upload,
        Play
    }
}
