using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Collections;
using SensorNetwork.Uart.Sockets;
using System.Collections.Concurrent;
using System.Linq;


namespace SensorNetwork.Uart.Sockets.TCPImpl
{

    public class AsyncTcpServer : IM.Sockets.TCPImpl.AsyncTcpServerBase<AsyncSocketConnection, T808Unpacker, byte[]>, IServer
    {
        public List<string> GetTerminalIds()
        {
            return connections.Select(o => o.Value).Where(o => o.IsAuthenticated).Select(o => o.TerminalId).ToList();
        }

        public AsyncSocketConnection GetConnectionByTerminalId(string terminalId)
        {
            return connections.Select(o => o.Value).Where(o => o.TerminalId == terminalId).OrderByDescending(o => o.LastAlive).FirstOrDefault();
        }

        public List<AsyncSocketConnection> Connections()
        {
            return connections.Select(o => o.Value).ToList();
        }
    }
}
