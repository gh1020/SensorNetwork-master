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
    /// 5.3.3	接口参数设置
    /// </summary>
    public class IntfPacketData : BasePacketData<IntfParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.INTF; } }

    }

}
