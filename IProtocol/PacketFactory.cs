using IM.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol
{
    public static class PacketFactory
    {
        private static List<PacketTypeItem> types;
        static PacketFactory()
        {
            types = TypeCache.GetSubtypes(typeof(IPacketData))
                .Select(o =>
                {
                    try
                    {
                        return Activator.CreateInstance(o) as IPacketData;
                    }
                    catch { }
                    return null;
                }).Where(o => o != null)
                .GroupBy(o => new { o.Serv_Type, o.Serv_Code })
                .Select(o => new PacketTypeItem() { Serv_Code = o.Key.Serv_Code, Serv_Type = o.Key.Serv_Type, type = o.First().GetType() })
                .ToList();

        }
        public static IPacketData Create(BinaryReader br, ServiceType type, ushort code)
        {
            var t = types.Where(o => o.Serv_Type == type && o.Serv_Code == code)
                 .Select(o => o.type).FirstOrDefault();
            if (t == null)
                return null;
            var p = Activator.CreateInstance(t) as IPacketData;
            p.Parse(br);
            return p;
        }

        private class PacketTypeItem
        {
            public ServiceType Serv_Type { get; set; }
            /// <summary>
            /// 服务代码
            /// </summary>
            public ushort Serv_Code { get; set; }

            public Type type { get; set; }

        }
    }
}
