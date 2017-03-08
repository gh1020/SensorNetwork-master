using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.DQRY;
using SensorNetwork.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace SensorNetwork.Server.Processors.Packet.DQRY
{
    public class REGBT : PacketProcessorBase, IPacketProcessor
    {

        public ushort Serv_Code { get { return (ushort)ServiceCode.SENS; } }

        public ServiceType Serv_Type { get { return ServiceType.DQRY; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as REGBTPacketData;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);
            }

            log.Info($"收到蓝牙设备（主动上报）注册包：{ packet.ToJson()}");

            var d = new SensorStateLog()
            {
                equipid = packet.Equip_Id,
                gatewayid = agent.TerminalId,
                humidity = packet.Humidity / 10.0,
                temperature = packet.Temperature / 10.0,
                created = DateTime.Now,
            };
            await db.AddAsync(d);
            return await SendCommonResponseMessageAsync(agent, msg, 0);

        }
    }
}
