using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class EXTSENSPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.EXTSENS; } }
        /// <summary>
        /// 蓝牙设备的UID
        /// </summary>
        public string Equip_UID { get; set; }
        /// <summary>
        /// 蓝牙设备的MAC
        /// </summary>
        public string MAC { get; set; }
        public short DataLength { get; set; }
      //  public short FrameNum { get; set; }  //不需要这个字段，ReadBytesWithLength16Tag已经跳过2个字节了
                                             /// <summary>
                                             /// 蓝牙设备的协议数据包，网关直接打包上传的数据
                                             /// </summary>
        //public byte[] Data { get; set; }
        public string Data { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Equip_UID = br.ReadStingWithFixLength(24);
            MAC = br.ReadHexStingWithFixLength(6);   //br.ReadStingWithFixLength(6);
            DataLength = br.ReadInt16();//蓝牙设备数据包字节数量
            Data = br.ReadHexStingWithFixLength(DataLength);  //br.ReadBytesWithLength16Tag();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Equip_UID, 24);
            bw.WriteStingWithFixLength(MAC, 6);
           bw.Write(DataLength);
            bw.WriteStingWithFixLength(Data, DataLength);
            //bw.WriteBytesWithLength16Tag(Data);
            return true;
        }
    }
}
