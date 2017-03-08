using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.DQRY
{
    public class TimePacketData : BasePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.TIME; } }
        /// <summary>
        /// 字节数组	6	秒、分、时、日、月、年
        /// </summary>
        public DateTime Datetime { get; set; }


        public override bool Parse(BinaryReader br)
        {
            var t = br.ReadBytes(6);
            Datetime = new DateTime(t[5], t[4], t[3], t[2], t[1], t[0]);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            var t = new byte[] {
                (byte) Datetime.Second,
                (byte) Datetime.Minute,
                (byte) Datetime.Hour,
                (byte) Datetime.Day,
                (byte) Datetime.Month,
                (byte) Datetime.Year
            };
            bw.Write(t);
            return true;
        }
    }
}
