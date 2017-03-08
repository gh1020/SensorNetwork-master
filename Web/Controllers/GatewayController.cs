using SensorNetwork.Server.Management;
using IM;
using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.Params;
using SensorNetwork.Protocol.Packet.PQRY;
using SensorNetwork.Protocol.Packet.RESP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web.Models.Gateway;
using SensorNetwork.Protocol.Packet;
using SensorNetwork.Data;
using io.nulldata.SensorNetwork.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using log4net;
using Web.Models;

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

namespace Web.Controllers
{
    [RoutePrefix("api/gateway")]
    public class GatewayController : ApiController
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ApplicationDbContext db = ApplicationDbContext.Default();
        /*
        /// <summary>
        /// 查询在线网关列表
        /// </summary>
        /// <returns></returns>
        [Route(""), HttpGet]
        public ApiResult<List<GatewayItem>> Get()
        {
            return Framework.Instance.Server.Connections()
                   .Select(o => new GatewayItem()
                   {
                       ipaddress = o.RemoteEndPoint.Address.ToString(),
                       session_id = o.SessionId,
                       id = o.TerminalId,
                       token = o.Token,
                       last_alive = o.LastAlive,
                       is_valid = o.IsValid(),
                       is_authenticated = o.IsAuthenticated,
                   })
                   .ToList();


        }*/


            //视频、音频接口



        /// <summary>
        /// 查询在线网关列表
        /// </summary>
        /// <param name="authCode">权限码，可为应用云平台代码</param>
        /// <returns></returns>
        [Route(""), HttpGet]
        public ApiResult<List<GatewayItem>> Get(string authCode)
        {
            if (string.IsNullOrWhiteSpace(authCode))
                return "authCode不正确";

            if (authCode == "LQ82858885")
            {
                return Framework.Instance.Server.Connections()
                   .Select(o => new GatewayItem()
                   {
                       ipaddress = o.RemoteEndPoint.Address.ToString(),
                       session_id = o.SessionId,
                       id = o.TerminalId,
                       token = o.Token,
                       last_alive = o.LastAlive,
                       is_valid = o.IsValid(),
                       is_authenticated = o.IsAuthenticated,
                   }).ToList();
            }
            else
            {                
                var query = db.GetCollection<Gateway>().Where(o => o.ApplyCloudPlatformCode == authCode ).DoQuery().ToList();

                List<string> li = new List<string>();
                foreach (var q in query)
                {
                    li.Add(q.GatewayID);
                }
                var list = Framework.Instance.Server.Connections()
                    .Where(o => li.Contains(o.TerminalId))
                   .Select(o => new GatewayItem()
                   {
                       ipaddress = o.RemoteEndPoint.Address.ToString(),
                       session_id = o.SessionId,
                       id = o.TerminalId,
                       token = o.Token,
                       last_alive = o.LastAlive,
                       is_valid = o.IsValid(),
                       is_authenticated = o.IsAuthenticated,
                   }).ToList();
                return list;
            }
        }

        



        /// <summary>
        /// 查询单个在线网关列表
        /// </summary>
        /// <param name="id">网关id</param>
        /// <returns></returns>
        [Route(""), HttpGet]
        public ApiResult<GatewayItem> Get2(string id)
        {
            return Framework.Instance.Server.Connections()
                .Where(o => o.TerminalId == id)
                   .Select(o => new GatewayItem()
                   {
                       ipaddress = o.RemoteEndPoint.Address.ToString(),
                       session_id = o.SessionId,
                       id = o.TerminalId,
                       token = o.Token,
                       last_alive = o.LastAlive,
                       is_valid = o.IsValid(),
                       is_authenticated = o.IsAuthenticated,
                   }).FirstOrDefault();
        }
        /// <summary>
        /// 查询单个网关是否在线
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("IsGatewayOnline"), HttpGet]
        public ApiResult<bool> IsGatewayOnline(string id)
        {
            var query = Framework.Instance.Server.Connections()
                .Where(o => o.TerminalId == id)
                   .Select(o => new GatewayItem()
                   {
                       ipaddress = o.RemoteEndPoint.Address.ToString(),
                       session_id = o.SessionId,
                       id = o.TerminalId,
                       token = o.Token,
                       last_alive = o.LastAlive,
                       is_valid = o.IsValid(),
                       is_authenticated = o.IsAuthenticated,
                   }).FirstOrDefault();
            if (query != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 设置网关参数
        /// </summary>
        /// <param name="model">网关参数</param>
        /// <returns></returns>
        [Route("params/base"), HttpPost]
        public ApiResult<bool> params_base(IGatewayParam<BaseParam> model)
        {
            if (model == null)
                return "数据不能为空。";
            if (this.ModelState.IsValid == false)
                return string.Join("\r\n", ModelState.SelectMany(o => o.Value.Errors.Select(v => v.ErrorMessage)));
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(model.id);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = model.id,
                Data = new SensorNetwork.Protocol.Packet.PSET.BBasePacketData()
                {
                    Param = model.data,
                }
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is CommonPacketData)
            {
                return (r.Data as CommonPacketData).ErrorCode == 0;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 查询网关参数
        /// </summary>
        /// <param name="id">网关id</param>
        /// <returns></returns>
        [Route("params/base"), HttpGet]
        public ApiResult<BaseParam> params_base(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "网关id不正确";
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(id);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = id,
                Data = EmptyPacketData.Create(ServiceType.PQRY, SensorNetwork.Protocol.Packet.PQRY.ServiceCode.BASE)
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is SensorNetwork.Protocol.Packet.PSET.BBasePacketData)
            {
                return (r.Data as SensorNetwork.Protocol.Packet.PSET.BBasePacketData).Param;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 查询网关参数
        /// </summary>
        /// <param name="id">网关id</param>
        /// <returns></returns>
        [Route("params/video"), HttpGet]
        public ApiResult<VideoSourceParam> params_video(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "网关id不正确";
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(id);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = id,
                Data = EmptyPacketData.Create(ServiceType.PQRY, SensorNetwork.Protocol.Packet.PQRY.ServiceCode.VIDEOSOURCE)
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is VideoSourcePacketData)
            {
                return (r.Data as VideoSourcePacketData).Param;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 查询我自己的网关（包含不在线的）
        /// </summary>
        /// <param name="appuid">AppUserId</param>
        /// <returns></returns>
        [Route("my"), HttpGet]
        public async Task<ApiResult<List<GatewayBindItem>>> MyGateway(string appuid)
        {
            if (string.IsNullOrWhiteSpace(appuid))
                return "appuid不正确";

            var ls = await db.GetCollection<Gateway>().Where(o => o.app_user_id == appuid)
                  .Select(o => new GatewayBindItem()
                  {
                      id = o.GatewayID,
                      last_updated = o.last_updated,
                      ip = o.ip,
                      mac = o.mac,
                      port = o.port,
                  }).ToListAsync();

            return ls;
        }


        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [Route("test"), HttpGet]
        public async Task<ApiResult<byte[]>> Test()
        {
            var p = new TPKGHead()
            {
                Term_Code = "321409000001",
                Data = new SensorNetwork.Protocol.Packet.LINK.LoginPacketData()
                {
                    AppUserID = "gaohan",
                    Cloud_Code = "3214",
                    Username = "gaohan",
                    Password = "gaohan1234"
                }
            };
            return BitConverter.ToString(p.ToBytes()).Replace("-", " ");
        }





        /// <summary>
        /// 查询可见未知蓝牙设备列表
        /// </summary>
        /// <returns></returns>
        [Route("Bluetooth/DeviceList"), HttpGet]
        public ApiResult<BluetoothParam> DeviceList(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "网关id不正确";
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(id);//取在线网关
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = id,
                Data = EmptyPacketData.Create(ServiceType.PQRY, SensorNetwork.Protocol.Packet.PQRY.ServiceCode.QRYBT)
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is BluetoothPacketData)
            {
                return (r.Data as BluetoothPacketData).Param;
            }
            return "网关响应不正确。";
        }
        /// <summary>
        /// 查询已连接的蓝牙设备列表
        /// </summary>
        /// <returns></returns>
        [Route("Bluetooth/DeviceConnectedList"), HttpGet]
        public ApiResult<QRYONLINEBTParam> DeviceConnectedList(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "网关id不正确";
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(id);//取在线网关
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = id,
                Data = EmptyPacketData.Create(ServiceType.PQRY, SensorNetwork.Protocol.Packet.PQRY.ServiceCode.QRYONLINEBT)
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is QRYONLINEBTPacketData)
            {
                return (r.Data as QRYONLINEBTPacketData).Param;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 连接指定蓝牙设备
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Bluetooth/Connection"), HttpPost]
        public ApiResult<bool> Bluetooth_Connection(BluetoothConnectionParam model)
        {
            //if (Gateway_Id == null || string.IsNullOrEmpty(Gateway_Id))
            //    return "网关ID不能为空。";
            //if (Equip_Name == null || string.IsNullOrEmpty(Equip_Name))
            //    return "设备名称不能为空。";
            //if (Equip_MAC == null || string.IsNullOrEmpty(Equip_MAC))
            //    return "设备MAC地址不能为空。";

            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(model.GatewayID);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = model.GatewayID,
                Data = new SensorNetwork.Protocol.Packet.CTRL.BluetoothConnectPacketData()
                {
                    Equip_UID=model.UID,
                    Equip_MAC = model.MAC,
                    PinCode_Length=model.PinCode_Len,
                    PinCode=model.PinCode
                }
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is CommonPacketData)
            {
                return (r.Data as CommonPacketData).ErrorCode == 0;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 删除指定蓝牙设备
        /// </summary>
        /// <param name="Gateway_Id">网关ID</param>
        /// <param name="Equip_Name">蓝牙设备名称</param>
        /// <param name="Equip_MAC">蓝牙设备MAC</param>
        /// <returns></returns>
        [Route("Bluetooth/Delete"), HttpPost]
        public ApiResult<bool> Bluetooth_Del(string Gateway_Id, string UID)
        {
            if (Gateway_Id == null || string.IsNullOrEmpty(Gateway_Id))
                return "网关ID不能为空。";
            if (UID == null || string.IsNullOrEmpty(UID))
                return "UID。";
            //if (Equip_MAC == null || string.IsNullOrEmpty(Equip_MAC))
            //    return "设备MAC地址不能为空。";

            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(Gateway_Id);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = Gateway_Id,
                Data = new SensorNetwork.Protocol.Packet.CTRL.BluetoothDeletePacketData()
                {
                    UID=UID
                }
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is CommonPacketData)
            {
                return (r.Data as CommonPacketData).ErrorCode == 0;
            }
            return "网关响应不正确。";
        }

        /// <summary>
        /// 向指定蓝牙设备发送指令
        /// </summary>
        /// <param name="Gateway_Id">网关ID</param>
        /// <param name="CMD">指定蓝牙设备发送的指令,二进制指令包内容使用base64编码后的串传入</param>
        /// <param name="Equip_MAC">蓝牙设备MAC</param>
        /// <returns></returns>
        [Route("Bluetooth/SendCommand"), HttpPost]
        public ApiResult<int?> Bluetooth_SendCMD(string Gateway_Id, string UID, string CMD)
        {
            if (Gateway_Id == null || string.IsNullOrEmpty(Gateway_Id))
                return "网关ID不能为空。";
            if (CMD == null || string.IsNullOrEmpty(CMD))
                return "指令不能为空。";
            if (UID == null || string.IsNullOrEmpty(UID))
                return "UID不能为空。";
            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(Gateway_Id);
            if (gateway == null)
                return "网关不在线。";
            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = Gateway_Id,
                Data = new SensorNetwork.Protocol.Packet.CTRL.BluetoothCommandPacketData()
                {
                    UID = UID,
                    CMD = Convert.FromBase64String(CMD)
                }
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is CommonPacketData)
            {
                return (r.Data as CommonPacketData).ErrorCode;//该命令不能用来向蓝牙设备请求测量数据，比如无法请求到血糖仪数据
            }
            return "网关响应不正确。";
        }

        ///// <summary>
        ///// 向指定蓝牙设备发送指令
        ///// </summary>
        ///// <param name="Gateway_Id">网关ID</param>
        ///// <param name="UID">蓝牙设备UID</param>
        ///// <returns></returns>
        //[Route("Bluetooth/SendCommand"), HttpPost]
        //public ApiResult<int?> Bluetooth_SendCMD(string Gateway_Id, string UID)
        //{
        //    if (Gateway_Id == null || string.IsNullOrEmpty(Gateway_Id))
        //        return "网关ID不能为空。";

        //    if (UID == null || string.IsNullOrEmpty(UID))
        //        return "设备UID不能为空。";
        //    string Equip_MAC = "";//需要根据UID查询设备MAC
        //    byte[] arr = new byte[] { 0x01, 0x02, 0x03 };//指令集
        //    string CMD = Convert.ToBase64String(arr); //指定蓝牙设备发送的指令,二进制指令包内容使用base64编码后的串传入

        //    var gateway = Framework.Instance.Server.GetConnectionByTerminalId(Gateway_Id);
        //    if (gateway == null)
        //        return "网关不在线。";

        //    var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
        //    {
        //        Term_Code = Gateway_Id,
        //        Data = new SensorNetwork.Protocol.Packet.CTRL.BluetoothCommandPacketData()
        //        {
        //            MAC = Equip_MAC,
        //            CMD = Convert.FromBase64String(CMD)
        //        }
        //    });

        //    if (r == null)
        //        return "网关不在线或者无响应。";

        //    if (r.Data is CommonPacketData)
        //    {
        //        return (r.Data as CommonPacketData).ErrorCode;//该命令不能用来向蓝牙设备请求测量数据，比如无法请求到血糖仪数据
        //    }
        //    return "网关响应不正确。";
        //}

        /// <summary>
        /// 设置传感器上传时间间隔,温度、温度、空气质量三个时间间隔分开设置
        /// </summary>
        /// <param name="Gateway_Id">网关ID</param>
        /// <param name="TP">温度数据上传的时间间隔</param>
        /// <param name="HM">温度数据上传的时间间隔</param>
        /// <param name="Air">空气质量数据上传的时间间隔</param>
        /// <returns></returns>
        [Route("SetInterval"), HttpPost]
        public ApiResult<bool> Gateway_SetInterval(string Gateway_Id, uint TP = 30, uint HM = 30, uint Air = 30)
        {
            log.Info("Web API:Gateway_SetInterval(Gateway_Id:" + Gateway_Id + ",TP:" + TP + ",HM:" + HM + ",Air:" + Air + ")");
            if (Gateway_Id == null || string.IsNullOrEmpty(Gateway_Id))
                return "网关ID不能为空。";

            var gateway = Framework.Instance.Server.GetConnectionByTerminalId(Gateway_Id);
            if (gateway == null)
                return "网关不在线。";

            var r = Framework.Instance.SendAndWaitTerminalResponse(new TPKGHead()
            {
                Term_Code = Gateway_Id,
                Data = new SensorNetwork.Protocol.Packet.PSET.IntervalData()
                {
                    Param = new IntervalParam()
                    {
                        TP_Interval = TP,
                        HM_Interval = HM,
                        Air_Interval = Air
                    }
                }
            });

            if (r == null)
                return "网关不在线或者无响应。";

            if (r.Data is CommonPacketData)
            {
                return (r.Data as CommonPacketData).ErrorCode == 0;
            }
            return "网关响应不正确。";
        }
        /// <summary>
        /// 根据UID获取蓝牙设备最新的N条记录
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        [Route("Bluetooth/GetLatestListTopN"), HttpGet]
        public ApiResult<List<string>> GetLatestListTopN(string UID, int N)
        {
            if (string.IsNullOrWhiteSpace(UID))
                return "UID不正确。";
            if (N < 1)
                return "N必须大于等于1。";
            //var collection = db.GetCollection<SensorNetwork.Data.Models.BluetoothSensorLog>();
            //var query = from q in db.GetCollection<SensorNetwork.Data.Models.BluetoothSensorLog>()
            //            where q.UID == UID                         

            //            select q;


            //var query = db.GetCollection<SensorNetwork.Data.Models.BluetoothSensorLog>().Where(o => o.UID == UID).DoQuery().SortByDescending(o => o.Created).Limit(N)
            // .ToList();

            var ls = db.GetCollection<SensorNetwork.Data.Models.BluetoothSensorLog>().Where(o => o.UID == UID).DoQuery().SortByDescending(o => o.Created)
                .Limit(N).Select(o => o.Json).ToList();

            //return new ApiResult<List< BluetoothSensorLog >> (query);
            return ls;
        }
    }
}
