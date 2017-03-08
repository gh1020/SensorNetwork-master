using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.3.5	终端密码设置
    /// </summary>
    public class PKeyParam : BytesConverter
    {
        /// <summary>
        /// 密码算法编号
        /// </summary>
        public sbyte Algorithm_id { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public byte[] Key { get; set; }
       

        public override bool Parse(BinaryReader br)
        {

            Algorithm_id = br.ReadSByte();
            Key = br.ReadBytes(16);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.Write(Algorithm_id);
            bw.WriteBytesWithFixLength(Key, 16);
            return true;
        }
    }

}
