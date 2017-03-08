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
    /// 5.4.7	可见未知蓝牙设备列表查询
    /// </summary>
    public class QRYONLINEBTPacketData : BasePacketData<QRYONLINEBTParam>, IResponsePacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.QRYONLINEBT; } }
        /// <summary>
        /// 连接的蓝牙设备数量
        /// </summary>
      //  public byte Num { get; set; }
        /// <summary>
        /// 设备UID和Name字典
        /// </summary>
       // public List<QRYONLINEBTPacketData> { get; set; }
    }

}
