using IM.Sockets;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Server.Processors.Packet
{
    public interface IPacketProcessor
    {
        /// <summary>
        /// 服务类型
        /// </summary>
        ServiceType Serv_Type { get; }
        /// <summary>
        /// 服务代码
        /// </summary>
        ushort Serv_Code { get; }

        Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg);
    }
}
