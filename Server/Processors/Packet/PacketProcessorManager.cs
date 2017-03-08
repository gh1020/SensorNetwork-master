using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using IM.Shared;
using SensorNetwork.Protocol;
using IM.Sockets;
using SensorNetwork.Uart.Sockets;

namespace SensorNetwork.Server.Processors.Packet
{
    public class PacketProcessorManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<PacketTypeItem> processors;
        static PacketProcessorManager()
        {
            processors = TypeCache.GetSubtypes(typeof(IPacketProcessor))
                  .Select(o =>
                  {
                      try
                      {
                          return Activator.CreateInstance(o) as IPacketProcessor;
                      }
                      catch { }
                      return null;
                  }).Where(o => o != null)
                  .GroupBy(o => new { o.Serv_Type, o.Serv_Code })
                  .Select(o => new PacketTypeItem() { Serv_Code = o.Key.Serv_Code, Serv_Type = o.Key.Serv_Type, Processors = o.ToList() })
                .ToList();
        }
        public static async Task<bool> DoProcess(AsyncSocketConnection agent,TPKGHead message)
        {
            if (message == null)
                return false;
            int count = 0;
            //if (message.Data is ITerminalResponse)
            //{
            //    var data = message.Data as ITerminalResponse;
            //    Framework.Instance.TerminalCommandLogger.AddRepliedMessage(message, data.ResponsePacketIndex, (int)TerminalResponseResult.Success);
            //    count++;
            //    Framework.Instance.FireWaitHandle(message.SimId, data.ResponsePacketIndex, message.Data);
            //}
            //else if (message.Data is JT_8003)
            //{
            //    var data = message.Data as JT_8003;
            //    var r = Framework.Instance.FireWaitHandle(message.SimId, data.OriginalPacketIndex, data);
            //    if (r) //已被处理
            //        return true;
            //}

            var q = processors.Where(o => o.Serv_Code == message.Serv_Code && o.Serv_Type == message.Serv_Type).FirstOrDefault();

            if (q == null)
            {
                log.WarnFormat("Unhandled packet type: {0:X4}, {1:X4}", (int)message.Serv_Type, message.Serv_Code);
                return false;
            }

            foreach (var v in q.Processors)
                count += await v.DoProcess(agent, message) ? 1 : 0;
            return count > 0;
        }

        private class PacketTypeItem
        {
            public ServiceType Serv_Type { get; set; }
            /// <summary>
            /// 服务代码
            /// </summary>
            public ushort Serv_Code { get; set; }

            public List<IPacketProcessor> Processors { get; set; }

        }
    }
}
