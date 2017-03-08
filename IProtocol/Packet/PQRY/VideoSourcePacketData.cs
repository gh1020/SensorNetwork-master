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
    /// 5.4.6	视频源地址查询
    /// </summary>
    public class VideoSourcePacketData : BasePacketData<VideoSourceParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.VIDEOSOURCE; } }
    }

}
