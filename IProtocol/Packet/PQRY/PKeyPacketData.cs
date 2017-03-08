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
    /// 5.3.5	终端密码设置
    /// </summary>
    public class PKeyPacketData : BasePacketData<PKeyParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.NTP; } }
    }

}
