using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.3.6	传感器上传时间间隔设置
    /// </summary>
    public class IntervalParam : BytesConverter
    {
        /// <summary>
        /// 温度数据上传的时间间隔
        /// </summary>
        public uint TP_Interval { get; set; }

        /// <summary>
        /// 湿度数据上传的时间间隔
        /// </summary>
        public uint HM_Interval { get; set; }
        /// <summary>
        /// 空气数据上传的时间间隔
        /// </summary>
        public uint Air_Interval { get; set; }


        public override bool Parse(BinaryReader br)
        {
            TP_Interval = br.ReadUInt32();
            HM_Interval = br.ReadUInt32();
            Air_Interval = br.ReadUInt32();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write(TP_Interval);
            bw.Write(HM_Interval);
            bw.Write(Air_Interval);
            return true;
        }
    }

}
