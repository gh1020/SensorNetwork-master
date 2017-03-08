using IM.Sockets;
using io.nulldata.SensorNetwork.Data.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SensorNetwork.Data.Models;
using SensorNetwork.Protocol;
using SensorNetwork.Uart.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Server.Processors.Packet.DQRY
{
    class DataSending : PacketProcessorBase
    {
        private HttpClient client = new HttpClient();
        public async Task<bool> SendingData(AsyncSocketConnection agent, TPKGHead msg,string gatewayid, JObject json)
        {
            var query = db.GetCollection<Gateway>().Where(o => o.GatewayID == gatewayid).DoQuery().FirstOrDefault();//根据网关ID查找应用云平台代码
            if (query != null)
            {
                string ApplyCloudPlatformCode = query.ApplyCloudPlatformCode;
                var query2 = db.GetCollection<ApplyCloudPlatform>().Where(o => o.PlatformCode == ApplyCloudPlatformCode).DoQuery().FirstOrDefault();//根据应用云平台代码查找数据发送地址
                if (query2 != null)
                {
                    string httpInterface = query2.HTTPInterface;
                    string[] sArray = httpInterface.Split(';');
                    foreach(string i in sArray)
                    {
                        log.Info($"数据发送地址：{ i.ToString()}");
                        var result = await client.PostAsJsonAsync(i.ToString(), json);//发送数据

                    }
                }

            }
            return await SendCommonResponseMessageAsync(agent, msg, 0);//给网关回复0，说明正常

        }

    }
}
