using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        /// <summary>
        /// 更新日志
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateLog()
        {
            return View();
        }

        /// <summary>
        /// 开发说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Standard()
        {
            return View();
        }
        /// <summary>
        /// 数据接收说明
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiveData()
        {
            return View();
        }
        /// <summary>
        /// 数据描述
        /// </summary>
        /// <returns></returns>
        public ActionResult TestResultDesc()
        {
            return View();
        }

        /// <summary>
        /// 查询单一网关是否在线
        /// </summary>
        /// <returns></returns>
        public ActionResult QuerySingleGatewayStatus()
        {
            return View();
        }
        public ActionResult InterAuthExplain()
        {
            return View();
        }

        public ActionResult Waiting()
        {
            return View();
        }
        public ActionResult GetBluetoothLatestTopN()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string strPwd = fc["pwd"];
            if (strPwd == "gaohan")
            {
                Session["LOGINPWD"] = strPwd;
                return RedirectToAction("Index", "Document");
            }
            else {
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 查询单个网关是否在线
        /// </summary>
        /// <returns></returns>
        public ActionResult QuerySingleGatewayOnline()
        {
            return View();
        }

        /// <summary>
        /// 查询蓝牙设备历史数据
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryBluetoothDeviceHistoryData()
        {
            return View();
        }

        /// <summary>
        /// 查询蓝牙设备最新数据
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryBluetoothDeviceNewData()
        {
            return View();
        }

        /// <summary>
        /// 查询网关下蓝牙设备
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryGatewayHaveBluetoothDevice()
        {
            return View();
        }

        /// <summary>
        /// 查询在线网关列表
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryOnlineGatewayList()
        {
            return View();
        }

        /// <summary>
        /// 连接蓝牙设备
        /// </summary>
        /// <returns></returns>
        public ActionResult ContectBluetoothDevice()
        {
            return View();
        }

        /// <summary>
        /// 删除蓝牙设备
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteBluetoothDevice()
        {
            return View();
        }

        /// <summary>
        /// 设置网关参数
        /// </summary>
        /// <returns></returns>
        public ActionResult SettingGatewayParam()
        {
            return View();
        }

        /// <summary>
        /// 设置网关传感器上传时间间隔
        /// </summary>
        /// <returns></returns>
        public ActionResult SettingGatewaySensorUploadTimeInterval()
        {
            return View();
        }

        /// <summary>
        /// 网关参数查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GatewayParamQuery()
        {
            return View();
        }

        /// <summary>
        /// 向指定蓝牙设备发送命令
        /// </summary>
        /// <returns></returns>
        public ActionResult SendCommandsToTheSpecifiedBluetoothDevice()
        {
            return View();
        }
    }
}
