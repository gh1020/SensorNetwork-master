using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.LINK;

namespace SensorNetwork.Server.Processors.Packet.TerminalManagement
{
    public class Logoff : PacketProcessorBase, IPacketProcessor
    {

        public ushort Serv_Code { get { return (ushort)ServiceCode.LOGOFF; } }

        public ServiceType Serv_Type { get { return ServiceType.LINK; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            //网关离线时候操作，写入日志，短信报警
            return true;
        }
    }
}
