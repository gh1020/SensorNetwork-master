using  SensorNetwork.Uart.Sockets.TCPImpl;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace  SensorNetwork.Uart.Sockets
{
    public interface IServer : IM.Sockets.IServerBase<AsyncSocketConnection, T808Unpacker, byte[]>
    {
        AsyncSocketConnection GetConnectionByTerminalId(string terminalId);
        List<string> GetTerminalIds();

        List<AsyncSocketConnection> Connections();
    }
}
