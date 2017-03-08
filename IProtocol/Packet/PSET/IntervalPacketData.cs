using SensorNetwork.Protocol.Packet.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PSET
{
    /// <summary>
    /// 5.3.6	传感器上传时间间隔设置
    /// </summary>
    public class IntervalData : BasePacketData<IntervalParam>, IRequestPacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.INTERVAL; } }
    }

}
