using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SensorNetwork.Protocol;
using IM.Sockets;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol.Packet.RESP;
using SensorNetwork.Data;

namespace SensorNetwork.Server.Processors.Packet
{
    public abstract class PacketProcessorBase
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static ApplicationDbContext db = ApplicationDbContext.Default();
        public static TPKGHead CreateResponseMessage(TPKGHead msg, IPacketData data)
        {
            TPKGHead ts = new TPKGHead()
            {
                Data = data,
                Major_Ver = 1,
                Minor_Ver = 1,
                SessionId = msg.SessionId,
                Seq_Id = ++msg.Seq_Id,
                Term_Code = msg.Term_Code,
            };
            return ts;
        }

        public static TPKGHead CreateMessage(AsyncSocketConnection agent, IPacketData data)
        {
            TPKGHead ts = new TPKGHead()
            {
                Data = data,
                Major_Ver = 1,
                Minor_Ver = 1,
                Term_Code = agent.TerminalId,
            };
            return ts;
        }

        public static async Task<bool> SendCommonResponseMessageAsync(AsyncSocketConnection agent, TPKGHead msg, int error_code)
        {
            return await Task.FromResult(SendCommonResponseMessage(agent, msg, error_code));
        }
        public static bool SendCommonResponseMessage(AsyncSocketConnection agent, TPKGHead msg, int error_code)
        {
            var data = new CommonPacketData()
            {
                ErrorCode = error_code,
                Serv_Code_Resp = msg.Serv_Code,
                Serv_Type_Resp = msg.Serv_Type
            };

            TPKGHead ts = new TPKGHead()
            {
                Data = data,
                Major_Ver = 1,
                Minor_Ver = 1,
                Term_Code = agent.TerminalId,
            };
            return agent.SendAsync(ts);
        }
    }

}
