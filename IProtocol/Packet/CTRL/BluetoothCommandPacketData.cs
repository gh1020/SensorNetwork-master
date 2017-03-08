using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class BluetoothCommandPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BLUCMD; } }

        /// <summary>
        /// 蓝牙设备UID
        /// </summary>
        public string UID { get; set; }//MAC 6->UID 24
        /// <summary>
        /// 给蓝牙设备发送的指令
        /// </summary>

        public byte[] CMD { get; set; }

        public override bool Parse(BinaryReader br)
        {
            UID = br.ReadStingWithFixLength(24);
            CMD = br.ReadBytesWithLengthTag();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(UID, 24);
            bw.WriteBytesWithLengthTag(CMD);
            return true;
        }
    }
}
