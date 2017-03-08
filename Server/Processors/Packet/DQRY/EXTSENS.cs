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
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using io.nulldata.SensorNetwork.Data.Models;

namespace SensorNetwork.Server.Processors.Packet.DQRY
{
    public class EXTSENS : PacketProcessorBase, IPacketProcessor
    {
        private HttpClient client = new HttpClient();
        public ushort Serv_Code { get { return (ushort)ServiceCode.EXTSENS; } }

        public ServiceType Serv_Type { get { return ServiceType.DQRY; } }

        public async Task<bool> DoProcess(AsyncSocketConnection agent, TPKGHead msg)
        {
            var packet = msg.Data as EXTSENSPacketData;
            if (packet == null)
            {
                return await SendCommonResponseMessageAsync(agent, msg, 1);//给网关回复1，说明不正常
            }
            log.Info($"(EXTSENS)收到设备状态包(16进制)：{ packet.ToBytes().ToHexString()}");
            log.Info($"(EXTSENS)收到设备状态包：{ packet.ToJson()}");
            Console.WriteLine($"(EXTSENS)收到设备状态包：{ packet.ToJson()}");
            //TODO: 
            var gatewayid = agent.TerminalId;

            string strUID = packet.Equip_UID;////外接蓝牙设备UID
            log.Info($"外接蓝牙设备数据包UID={ strUID}");
            string strUIDPrefix16 = "";//UID前16位，一共24位，去掉后面的8位是序号，前16号相同的协议也相同
            if (strUID.Length > 15)
            {
                strUIDPrefix16 = strUID.Substring(0, 16); //根据此16位去库中查去获取数据位置.SortByDescending(o => o.Created)
                //var query = db.GetCollection<ProtocolResolveRule>().Where(o => o.UIDPrefix == strUIDPrefix16).DoQuery().FirstOrDefault();
                //var query = db.GetCollection<SendStructure>().Where(o => o.UID_16 == strUIDPrefix16 && o.IsDataArea == "1").DoQuery().ToList();//查询出数据部分协议

                int iLenData = packet.DataLength;//外接蓝牙设备协议总字节数量
                string strProtocol = packet.Data.ToUpper();//外接蓝牙设备协议 （蓝牙设备数据）
                                                           //if (query != null && !string.IsNullOrEmpty(strProtocol))

                #region 查询协议长度是否相等，若有多条长度相同，则判断命令字
                var query0 = db.GetCollection<SendStructure2>().Where(o => o.UID_16 == strUIDPrefix16).DoQuery().ToList();//查询出数据部分协议
                if (query0.Count() > 0 && !string.IsNullOrEmpty(strProtocol))//有协议
                {
                    List<String> listItem = new List<string>();
                    foreach (var item in query0)
                    {
                        string StructName = item.StructName;
                        if (!listItem.Contains(StructName))
                        {
                            listItem.Add(StructName);
                        }
                    }
                    if (listItem.Count==1)//只有一条协议
                    {
                        //取协议解析数据
                        DecodeData(strUIDPrefix16, listItem[0], gatewayid, strUID, strProtocol, agent, msg);

                    }
                    else
                    {
                        for (int i = 0; i < listItem.Count; i++)
                        {
                            var query1 = db.GetCollection<SendStructure2>().Where(o => o.UID_16 == strUIDPrefix16 && o.StructName == listItem[i]).DoQuery().FirstOrDefault();//查询出数据部分协议
                            if (query1!=null && Convert.ToInt32(query1.Length) == iLenData)//长度相等，则取命令字
                            {
                                var query2 = db.GetCollection<SendStructure2>().Where(o => o.UID_16 == strUIDPrefix16 && o.StructName == listItem[i] && o.DataAreaDec == "2").DoQuery().ToList();//查询出数据部分协议
                                if (query2 != null && query2.Count() > 0)
                                {
                                    int count = 0;
                                    foreach (var q in query2)//一条协议中会出现多个命令字的情况
                                    {
                                        //判断命令字
                                        int OderiPos1 = Convert.ToInt32(q.LocationBegin + "");
                                        int OderiPos2 = Convert.ToInt32(q.LocationEnd + "");
                                        string Oder;
                                        if (q.LocationEnd == -1)
                                        {
                                            Oder = strProtocol.Substring((OderiPos1 - 1) * 2, 2);//得到16进制值，数据段的值
                                        }
                                        else
                                        {
                                            Oder = strProtocol.Substring((OderiPos1 - 1) * 2, 2 * (OderiPos2 - OderiPos1) + 2);//得到16进制值，数据段的值
                                        }
                                        string OrderCode = q.OrderCode;
                                        OrderCode = OrderCode.Replace(" ", "");//去掉空格
                                        if (Oder == OrderCode)//命令字相同
                                        {
                                            count++;
                                        }
                                    }
                                    if (query2.Count() == count)//所有命令字相同
                                    {
                                        DecodeData(strUIDPrefix16, listItem[i], gatewayid, strUID, strProtocol, agent, msg);
                                        break;
                                    }

                                }
                                else//无命令字，长度相同（只允许一个相等，需要做判断）
                                {
                                    DecodeData(strUIDPrefix16, listItem[i], gatewayid, strUID, strProtocol, agent, msg);
                                    break;
                                }
                            }

                        }
                    }

                }//end   if (!string.IsNullOrEmpty(strProtocol))
                else//无协议
                {
                    //未找到解析规则，直接将数据包转发 
                    JObject json = new JObject(
                       new JProperty("Success", true),
                       new JProperty("Type", "02"),
                       new JProperty("GatewayID", gatewayid),
                       new JProperty("UID", strUID),
                       new JProperty("Result", strProtocol),
                       new JProperty("TestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    );
                    //第二种构造Json方法
                    dynamic json2 = new System.Dynamic.ExpandoObject();
                    json2.Success = true;
                    json2.Type = "02";
                    json2.GatewayID = gatewayid;
                    json2.UID = strUID;
                    json2.Result = strProtocol;
                    json2.TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string s2 = Newtonsoft.Json.JsonConvert.SerializeObject(json2);//使用第二种Json构造方式，存入数据库
                    //此处要加发送和保存到数据库的代码
                    WaitCallback ac = async (xx) =>
                    {
                        //发送数据
                        DataSending send = new DataSending();
                        await send.SendingData(agent, msg, gatewayid, json);

                        //var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostBluetoothPacket", json);
                        //log.Info($"发送到应用云平台的蓝牙设备JSON数据：{ json}");

                        //以下部分要重新写代码入数据库
                        var d = new BluetoothSensorLog()
                        {
                            GatewayID = agent.TerminalId,
                            UID = strUID,
                            Json = s2,
                            Created = DateTime.Now,
                        };
                        await db.AddAsync(d);
                    };//end    WaitCallback ac = async (xx) =>
                    System.Threading.ThreadPool.QueueUserWorkItem(ac, null);


                    log.Info($"根据UID前16位从tProtocolResolveRule中没有查询到解析规则或者外接蓝牙设备协议为空。UID={ strUID},JSON={json}");
                }

                    #endregion



                 
            }
            else {
                log.Info($"外接蓝牙设备数据包UID长度不足16位。UID={ strUID}");
                //设备UID错误
                JObject json = new JObject(
                   new JProperty("Success", false),
                   new JProperty("Result", "错误"),
                   new JProperty("TestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                );
                //第二种构造Json方法
                dynamic json2 = new System.Dynamic.ExpandoObject();
                json2.Success = false;
                json2.Result = "错误";
                json2.TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string s2 = Newtonsoft.Json.JsonConvert.SerializeObject(json2);//使用第二种Json构造方式，存入数据库
                //此处要加发送
                WaitCallback ac = async (xx) =>
                {
                    //发送数据
                    DataSending send = new DataSending();
                    await send.SendingData(agent, msg, gatewayid, json);

                    //var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostBluetoothPacket", json);
                    //var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostBluetoothPacket", json);//联桥网云应用云平台
                    //var result1 = await client.PostAsJsonAsync("http://202.91.226.221:8088/api/Sensor/PostSenorData", json);//杭州大数据平台
                    //var result2 = await client.PostAsJsonAsync("http://202.91.226.221/api/Sensor/PostSenorData", json);//杭州大数据平台
                    //log.Info($"发送到应用云平台的蓝牙设备JSON数据：{ json}");
                    //以下部分要重新写代码入数据库
                    var d = new BluetoothSensorLog()
                    {
                        GatewayID = agent.TerminalId,
                        UID = strUID,
                        Json = s2,
                        Created = DateTime.Now,
                    };
                    await db.AddAsync(d);
                };//end    WaitCallback ac = async (xx) =>
                System.Threading.ThreadPool.QueueUserWorkItem(ac, null);
            }
            return await SendCommonResponseMessageAsync(agent, msg, 0);//给网关回复0，说明正常

        }


        public void DecodeData(string UID_16,string StructName,string GatewayID,string UID,string strProtocol, AsyncSocketConnection agent, TPKGHead msg)
        {
            //取协议解析数据
            #region
            var query2 = db.GetCollection<SendStructure2>().Where(o => o.UID_16 == UID_16 && o.StructName == StructName && o.DataAreaDec == "1").DoQuery().ToList();//查询出数据部分协议
                                                                                                                                                                             // to do 需要根据gatewayid去数据库里查找其对应的应用云平台接口地址
            WaitCallback ac = async (xx) =>
            {
                //BluetoothDeviceData obj = null; 
                JObject json = new JObject();
                dynamic json2 = new System.Dynamic.ExpandoObject();//动态构建Json数据包，该方法暂定为不可用。
                try
                {
                    json.Add(new JProperty("Success", true));
                    json.Add(new JProperty("Code", 1));
                    json.Add(new JProperty("Type", "01"));
                    json.Add(new JProperty("GatewayID", GatewayID));
                    json.Add(new JProperty("UID", UID));

                    //第二种方法，动态构建Json数据包，暂时未将其发到应用云平台，只用于存储到数据库中
                    json2.Success = true;
                    json2.Code = 1;
                    json2.Type = "01";
                    json2.GatewayID = GatewayID;
                    json2.UID = UID;
                    //var s = Newtonsoft.Json.JsonConvert.SerializeObject(json2);


                    JArray array = new JArray();
                    foreach (var q in query2)
                    {
                        //取出所有的数据域部分，进行数据处理，等到testresult,testresultdesc
                        string strDataDesc = q.DataDesc;
                        string strFormula = q.Formula;

                        int iPos1 = Convert.ToInt32(q.LocationBegin + "");
                        int iPos2 = Convert.ToInt32(q.LocationEnd + "");
                        //log.Info($"获取位置：iPos1={ iPos1}，iPos2={ iPos2}");

                        double data;
                        string strVHex;
                        if (q.LocationEnd == -1)
                        {
                            strVHex = strProtocol.Substring((iPos1 - 1) * 2, 2);//得到16进制值，数据段的值
                        }
                        else
                        {
                            strVHex = strProtocol.Substring((iPos1 - 1) * 2, 2 * (iPos2 - iPos1) + 2);//得到16进制值，数据段的值
                        }
                        if (!string.IsNullOrEmpty(q.Formula))//存在需要进一步计算转换的公式
                        {
                            int iTestResult1 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strVHex);
                            string strTemp1 = q.Formula.Replace("D", iTestResult1.ToString());
                            Microsoft.JScript.Vsa.VsaEngine ve = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();//公式计算
                            var Result1 = Microsoft.JScript.Eval.JScriptEvaluate(strTemp1, ve);
                            data = Convert.ToDouble(Result1);
                            //log.Info($"取数：data={ data}");
                        }
                        else
                        {
                            data = SensorNetwork.Common.RadixConverter.HexStrToInt32(strVHex) / 1.0;
                            //log.Info($"取数：data={ data}");
                        }
                        //log.Info($"数组中加入数据：TestResult={ data}，TestResultDesc={q.DataDesc}");
                        JObject jo = new JObject();
                        jo.Add(new JProperty("TestResult", data));
                        jo.Add(new JProperty("TestResultDesc", q.DataDesc)); //
                        array.Add(jo);
                    }
                    //array.Add( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    json.Add("Result", array);
                    json.Add(new JProperty("TestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                    json2.Result = array;
                    json2.TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string s = Newtonsoft.Json.JsonConvert.SerializeObject(json2);

                    //发送数据
                    DataSending send = new DataSending();
                    await send.SendingData(agent, msg, GatewayID, json);

                }
                catch (Exception ex)
                {
                    log.Info($"处理蓝牙设备数据包发生异常：{ ex.Message}");
                    //设备UID错误
                    JObject jsonError = new JObject(
                       new JProperty("Success", false),
                       new JProperty("Result", "错误"),
                       new JProperty("TestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    );
                    //此处要加发送
                    //发送数据
                    DataSending send = new DataSending();
                    await send.SendingData(agent, msg, GatewayID, jsonError);

                }
                //log.Info($"蓝牙设备数据包：{ obj.ToJson()}");


                //                        log.Info($"发送到应用云平台的蓝牙设备JSON数据：{ json}");
                //log.Info($"json.tostring：{ json.ToString()}");
                //以下部分要重新写代码入数据库
                string js = json.ToString();// json.ToString().Substring(0, 1002),//保存长度有问题，需要更改.Substring(0, 1002)
                string s2 = Newtonsoft.Json.JsonConvert.SerializeObject(json2);//使用第二种Json构造方式，存入数据库
                log.Info($"js：{ js.Length}");
                var d = new BluetoothSensorLog()
                {
                    GatewayID = agent.TerminalId,
                    UID = UID,
                    Json = s2,
                    Created = DateTime.Now,
                };
                await db.AddAsync(d);
                // }
            };//end    WaitCallback ac = async (xx) =>
            System.Threading.ThreadPool.QueueUserWorkItem(ac, null);

            #endregion

        }
    }

    /// <summary>
    /// 蓝牙设备检测结果
    /// </summary>
    public class BluetoothDeviceData
    {
        /// <summary>
        /// 设备UID
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// 检测结果1
        /// </summary>
        public double TestResult1 { get; set; }
        /// <summary>
        /// 检测结果1的描述
        /// </summary>
        public string TestResult1Desc { get; set; }
        /// <summary>
        /// 检测结果2
        /// </summary>
        public double TestResult2 { get; set; }
        /// <summary>
        /// 检测结果2的描述
        /// </summary>
        public string TestResult2Desc { get; set; }
        /// <summary>
        /// 检测结果3
        /// </summary>
        public double TestResult3 { get; set; }
        /// <summary>
        /// 检测结果3的描述
        /// </summary>
        public string TestResult3Desc { get; set; }
        /// <summary>
        /// 检测结果4
        /// </summary>
        public double TestResult4 { get; set; }
        /// <summary>
        /// 检测结果4的描述
        /// </summary>
        public string TestResult4Desc { get; set; }
        /// <summary>
        /// 检测结果5
        /// </summary>
        public double TestResult5 { get; set; }
        /// <summary>
        /// 检测结果5的描述
        /// </summary>
        public string TestResult5Desc { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        public string TestTime { get; set; }
    }
}
