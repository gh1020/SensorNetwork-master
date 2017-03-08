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
    /// 5.3.4	时钟参数设置
    /// </summary>
    public class NtpPacketData : BasePacketData<NtpParam>, IRequestPacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.NTP; } }
    }

}
