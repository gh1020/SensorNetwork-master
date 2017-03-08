using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Uart.Sockets.TCPImpl;
using System.Collections.Concurrent;
using System.Threading;
using IM.Sockets;
using SensorNetwork.Protocol;
using IM;
using SensorNetwork.Data.Models;

namespace SensorNetwork.Server.Management
{
    partial class Framework
    {
        private IServer socketServer;
        private List<Thread> threadProcess;
        /// <summary>
        /// 监听端口
        /// </summary>
        public int ListenPort { get; private set; }
        private ConcurrentQueue<Tuple<AsyncSocketConnection, TPKGHead>> ackQueue = new ConcurrentQueue<Tuple<AsyncSocketConnection, TPKGHead>>();
        private ConcurrentDictionary<string, Tuple<Guid, DateTime>> lastAlive = new ConcurrentDictionary<string, Tuple<Guid, DateTime>>();
        private ConcurrentQueue<TerminalConnectionLog> newConnections = new ConcurrentQueue<TerminalConnectionLog>();
        private ConcurrentDictionary<Guid, Tuple<string, DateTime>> closedConnections = new ConcurrentDictionary<Guid, Tuple<string, DateTime>>();


        public SensorNetwork.Uart.Sockets.IServer Server { get { return socketServer; } }

        private bool _isWorking = false;
        ServerSettings serverSettings;
        private bool StartListener()
        {
            ListenPort = ConfigurationManager.AppSettings.GetValue("ListenPort", 8899);
            var MaxConnections = ConfigurationManager.AppSettings.GetValue("MaxConnections", 11000);
            var PacketProcessThreadCount = ConfigurationManager.AppSettings.GetValue("PacketProcessThreadCount", -1);
            if (MaxConnections < 100)
                MaxConnections = 100;
            if (PacketProcessThreadCount <= 0 || PacketProcessThreadCount > Environment.ProcessorCount)
                PacketProcessThreadCount = Environment.ProcessorCount;
            StopListener();
            socketServer = new AsyncTcpServer();
            socketServer.OnDataReceived += socketServer_OnDataReceived;
            socketServer.OnSocketError += socketServer_OnSocketError;
            serverSettings = new ServerSettings() { Port = ListenPort, MaxConnections = MaxConnections };
            socketServer.Init(serverSettings);

            var ret = socketServer.Listen();

            if (ret)
            {
                _isWorking = true;
                log.InfoFormat("服务端口:{0}, 允许最大连接数: {1}, 包处理线程数: {2}", ListenPort, MaxConnections, PacketProcessThreadCount);
                if (threadProcess != null && threadProcess.Count > 0)
                {
                    foreach (var p in threadProcess)
                    {
                        try
                        {
                            p.Abort();
                        }
                        catch { }
                    }
                    threadProcess.Clear();
                }

                //启动应答处理线程
                var start = new ThreadStart(ProcessMessageThread);
                threadProcess = Enumerable.Range(0, PacketProcessThreadCount).Select(o => new Thread(start)).ToList();
                threadProcess.ForEach(o => { o.Priority = ThreadPriority.AboveNormal; o.Start(); });
            }
            else
                log.ErrorFormat("监听失败。");
            return ret;
        }


        /// <summary>
        /// 开始接收网关上传的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void socketServer_OnDataReceived(object sender, ReceivedEventArgs<byte[]> e)
        {
            AsyncSocketConnection conn = (AsyncSocketConnection)sender;
            TPKGHead packet = new TPKGHead();
            bool isOk = false;
            try
            {
                isOk = packet.Parse(e.Data);//解析收到的数据包
                log.Info($"SensorNetwork.Server.Management.Framework中接收的包，Serv_Type:" + packet.Serv_Type + ",Serv_Code:" + packet.Serv_Code + "包数据域Data：\r\n{" + packet.Data.ToBytes().ToHexString() + "}");
                if (isOk == false)
                    log.Error($"包解析错误：\r\n{BitConverter.ToString(e.Data).Replace("-", " ")}");
                else
                {
                    log.Info($"包解析成功(16进制)：\r\n{BitConverter.ToString(e.Data).Replace("-", " ")}");
                    log.Info($"包数据域：\r\n{packet.Data.ToString()}");
                }

                bool isNew = isOk && string.IsNullOrWhiteSpace(conn.TerminalId) && string.IsNullOrWhiteSpace(packet.Term_Code) == false;
                if (isNew)
                {
                    AddLog(packet.Term_Code, "设备已连接。");
                }
                if (string.IsNullOrWhiteSpace(packet.Term_Code) == false)
                {
                    conn.TerminalId = packet.Term_Code;
                    lastAlive[packet.Term_Code] = Tuple.Create(conn.Token, DateTime.Now);
                    //if (conn.Protocol == null)
                    //    conn.Protocol = ProtocolManager.Find(packet.Term_Code);
                    var s = Newtonsoft.Json.JsonConvert.SerializeObject(packet.Data);
                    AddLog(packet.Term_Code, packet.Data, "收到数据包：Data:{0}", packet.Data.ToString());
                }
                Interlocked.Increment(ref receivedMessageCount);
                conn.LastAlive = DateTime.Now;//更新连接的最后活动时间
                //else
                //{
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(packet.Data, Newtonsoft.Json.Formatting.Indented);
                //    log.DebugFormat("{0}\nMACID: {1}\n{2}", packet., packet.Id, json);
                //}


                if (isOk)//如果解析成功
                {
                    ackQueue.Enqueue(Tuple.Create(conn, packet));//将接收到的数据包放到队列中
                    if (isNew)//如果是一条新的连接，则创建一条新的连接写入DB中
                    {
                        newConnections.Enqueue(new TerminalConnectionLog()
                        {
                            id = conn.Token,//guid
                            connected = conn.Created,
                            ipaddress = conn.RemoteEndPoint.ToString(),
                            server_id = ServerId,
                            terminal_id = conn.TerminalId,
                            disconnected = conn.Created,
                        });
                        conn.Closed += AsyncSocketConnection_Closed;
                        log.DebugFormat("new client: {0}", conn.TerminalId);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"包解析/处理异常：\r\n{BitConverter.ToString(e.Data).Replace("-", " ")}");
                log.Error(ex);
            }




        }

        void AsyncSocketConnection_Closed(object sender, EventArgs e)
        {
            var conn = sender as AsyncSocketConnection;
            if (conn == null)
                return;
            conn.Closed -= AsyncSocketConnection_Closed;
            if (string.IsNullOrWhiteSpace(conn.TerminalId) || conn.Token == Guid.Empty)
                return;
            closedConnections[conn.Token] = Tuple.Create(conn.TerminalId, DateTime.Now);
            AddLog(conn.TerminalId, "设备已断开连接。");
        }

        void socketServer_OnSocketError(object sender, SocketErrorEventArgs e)
        {
            if (sender is AsyncSocketConnection)
            {
                var conn = sender as AsyncSocketConnection;
                log.ErrorFormat("通信错误:  tid:{0}, {1}, {2}", conn.TerminalId, e.Error.Message, e.Error.StackTrace);
            }
            else
            {
                SocketError se = (SocketError)e.ErrorCode;
                log.ErrorFormat("通信错误", se.GetDescription() + "," + e.Error.Message);
            }
        }


        public void StopListener()
        {
            _isWorking = false;
            Tuple<AsyncSocketConnection, TPKGHead> item;
            while (ackQueue.TryDequeue(out item)) { }

            if (socketServer != null)
            {
                try
                {
                    socketServer.Close();
                }
                catch (Exception ex)
                {
                    //DoLogEvent(this, LogLevel.Debug, ex);

                    log.Error(ex);
                }
                socketServer = null;
            }
        }



    }
}
