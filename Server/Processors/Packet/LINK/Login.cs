using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.LINK;
using SensorNetwork.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using io.nulldata.SensorNetwork.Data.Models;

namespace SensorNetwork.Server.Processors.Packet.TerminalManagement
{
    public class Login : PacketProcessorBase, IPacketProcessor
    {

        public ushort Serv_Code { get { return (ushort)ServiceCode.LOGIN; } }

        public ServiceType Serv_Type { get { return ServiceType.LINK; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as LoginPacketData;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);//给网关回复，0是正常，非0错误
            }
            var isOk = await db.GetCollection<CloudUser>()
                 .Where(o => o.cloud_code == packet.Cloud_Code && o.username == packet.Username &&
                 o.password == packet.Password).DoQuery().AnyAsync();

            if (isOk)
            {
                agent.SetNewSessionId();
                agent.IsAuthenticated = true;

                await db.GetCollection<Gateway>()
                       .Where(o => o.GatewayID == agent.TerminalId)
                       .UpdateOneAsync(Builders<Gateway>.Update
                       .Set(o => o.app_user_id, packet.AppUserID)
                       .Set(o => o.last_updated, DateTime.Now)
                       .Set(o => o.ip, agent.RemoteEndPoint.Address.ToString())
                       //.Set(o => o.mac, string.Join(":", msg.MAC_Addr.Select(o => o.ToString("X2"))))
                       .Set(o => o.port, agent.RemoteEndPoint.Port)
                       );

            }
            //else
            //{
            //    var u = new CloudUser()
            //    {
            //        cloud_code = packet.Cloud_Code,
            //        username = packet.Username,
            //        password = packet.Password
            //    };
            //    db.Add(u);
            //}
            log.Info($"网关登录：{ packet.ToJson()} , 结果：{isOk}");
            await SendCommonResponseMessageAsync(agent, msg, isOk ? 0 : 2);//回复登录是否正确，0：正确，2：登录验证错误

            var data2 = new SensorNetwork.Protocol.Packet.PQRY.BBasePacketData()
            {
                Param = new SensorNetwork.Protocol.Packet.Params.BaseParam()
                {
                    Addr_Desc = "地址",
                    Term_Code = msg.Term_Code,
                    Term_Desc = "测试设备",
                }
            };
            var ts = CreateResponseMessage(msg, data2);//给网关下发测试包
            return agent.SendAsync(ts);
        }
    }
}
