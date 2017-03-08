using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SensorNetwork.Data.Models;
using MongoDB.Driver;
using log4net;
using System.Security.Cryptography;
using System.Text;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SensorNetwork.Data.ApplicationDbContext db = SensorNetwork.Data.ApplicationDbContext.Default();

        [HttpGet]
        public ActionResult Register()
        {
            List<ApplyCloudCategory> cate = db.GetCollection<ApplyCloudCategory>().FindAll().ToList();
            ViewData["category"] = cate;
            return View();
        }
        [HttpPost]
        public string Register(ApplyCloudPlatform acpf, string acCategory, string[] HTTPInterfaces)
        {
            if (acpf.PlatformName != "" && acpf.PlatformName != null &&
                acpf.AccessID != "" && acpf.AccessID != null &&
                acpf.AccessPwd != "" && acpf.AccessPwd != null &&
                HTTPInterfaces[0] != "")
            {

                List<ApplyCloudPlatform> listPlatform = db.GetCollection<ApplyCloudPlatform>().Where(o => o.AccessID == acpf.AccessID).ToList();
                if (listPlatform.Count != 0)
                {
                    return "该账号已存在";
                }
                else
                {
                    // 平台编号
                    List<ApplyCloudPlatform> allList = db.GetCollection<ApplyCloudPlatform>().FindAll().ToList();
                    ApplyCloudPlatform big = allList.OrderByDescending(o => o.PlatformCode).ToList().First();

                    int bigCode = Convert.ToInt32(big.PlatformCode);
                    bigCode++;
                
                    if (bigCode.ToString().Length <= 8)
                    {// 不超过超过8位数
                        StringBuilder prevStr = new StringBuilder();
                        for (int i = 0; i < 8 - bigCode.ToString().Length; i++)
                        {
                            prevStr.Append("0");
                        }
                        acpf.PlatformCode = prevStr.Append(bigCode).ToString();
                    }
                    else
                    {// 超过8位
                        
                    }


                    acpf.HTTPInterface = changeHttpInterfaces(HTTPInterfaces);

                    string md5Pwd = GetMD5FromString(acpf.AccessPwd);
                    acpf.AccessPwd = md5Pwd;

                    acpf.IsValid = "是";
                    // 是否有需要审核的接口
                    acpf.IsValid_Interface = "";

                    acpf.CategoryName = acCategory;

                    DateTime nowTime = DateTime.Now;
                    string nowTimeStr = nowTime.ToString();
                    string replaceStr = nowTimeStr.Replace("/", "-");
                    // 注册时间
                    acpf.CreateTime = replaceStr;

                    string mill = nowTime.Millisecond.ToString();
                    string sn = nowTimeStr.Replace("/", "").Replace(":", "").Replace(" ", "") + mill;
                    // sn
                    acpf.sn = sn;

                    db.GetCollection<ApplyCloudPlatform>().InsertOneAsync(acpf);
                    Session.Remove("AccessID");
                    Session["AccessID"] = acpf.AccessID;
                    return "注册成功";
                }

            }
            return "请完善信息";
        }

        public string changeHttpInterfaces(string[] HTTPInterfaces)
        {
            List<string> https = new List<string>();
            for (int i = 0; i < HTTPInterfaces.Length; i++)
            {
                if (HTTPInterfaces[i] != "")
                {
                    https.Add(HTTPInterfaces[i]);
                }
            }
            return string.Join(";", https.ToArray());
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public string Login(string AccessID, string AccessPwd)
        {
            if (AccessID == "" && AccessID == null && AccessPwd == "" && AccessPwd == null)
            {
                return "账号或密码为空";
            }
            else
            {

                List<ApplyCloudPlatform> listPlatform = db.GetCollection<ApplyCloudPlatform>().Where(o => o.AccessID == AccessID).Where(o => o.AccessPwd == GetMD5FromString(AccessPwd)).ToList();
                if (listPlatform.Count > 0)
                {
                    Session["AccessID"] = AccessID;
                    return "登录成功";
                }
                else
                {

                    List<ApplyCloudPlatform> list = db.GetCollection<ApplyCloudPlatform>().Where(o => o.AccessID == AccessID).ToList();
                    if (list.Count > 0)
                    {
                        return "密码错误";
                    }
                    else
                    {
                        return "该账号不存在";
                    }
                }
            }
        }

        [HttpGet]
        public ActionResult Exit()
        {
            Session.Abandon();
            Session.Clear();
            return Redirect("/Document/Index");
        }

        public void GetUserMsg()
        {
            if (Session["AccessID"] != null)
            {
                ViewData["AccessID"] = Session["AccessID"].ToString();

                List<ApplyCloudPlatform> listPlatform = db.GetCollection<ApplyCloudPlatform>().Where(o => o.AccessID == Session["AccessID"].ToString()).ToList();
                if (listPlatform.Count > 0)
                {
                    ViewData["Platform"] = listPlatform.First();
                }
                else
                {
                    ViewData["Platform"] = null;
                }
            }
            else
            {
                ViewData["Platform"] = null;
            }
        }

        [HttpGet]
        public ActionResult ShowUserMsg()
        {

            GetUserMsg();

            return View();
        }

        [HttpGet]
        public ActionResult EditUserMsg()
        {
            GetUserMsg();

            return View();
        }

        [HttpPost]
        public string EditUserMsg( string PlatformName,string AccessID, string[] HTTPInterfaces)
        {
            db.GetCollection<ApplyCloudPlatform>().
                Where(o => o.AccessID == Session["AccessID"].ToString())
                .UpdateOneAsync(Builders<ApplyCloudPlatform>.Update
                .Set(o => o.PlatformName, PlatformName)
                .Set(o => o.HTTPInterface, changeHttpInterfaces(HTTPInterfaces))
                );
            return "修改成功";
        }
         
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string GetMD5FromString(string msg)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] msgBuffer = Encoding.Default.GetBytes(msg);
                byte[] md5Buffer = md5.ComputeHash(msgBuffer);
                md5.Clear();
                StringBuilder sbMd5 = new StringBuilder();
                for (int i = 0; i < md5Buffer.Length; i++)
                {
                    sbMd5.Append(md5Buffer[i].ToString("x2"));
                }
                return sbMd5.ToString();
            }
        }
    }
}