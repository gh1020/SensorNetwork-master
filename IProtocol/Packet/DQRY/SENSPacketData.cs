using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class SensPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.SENS; } }
        /// <summary>
        /// 网关ID
        /// </summary>
        public string Equip_Id { get; set; }
        public double Data { get; set; }
        /// <summary>
        /// 0：温度；1：湿度；2：空气质量 用于标示data是哪种传感器数据
        /// </summary>
        public byte Type { get; set; }

       // public ushort AirQuality { get; set; }


        public override bool Parse(BinaryReader br)
        {
            Equip_Id = br.ReadStingWithFixLength(12);
            Data = br.ReadInt16()/10.0;//三种类型数据均须除以10.0
            Type = br.ReadByte();
           // AirQuality = br.ReadUInt16();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Equip_Id, 12);
            bw.Write(Data);
            bw.Write(Type);
            return true;
        }
    }
}
