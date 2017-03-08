using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.PQRY;
using SensorNetwork.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Net.Http;
using Newtonsoft.Json;

namespace SensorNetwork.Server.Processors.Packet.PQRY
{
    public class QRYONLINEBT : PacketProcessorBase, IPacketProcessor
    {
        private HttpClient client = new HttpClient();

        public ushort Serv_Code { get { return (ushort)ServiceCode.QRYONLINEBT; } }

        public ServiceType Serv_Type { get { return ServiceType.PQRY; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as QRYONLINEBTPacketData;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);
            }

            log.Info($"(QRYONLINEBT)5.4.8 已连接的蓝牙设备列表数据包：{ packet.ToJson()}");
            //to do 需要根据gatewayid去数据库里查找其对应的应用云平台接口地址
            //WaitCallback ac = async (xx) =>
            //{
            //    //1.通过http POST给应用云平台
            //    var json = JsonConvert.SerializeObject(packet);
            //    var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostGatewaySensorPacket", packet);
            //    //if (result.IsSuccessStatusCode && (await result.Content.ReadAsStringAsync()).Contains("'status':false,"))
            //    //{
            //    //    //2.通过ZMQ 发送给应用云平台 -
            //    //    SensorNetwork.Common.ZeroMQ.ZMQRequest("tcp://120.25.159.86:6789", JsonConvert.SerializeObject(packet));
            //    //}

            //    log.Info(packet.ToJson());
            //};
            //System.Threading.ThreadPool.QueueUserWorkItem(ac, null);



            //var d = new SensorStateLog()
            //{
            //    equipid = packet.Equip_Id,
            //    gatewayid = agent.TerminalId,
            //    data = packet.Data / 10.0,
            //    type = packet.Type,
            //    created = DateTime.Now,
            //};
            //await db.AddAsync(d);
            return await SendCommonResponseMessageAsync(agent, msg, 0);

        }
    }
}
