using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.4.6	视频源地址查询
    /// </summary>
    public class VideoSourceParam : BytesConverter
    {
        /// <summary>
        /// IP地址 4
        /// </summary>
        public byte[] Source_IP { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public ushort Source_Port { get; set; }
      
        public override bool Parse(BinaryReader br)
        {

            Source_IP = br.ReadBytes(4);
            Source_Port = br.ReadUInt16();
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {

            bw.WriteBytesWithFixLength(Source_IP, 4);
            bw.Write(Source_Port);
            return true;
        }
    }

}
