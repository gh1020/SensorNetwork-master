using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SensorNetwork.Protocol
{
    public abstract class IPacketData : IBytesConverter
    {
        /// <summary>
        /// 服务类型
        /// </summary>
        public abstract ServiceType Serv_Type { get; }
        /// <summary>
        /// 服务代码
        /// </summary>
        public abstract ushort Serv_Code { get; }

        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    ToBytes(bw);
                    return ms.ToArray();
                }
            }
        }

        public bool Parse(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                ms.Position = 0;
                using (var br = new BinaryReader(ms))
                {
                    return Parse(br);
                }
            }
        }

        public abstract bool ToBytes(BinaryWriter bw);

        public abstract bool Parse(BinaryReader br);
    }

    /// <summary>
    /// 空数据
    /// </summary>
    public class EmptyPacketData : IPacketData
    {
        private ServiceType type;
        private ushort code;
        public override ushort Serv_Code { get { return code; } }

        public override ServiceType Serv_Type { get { return type; } }

        public EmptyPacketData(ServiceType type, ushort code)
        {
            this.type = type;
            this.code = code;
        }
        public static EmptyPacketData Create<TCode>(ServiceType type, TCode code)
            where TCode : struct
        {
            return new EmptyPacketData(type, Convert.ToUInt16(code));
        }

        public override bool Parse(BinaryReader br)
        {
            return true;
        }

        public override bool ToBytes(BinaryWriter bw)
        {
            return true;
        }
    }
}
