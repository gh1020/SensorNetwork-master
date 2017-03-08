using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using SensorNetwork.Uart.Sockets;
using SensorNetwork.Protocol;
using SensorNetwork.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using SensorNetwork.Protocol.Packet.PQRY;
using SensorNetwork.Server.Processors.Packet;

namespace SensorNetwork.Server.Processors.Packet.PQRY
{
    public abstract class IBase<TParam> : PacketProcessorBase, IPacketProcessor
        where TParam : IBytesConverter, new()
    {

        public abstract ushort Serv_Code { get; }

        public ServiceType Serv_Type { get { return ServiceType.PQRY; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as BasePacketData<TParam>;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);
            }

            log.Info($"收到设备参数包：{ packet.ToJson()}");

            db.GetCollection<Terminal>().UpdateOne(Builders<Terminal>.Filter.Where(o => o.deviceid == agent.TerminalId),
                Builders<Terminal>.Update.Set(((ServiceCode)Serv_Code).ToString(), packet.Param), new UpdateOptions() { IsUpsert = true });
            return await SendCommonResponseMessageAsync(agent, msg, 0);

        }
    }
}
