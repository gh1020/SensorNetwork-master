using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SensorNetwork.Uart.Sockets;
using System.Linq.Expressions;
using MongoDB.Driver;
using SensorNetwork.Data.Models;

namespace SensorNetwork.Server.Management
{
    partial class Framework
    {

        private Thread threadUpdateTerminalState;
        private void StartUpdateTerminalStateThread()
        {
            if (threadUpdateTerminalState != null && threadUpdateTerminalState.IsAlive)
                return;
            threadUpdateTerminalState = new Thread(UpdateTerminalStateThread);
            threadUpdateTerminalState.Start();
        }

        public List<string> GetOnlineTerminalIds()
        {
            if (socketServer == null)
                return new List<string>();
            return socketServer.GetTerminalIds();
        }

        private void SetTerminalConnectionLogToDisconnected()
        {
            var dt = DateTime.Now;
            db.GetCollection<TerminalConnectionLog>().UpdateManyAsync(o => o.server_id == ServerId && o.disconnected == null && o.connected < dt, Builders<TerminalConnectionLog>.Update.Set(o => o.disconnected, dt));
        }

        private void UpdateTerminalStateThread()
        {
            var requests = new List<WriteModel<Terminal>>(1000);
            var requests2 = new List<WriteModel<TerminalConnectionLog>>(1000);
            var col = db.GetCollection<Terminal>();
            var connList = new List<TerminalConnectionLog>(1000);
            do
            {
                if (lastAlive.IsEmpty == false)
                {
                    foreach (var id in lastAlive.Select(o => o.Key).Take(1000))
                    {
                        Tuple<Guid, DateTime> dt;
                        if (lastAlive.TryRemove(id, out dt) == false)
                            continue;

                        {
                            Expression<Func<Terminal, bool>> filter = (o) => o.deviceid == id;
                            var u = Builders<Terminal>.Update
                                .Set(x => x.last_alive, dt.Item2)
                                .Set(o => o.online, true);

                            requests.Add(new UpdateOneModel<Terminal>(filter, u) { IsUpsert = true });
                        }

                        {
                            Expression<Func<TerminalConnectionLog, bool>> filter = (o) => o.id == dt.Item1 && o.disconnected < dt.Item2;
                            var u = Builders<TerminalConnectionLog>.Update.Set(x => x.disconnected, dt.Item2);
                            requests2.Add(new UpdateOneModel<TerminalConnectionLog>(filter, u));
                        }
                    }
                    col.BulkWriteUnorderedAsync(requests).GetAwaiter().GetResult();
                    db.GetCollection<TerminalConnectionLog>().BulkWriteUnorderedAsync(requests2).GetAwaiter().GetResult();
                    log.InfoFormat("Bulk Update Terminal State. count:{0}", requests.Count);
                    requests.Clear();
                    requests2.Clear();
                    Thread.Sleep(10);
                }

                while (newConnections.IsEmpty == false)
                {
                    newConnections.TryDequeue(1000, connList);
                    db.GetCollection<TerminalConnectionLog>().InsertManyAsync(connList);

                    foreach (var conn in connList)
                    {
                        Expression<Func<Terminal, bool>> filter = (o) => o.deviceid == conn.terminal_id;
                        var u = Builders<Terminal>.Update
                            .Set(x => x.last_ipaddress, conn.ipaddress)
                            .Set(o => o.last_connected, conn.connected)
                            .Set(o => o.online, true);
                        requests.Add(new UpdateOneModel<Terminal>(filter, u));
                    }
                    db.GetCollection<Terminal>().BulkWriteUnorderedAsync(requests).GetAwaiter().GetResult();
                    requests.Clear();

                    log.InfoFormat("Bulk Update Terminal New Connection Logs. count:{0}", connList.Count);
                }

                while (closedConnections.IsEmpty == false)//从关闭的连接中取出批量写入DB
                {
                    foreach (var id in closedConnections.Select(o => o.Key).Take(1000))
                    {
                        Tuple<string, DateTime> dt;
                        if (closedConnections.TryRemove(id, out dt) == false)
                            continue;
                        ////设置离线时间
                        //{
                        //    Expression<Func<TerminalConnectionLog, bool>> filter = (o) => o.Id == id;
                        //    var u = Builders<TerminalConnectionLog>.Update.Set(x => x.Disconnected, dt.Item2);
                        //    requests2.Add(new UpdateOneModel<TerminalConnectionLog>(filter, u));
                        //}
                        //设置为离线
                        //{
                        Expression<Func<Terminal, bool>> filter = (o) => o.deviceid == dt.Item1;
                        var u = Builders<Terminal>.Update
                            .Set(o => o.online, false);
                        requests.Add(new UpdateOneModel<Terminal>(filter, u));
                        //}
                    }

                    db.GetCollection<Terminal>().BulkWriteUnorderedAsync(requests).GetAwaiter().GetResult();
                    requests.Clear();
                    //db.TerminalConnectionLogs.BulkWriteUnorderedAsync(requests2).GetAwaiter().GetResult();
                    log.InfoFormat("Bulk Update Terminal Closed Connection Logs. count:{0}", requests2.Count);
                    //requests2.Clear();
                }
                if (lastAlive.IsEmpty && newConnections.IsEmpty && closedConnections.IsEmpty)
                    Thread.Sleep(1000);
            } while (true);
        }

    }

}
