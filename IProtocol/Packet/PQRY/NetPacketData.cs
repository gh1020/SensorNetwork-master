using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PQRY
{
    /// <summary>
    /// 5.3.2	网络参数设置
    /// </summary>
    public class NetPacketData : BasePacketData<NetParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.NET; } }
    }
}
