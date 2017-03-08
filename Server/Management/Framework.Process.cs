using SensorNetwork.Uart.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IM.Shared;
using SensorNetwork.Protocol;
using SensorNetwork.Server.Processors.Packet;
using IM;

namespace SensorNetwork.Server.Management
{
    partial class Framework
    {
        private int processedMessageCount = 0;
        private int receivedMessageCount = 0;
        private DateTime lastRefreshTime = DateTime.Now;
        private ConcurrentDictionary<uint, int> packetCounts = new ConcurrentDictionary<uint, int>();
        private ConcurrentDictionary<uint, double> packetTime = new ConcurrentDictionary<uint, double>();
        private ConcurrentDictionary<uint, int> packetCounts30s = new ConcurrentDictionary<uint, int>();
        private ConcurrentDictionary<uint, double> packetTime30s = new ConcurrentDictionary<uint, double>();
        private void ProcessMessageThread()
        {
            ThreadProcessor.Auto();
            log.InfoFormat("开始对终端消息进行应答处理 {0}", Thread.CurrentThread.ManagedThreadId);
            Tuple<AsyncSocketConnection, TPKGHead> rd = null;
            HiPerfTimer timer = new HiPerfTimer();
            while (_isWorking)
            {
                if (ackQueue.TryDequeue(out rd) == false)//开始从对列中取接收到的数据进行处理
                {
                    Thread.Sleep(50);
                    continue;
                }
                try
                {
                    //服务端

                    timer.Start();
                    FireWaitHandle(rd.Item2); 
                    var rs = PacketProcessorManager.DoProcess(rd.Item1, rd.Item2).GetAwaiter().GetResult();
                    if (rd.Item2.Flags.HasFlag(TPKGHeadFlags.CON))
                    {
                        PacketProcessorBase.SendCommonResponseMessage(rd.Item1, rd.Item2, rs == true ? 0 : 1);
                    }
                    timer.Stop();
                    packetCounts.AddOrUpdate(rd.Item2.CommandId, 1, (o, v) => ++v);
                    packetTime.AddOrUpdate(rd.Item2.CommandId, timer.Duration, (o, v) => v + timer.Duration);
                    packetCounts30s.AddOrUpdate(rd.Item2.CommandId, 1, (o, v) => ++v);
                    packetTime30s.AddOrUpdate(rd.Item2.CommandId, timer.Duration, (o, v) => v + timer.Duration);

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                Interlocked.Increment(ref processedMessageCount);
            }

        }


        private void PeriodWorkThread(object state)
        {
            foreach (AsyncSocketConnection ic in socketServer.GetConnections())//获取服务上全部连接对象
            {
                var seconds = (DateTime.Now - ic.LastAlive).TotalSeconds;

                if (string.IsNullOrWhiteSpace(ic.TerminalId))//TerminalId :网关ID
                {
                    if (seconds > 60)
                    {
                        socketServer.CloseConnection(ic.SessionId);
                        log.Info("清理空连接。");
                    }
                }
                else if (seconds > 120)
                {
                    socketServer.CloseConnection(ic.SessionId);
                    log.InfoFormat("清理超期无响应终端： {0} - {1}", ic.TerminalId, ic.LicenseNumber);

                }
            }

            var c = socketServer.GetConnections().Count();//

            if (Environment.UserInteractive)
            {
                var p = Interlocked.Exchange(ref processedMessageCount, 0);
                var r = Interlocked.Exchange(ref receivedMessageCount, 0);
                var q = ackQueue.Count;
                var last = lastRefreshTime;
                lastRefreshTime = DateTime.Now;
                var i = (DateTime.Now - last).TotalSeconds;
                Console.Title = string.Format("Connection: {0}, Queue: {1}, Received: {2:0.00}/s, Processed: {3:0.00}/s", c, q, r / i, p / i);

                int count;
                double time;
                int count30s;
                double time30s;
                foreach (var k in packetTime30s.Select(o => o.Key))
                {
                    if (packetCounts.TryGetValue(k, out count) && packetTime.TryGetValue(k, out time) && packetCounts30s.TryRemove(k, out count30s) && packetTime30s.TryRemove(k, out time30s))
                    {
                        log.WarnFormat("\r\nPacketType:{0:X} Count: {1}, Time: {2:0.00}s, Speed: {3:0.00}/s, Last 30s: Count: {4}, Time: {5:0.00}s, Speed: {6:0.00}/s", k, count, time, count / time, count30s, time30s, count30s / time30s);
                    }
                }
            }

        }

    }
}
