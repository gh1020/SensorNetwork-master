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
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using io.nulldata.SensorNetwork.Data.Models;

namespace SensorNetwork.Server.Processors.Packet.DQRY
{
    public class Sens : PacketProcessorBase, IPacketProcessor
    {
        private HttpClient client = new HttpClient();

        public ushort Serv_Code { get { return (ushort)ServiceCode.SENS; } }

        public ServiceType Serv_Type { get { return ServiceType.DQRY; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as SensPacketData;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);
            }

            log.Info($"(Sens)收到设备状态包：{ packet.ToJson()}");
            //to do 需要根据gatewayid去数据库里查找其对应的应用云平台接口地址
            WaitCallback ac = async (xx) =>
            {
                //1.通过http POST给应用云平台
                //var json = JsonConvert.SerializeObject(packet);
                //var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostGatewaySensorPacket", packet);
                //构造Json数据包
                JObject json = new JObject();
                json.Add(new JProperty("Success", true));
                json.Add(new JProperty("Code", 1));
                json.Add(new JProperty("Type", "02"));//网关内部传感器数据
                json.Add(new JProperty("GatewayID", packet.Equip_Id));
                json.Add(new JProperty("UID", packet.Equip_Id));

                JArray array = new JArray();
                JObject jo = new JObject();
                jo.Add(new JProperty("TestResult", packet.Data));
                jo.Add(new JProperty("TestResultDesc", packet.Type)); //
                array.Add(jo);
                json.Add("Result", array);
                json.Add(new JProperty("TestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                //发送数据
                DataSending send = new DataSending();
                await send.SendingData(agent, msg, packet.Equip_Id, json);

  /*              var query = db.GetCollection<Gateway>().Where(o => o.GatewayID == packet.Equip_Id).DoQuery().FirstOrDefault();//根据网关ID查找应用云平台代码
                if (query != null)
                {
                    string ApplyCloudPlatformCode = query.ApplyCloudPlatformCode;
                    var query2 = db.GetCollection<ApplyCloudPlatform>().Where(o => o.PlatformCode == ApplyCloudPlatformCode).DoQuery().FirstOrDefault();//根据应用云平台代码查找数据发送地址
                    if (query2 != null)
                    {
                        string httpInterface = query2.HTTPInterface;
                        string[] sArray = httpInterface.Split(';');
                        foreach (string i in sArray)
                        {
                            log.Info($"数据发送地址：{ i.ToString()}");
                            var result = await client.PostAsJsonAsync(i.ToString(), packet);//发送数据

                        }
                    }

                }*/



                //if (result.IsSuccessStatusCode && (await result.Content.ReadAsStringAsync()).Contains("'status':false,"))
                //{
                //    //2.通过ZMQ 发送给应用云平台 -
                //    SensorNetwork.Common.ZeroMQ.ZMQRequest("tcp://120.25.159.86:6789", JsonConvert.SerializeObject(packet));
                //}

                log.Info(packet.ToJson());
            };
            System.Threading.ThreadPool.QueueUserWorkItem(ac, null);



            var d = new SensorStateLog()
            {
                equipid = packet.Equip_Id,
                gatewayid = agent.TerminalId,
                data = packet.Data / 10.0,
                type = packet.Type,
                created = DateTime.Now,
            };
            await db.AddAsync(d);
            return await SendCommonResponseMessageAsync(agent, msg, 0);

        }
    }
}
