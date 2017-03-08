using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public class BluetoothConnectPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BLUCON; } }

      /// <summary>
      /// 蓝牙设备UID
      /// </summary>
        public string Equip_UID { get; set; }//MAC 6->UID 24
        /// <summary>
        /// 蓝牙设备MAC
        /// </summary>
        public string Equip_MAC { get; set; }
        /// <summary>
        /// PIN码长度
        /// </summary>

        public byte PinCode_Length { get; set; }
        /// <summary>
        /// Pin码
        /// </summary>

        public string PinCode{ get; set; }

        public override bool Parse(BinaryReader br)
        {
            Equip_UID = br.ReadStingWithFixLength(24);
            Equip_MAC = br.ReadStingWithFixLength(6);
            PinCode_Length = br.ReadByte();
            PinCode = br.ReadStingWithFixLength(8);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Equip_UID, 24);
            bw.WriteStingWithFixLength(Equip_MAC, 6);
            bw.Write(PinCode_Length);
            bw.WriteStingWithFixLength(Equip_MAC, 8);
            return true;
        }
    }
}
