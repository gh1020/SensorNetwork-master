using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol
{
    public interface INetworkTransmitter
    {
        /// <summary>
        /// 发送和等待回复
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        TPKGHead SendAndWaitTerminalResponse(TPKGHead data);
        /// <summary>
        /// 只发送
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool SendAsync(TPKGHead data);
        /// <summary>
        /// 判断是否在线
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool IsOnline(string uid);
    }
}
