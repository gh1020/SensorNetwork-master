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
    /// 5.3.6	传感器上传时间间隔设置
    /// </summary>
    public class IntervalPacketData : BasePacketData<IntervalParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.INTERVAL; } }
    }

}
