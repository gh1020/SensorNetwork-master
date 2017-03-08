using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class BTPacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.BT; } }
        public string Equip_Id { get; set; }
        public string MAC { get; set; }
        /// <summary>
        /// 0：上线；:1：下线
        /// </summary>
        public byte Equip_Status { get; set; }

     //   public DateTime Time { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Equip_Id = br.ReadStingWithFixLength(24);
            MAC = br.ReadStingWithFixLength(6);
            Equip_Status = br.ReadByte();
            //var t = br.ReadBytes(6);
            //Time = new DateTime(t[5], t[4], t[3], t[2], t[1], t[0]);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Equip_Id, 24);
            bw.WriteStingWithFixLength(MAC, 6);
            bw.Write(Equip_Status);
            //var t = new byte[] {
            //    (byte) Time.Second,
            //    (byte) Time.Minute,
            //    (byte) Time.Hour,
            //    (byte) Time.Day,
            //    (byte) Time.Month,
            //    (byte) Time.Year
            //};
            //bw.Write(t);
            return true;
        }
    }
}
