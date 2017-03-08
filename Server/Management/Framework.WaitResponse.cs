using IM.Shared.WaitEvent;
using SensorNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Server.Management
{
    partial class Framework
    {
        private WaitHandleCollection<TerminalResponseWaitItemKey, WaitItem<TerminalResponseWaitItemKey, TPKGHead>, TPKGHead> cache =
     new WaitHandleCollection<TerminalResponseWaitItemKey, WaitItem<TerminalResponseWaitItemKey, TPKGHead>, TPKGHead>();

        public bool FireWaitHandle(TPKGHead data)
        {
            if (data == null || cache.IsEmpty)
                return false;

            WaitItem<TerminalResponseWaitItemKey, TPKGHead> item;
            TerminalResponseWaitItemKey key = new TerminalResponseWaitItemKey()
            {
                TerminalId = data.Term_Code,
                SessionId = data.SessionId,
            };
            if (cache.TryGetWaitItem(key, out item) && item != null)
            {
                item.Result = data;
                item.Set();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 等待指定命令回复
        /// </summary>
        /// <param name="responseCommand">指定命令</param>
        /// <param name="timeout">最长等待时间</param>
        /// <returns></returns>
        public TPKGHead WaitTerminalResponse(string tid, uint? token, int timeout = 30000)
        {
            TerminalResponseWaitItemKey key = new TerminalResponseWaitItemKey()
            {
                TerminalId = tid,
                SessionId = token
            };

            var item = new WaitItem<TerminalResponseWaitItemKey, TPKGHead>(key, timeout);
            cache.AddWaitItem(key, item);
            item.WaitOne();
            cache.Remove(key);
            return item.Result;
        }


        class TerminalResponseWaitItemKey : IWaitItemKey<TerminalResponseWaitItemKey>
        {

            public string TerminalId { get; set; }

            public uint? SessionId { get; set; }


            public override int GetHashCode()
            {
                return (TerminalId ?? string.Empty).GetHashCode() ^ SessionId.GetHashCode();
            }

            public override bool Equals(TerminalResponseWaitItemKey obj)
            {
                if (obj == null)
                    return false;
                return TerminalId == obj.TerminalId && SessionId == obj.SessionId;
            }
        }


        public TPKGHead SendAndWaitTerminalResponse(TPKGHead data)
        {
            //log.Info($"向网关发送请求,Get其连接的蓝牙设备数据包1：{data.ToBytes().ToHexString()}");
            if (data == null)
                throw new Exception("数据不能等于空。");
            if (data.SessionId == null)
            {
                data.SetNextSessionId();
            }

            var c = Server.GetConnectionByTerminalId(data.Term_Code);
            if (c == null)
                throw new Exception("该设备不在线。");

            c.SendAsync(data.ToBytes());
            log.Info($"向网关发送请求,Get其连接的蓝牙设备数据包2：");
            log.Info($"{data.ToBytes().ToHexString()}");
            var resp = WaitTerminalResponse(data.Term_Code, data.SessionId);
            if (resp == null)
                throw new Exception("超时未响应。");
            return resp;

        }

        public bool SendAsync(TPKGHead data)
        {
            if (data == null)
                throw new Exception("数据不能等于空。");
            if (data.SessionId == null)
            {
                data.SetNextSessionId();
            }

            var c = Server.GetConnectionByTerminalId(data.Term_Code);
            if (c == null)
                throw new Exception("该设备不在线。");

            return c.SendAsync(data.ToBytes());
        }

        public bool IsOnline(string uid)
        {
            return Server.GetConnectionByTerminalId(uid) != null;
        }
    }
}
