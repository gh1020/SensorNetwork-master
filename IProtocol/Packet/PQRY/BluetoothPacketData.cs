using SensorNetwork.Protocol.Packet;
using SensorNetwork.Protocol.Packet.Params;
using SensorNetwork.Protocol.Packet.PQRY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PQRY
{
    /// <summary>
    /// 5.4.7	蓝牙设备查询
    /// </summary>
    public class BluetoothPacketData : BasePacketData<BluetoothParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.QRYBT; } }
    }
}
