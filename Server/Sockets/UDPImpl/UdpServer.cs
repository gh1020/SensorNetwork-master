using System;
using System.Net.Sockets;
using System.Net;
using  SensorNetwork.Uart.Sockets.UDPImpl;
using System.Threading;
using System.Collections.Generic;
using  SensorNetwork.Uart.Sockets;
using System.Collections;
using  SensorNetwork.Uart.Sockets.TCPImpl;
using System.Collections.Concurrent;
using System.Linq;
using log4net;

namespace  SensorNetwork.Uart.Sockets.UDPImpl
{
    public class UdpServer : IM.Sockets.TCPImpl.AsyncTcpServerBase<AsyncSocketConnection, T808Unpacker, byte[]>, IServer
    {
        public List<string> GetTerminalIds()
        {
            return connections.Select(o => o.Value).Where(o => o.IsAuthenticated).Select(o => o.TerminalId).ToList();
        }


        public List<AsyncSocketConnection> Connections()
        {
            return connections.Select(o => o.Value).ToList();
        }

        public AsyncSocketConnection GetConnectionByTerminalId(string terminalId)
        {
            return connections.Select(o => o.Value).Where(o => o.TerminalId == terminalId).OrderByDescending(o => o.LastAlive).FirstOrDefault();
        }

    }
}
