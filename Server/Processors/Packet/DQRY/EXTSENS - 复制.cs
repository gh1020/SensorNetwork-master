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
            string strUIDPrefix16 = "";//UID前16位，一共24位，去掉后面的8位是序号，前16号相同的协议也相同
            if (strUID.Length > 15)
            {
                strUIDPrefix16 = strUID.Substring(0,16); //根据此16位去库中查去获取数据位置
            }
            int iLenData = packet.DataLength;//外接蓝牙设备协议总字节数量
            string strProtocol = packet.Data.ToUpper();//外接蓝牙设备协议 
            if (!string.IsNullOrEmpty(strProtocol))
            {           
                #region 将蓝牙设备数据传递给应用云平台          
                    //var gatewayid = msg.Term_Code; //两种方式
                    // to do 需要根据gatewayid去数据库里查找其对应的应用云平台接口地址
                    WaitCallback ac = async (xx) =>
                    {
                        BluetoothDeviceData obj = null;
                        #region  三诺血糖仪数据包 
                        //三诺蓝牙设备数据包：{ "UID" : "AATJ00017000310000000002", "TestResult1" : 5.9, "TestResult1Desc" : "BloodSugar", "TestResult2" : 20.2, "TestResult2Desc" : "Temperature", "TestResult3" : -1.0, "TestResult3Desc" : "", "TestResult4" : -1.0, "TestResult4Desc" : "", "TestResult5" : -1.0, "TestResult5Desc" : "", "TestTime" : "2016-10-25 09:45:49" }
                        // 53 4E 10 00 04 04 0E 04 16 10 37 01 01 0C 00 00 E1 00 76
                        // 53 4E 0E 00 04 04 0F 06 11 13 28 00 2E 00 01 41 E7  缺少了秒和校正码
                        if (strProtocol.Length == 34 && strProtocol.Substring(0, 4) == "534E")
                        {
                            //                    测试结果为26.8mmol / L，时间为14年4月22日16点55分；则发送的数据为：53 4E    10    00 04   04 0E 04 16 10 37  01 01 0C 00 00 E1 00 76
                            //其中“0X53  4E ”数据包头;“0X10” 数据长度，从本字节后开始计算;“0X00  04”     机器代码;  “0X04”         结果命令;   “0x0E”         14年;
                            //“0X04”         4月;“0X16”         22日;“0X10”         16点;“0X37”         55分;“0X01”         01秒;“0X01 0C”      表示234，此数据除以10即为23.4mmol / L;
                            //“0x00”          样本类型;“00 E1”        表示225，此数据除以10即为22.5℃;“0x00”          校正码;“0X76”         为校验码，和校验，包含长度,高字节丢掉;
                            if (strProtocol.Substring(10, 2) == "04")//命令字：ox04  读取当前结果
                            {
                                string strV1Hex = strProtocol.Substring(22, 4);
                                string strV2Hex = strProtocol.Substring(28, 4);
                                obj = new BluetoothDeviceData();
                                obj.UID = strUID;
                                obj.TestResult1 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strV1Hex) / 10.0;
                                obj.TestResult1Desc = "BloodSugar";
                                obj.TestResult2 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strV2Hex) / 10.0;
                                obj.TestResult2Desc = "Temperature";
                                obj.TestResult3 = obj.TestResult4 = obj.TestResult5 = -1;
                                obj.TestResult3Desc = obj.TestResult4Desc = obj.TestResult5Desc = "";
                                obj.TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                log.Info($"三诺蓝牙设备数据包：{ obj.ToJson()}");
                            }
                        }
                    #endregion
                    #region  三开血压仪数据包 
                  //  三开血压仪数据包：{ "UID" : "ABTJ00017000110000000001", "TestResult1" : 122.0, "TestResult1Desc" : "BloodPressureSystolic", "TestResult2" : 75.0, "TestResult2Desc" : "BloodPressureDiastolic", "TestResult3" : 95.0, "TestResult3Desc" : "Pulse", "TestResult4" : -1.0, "TestResult4Desc" : "", "TestResult5" : -1.0, "TestResult5Desc" : "", "TestTime" : "2016-10-26 10:19:55" }
            if (strProtocol.Length == 40 && strProtocol.Substring(0, 4) == "AA80")
                        {
                            //AA 80 02 08 01 05 00 00 00 00 37 00 39  //中间结果，实时数据
                            //最终结果AA 80 02 0f 01 06 01 12 09 13 12 01 20 00 76 00 45 00 3f 3c  ，00 76 00 45 00 3f中00 76、00 45、00 03f分别对应收缩压、舒张压、脉搏
                            //        AA 80 02 0F 01 06 01 12 09 18 17 2C 18 00 7A 00 4B 00 5F 45
                            if (strProtocol.Substring(6, 6) == "0F0106")
                            {
                                string strV1Hex = strProtocol.Substring(26, 4);
                                string strV2Hex = strProtocol.Substring(30, 4);
                                string strV3Hex = strProtocol.Substring(34, 4);
                                obj = new BluetoothDeviceData();
                                obj.UID = strUID;
                                obj.TestResult1 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strV1Hex) / 1.0;
                                obj.TestResult1Desc = "BloodPressureSystolic";//收缩压
                                obj.TestResult2 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strV2Hex) / 1.0;
                                obj.TestResult2Desc = "BloodPressureDiastolic";//舒张压
                                obj.TestResult3 = SensorNetwork.Common.RadixConverter.HexStrToInt32(strV3Hex) / 1.0;
                                obj.TestResult3Desc = "Pulse";//脉搏
                                obj.TestResult4 = obj.TestResult5 = -1;
                                obj.TestResult4Desc = obj.TestResult5Desc = "";
                                obj.TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                log.Info($"三开血压仪数据包：{ obj.ToJson()}");
                            }
                        }
                        #endregion

                        if (obj != null)
                        {
                            //1.通过http POST给应用云平台
                            var json = JsonConvert.SerializeObject(obj);
                            var result = await client.PostAsJsonAsync("http://120.25.159.86/api/Sensor/PostBluetoothPacket", obj);
                            //if (result.IsSuccessStatusCode && (await result.Content.ReadAsStringAsync()).Contains("'status':false,"))
                            //{
                            //    //2.通过ZMQ 发送给应用云平台 -
                            //    SensorNetwork.Common.ZeroMQ.ZMQRequest("tcp://120.25.159.86:6789", JsonConvert.SerializeObject(packet));
                            //}

                            log.Info($"发送到应用云平台的蓝牙设备JSON数据：{ obj.ToJson()}");

                            var d = new BluetoothSensorLog()
                            {
                                GatewayID = agent.TerminalId,
                                UID = obj.UID,
                                TestResult1=obj.TestResult1,
                                TestResult1Desc=obj.TestResult1Desc,
                                TestResult2=obj.TestResult2,
                                TestResult2Desc=obj.TestResult2Desc,
                                TestResult3 = obj.TestResult3,
                                TestResult3Desc = obj.TestResult3Desc,
                                TestResult4 = obj.TestResult4,
                                TestResult4Desc = obj.TestResult4Desc,
                                TestResult5 = obj.TestResult5,
                                TestResult5Desc = obj.TestResult5Desc,
                                Created = DateTime.Now,
                            };
                            await db.AddAsync(d);
                        }
                    };//end    WaitCallback ac = async (xx) =>
                System.Threading.ThreadPool.QueueUserWorkItem(ac, null);
               
                #endregion
            } //end   if (!string.IsNullOrEmpty(strProtocol))

            return await SendCommonResponseMessageAsync(agent, msg, 0);//给网关回复0，说明正常

        }
    }
    /// <summary>
    /// 蓝牙设备检测结果
    /// </summary>
    public class BluetoothDeviceData {
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
