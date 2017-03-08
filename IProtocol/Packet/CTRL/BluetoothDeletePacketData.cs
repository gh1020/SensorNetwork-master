using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class BluetoothDeletePacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BLUDEL; } }

      //  public string Name { get; set; }
        public string UID { get; set; }//MAC 6->UID 24

        public override bool Parse(BinaryReader br)
        {
           // Name = br.ReadStingWithFixLength(32);
            UID = br.ReadStingWithFixLength(24);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
          //  bw.WriteStingWithFixLength(Name, 32);
            bw.WriteStingWithFixLength(UID, 24);
            return true;
        }
    }
}
