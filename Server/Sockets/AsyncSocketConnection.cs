using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text; //for testing
using SensorNetwork.Uart.Sockets;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using SensorNetwork.Uart;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using log4net;
using SensorNetwork.Protocol;

namespace SensorNetwork.Uart.Sockets
{

    public class AsyncSocketConnection : IM.Sockets.AsyncSocketConnection<T808Unpacker, byte[]>
    {

        public string LicenseNumber { get; set; }
        public int? LicenseColor { get; set; }

        public bool IsAuthenticated { get; set; }

        public string TerminalId { get; set; }

        private static int _sid = 0;

        public void SetNewSessionId()
        {
            SessionId2 = (uint)Interlocked.Increment(ref _sid);
        }
        /// <summary>
        /// 回话id
        /// </summary>
        public uint? SessionId2 { get; set; }

        /// <summary>
        /// 处理协议
        /// </summary>
        public IProtocol Protocol { get; set; }


        public override void ResetState()
        {
            base.ResetState();
            TerminalId = null;
            LicenseNumber = "";
            LicenseColor = null;
            IsAuthenticated = false;
            Protocol = null;
            SessionId2 = null;
        }



        public bool SendAsync(TPKGHead msg)
        {
            if (SessionId2 > 0 && !(msg.SessionId > 0))
            {
                msg.SessionId = SessionId2;
                msg.Flags |= TPKGHeadFlags.SIDV;
            }
            var buffer = msg.ToBytes();
            return SendAsync(buffer);
        }


    }
}
