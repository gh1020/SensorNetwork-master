using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SensorNetwork.Data.Models;

namespace Web.Controllers
{
    public class InterfaceController : Controller
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SensorNetwork.Data.ApplicationDbContext db = SensorNetwork.Data.ApplicationDbContext.Default();

        [HttpGet]
        public ActionResult AddInterface()
        {
            if (Session["AccessID"] != null)
            {
                // 所有的
                // 从 yInterface表中取数据 父分类
                List<Interfaces> interList = db.GetCollection<Interfaces>().Where(o => o.CategoryDistinction == "01").ToList();

                Dictionary<string, List<Interfaces>> allDic = new Dictionary<string, List<Interfaces>>();
                // 遍历父分类，根据父分类的CategoryCode来取出他的子分类
                for (int i = 0; i < interList.Count; i++)
                {
                    List<Interfaces> list = db.GetCollection<Interfaces>().Where(o => o.CategoryDistinction == (interList[i] as Interfaces).CategoryCode).Where(o => o.IsValid == "是").ToList();

                    var inter = interList[i] as Interfaces;
                    allDic.Add(inter.CategoryName, list);
                }

                // 已经选择的接口
                Dictionary<string, List<Interfaces>> selectDic = GetChoosedInterfaceByIsValid();

                // 差集
                Dictionary<string, List<Interfaces>> chaDic = new Dictionary<string, List<Interfaces>>();

                foreach (string key in allDic.Keys)
                {
                    if (selectDic.ContainsKey(key))
                    {
                        List<Interfaces> chaList = GetDifferent(allDic[key], selectDic[key]);
                        if (chaList.Count > 0)
                        {
                            chaDic.Add(key, chaList);
                        }
                    }
                    else
                    {
                        chaDic.Add(key, allDic[key]);
                    }
                }
                ViewData["dic"] = chaDic;

            }
           
            return View();

        }

        public Dictionary<string, List<Interfaces>> GetChoosedInterfaceByIsValid(string isValid = "")
        {
            var userInterList = db.GetCollection<UserInterface>().Where(o => o.AccessID == Session["AccessID"].ToString()).ToList();
            if (isValid != "")
            {
                userInterList = db.GetCollection<UserInterface>().Where(o => o.AccessID == Session["AccessID"].ToString()).Where(o => o.IsValid == isValid).ToList();
            }

            Dictionary<string, List<Interfaces>> selectDic = new Dictionary<string, List<Interfaces>>();
            foreach (UserInterface inter in userInterList)
            {
                Interfaces sel = db.GetCollection<Interfaces>().Where(o => o.CategoryCode == inter.CategoryCode).ToList().First();
                if (sel != null)
                {
                    if (!selectDic.ContainsKey(inter.TypeName))
                    {
                        selectDic.Add(inter.TypeName, new List<Interfaces>());
                    }

                    selectDic[inter.TypeName].Add(sel);
                }
            }
            return selectDic;
        }

        public List<Interfaces> GetDifferent(List<Interfaces> list1, List<Interfaces> list2)
        {
            List<Interfaces> chaList = new List<Interfaces>();
            Dictionary<string, Interfaces> dic = new Dictionary<string, Interfaces>();
            List<Interfaces> smallList, bigList;
            if (list1.Count >= list2.Count)
            {
                bigList = list1;
                smallList = list2;
            }
            else
            {
                bigList = list2;
                smallList = list1;
            }
            foreach (Interfaces item in smallList)
            {
                dic.Add(item.CategoryName, item);
            }

            foreach (Interfaces item in bigList)
            {
                var suc = dic.ContainsKey(item.CategoryName);
                if (suc == false)
                {
                    chaList.Add(item);
                }
            }

            return chaList;
        }


        [HttpPost]
        public string AddInterface(string[] interfaces)
        {
            if (Session["AccessID"] != null)
            {
                string accessID = Session["AccessID"].ToString();
                List<UserInterface> userInterList = new List<UserInterface>();
                if (interfaces == null)
                {
                    return "请选择接口";
                }
                else
                {
                    for (int i = 0; i < interfaces.Length; i++)
                    {
                        string typeCode = interfaces[i].Substring(0, 3);

                        // 父类
                        List<Interfaces> typeList = db.GetCollection<Interfaces>().Where(o => o.CategoryCode == typeCode).ToList();

                        // 子类
                        List<Interfaces> categoryList = db.GetCollection<Interfaces>().Where(o => o.CategoryCode == interfaces[i]).ToList();

                        string typeName = "";
                        string cateName = "";
                        string url = "";
                        if (typeList.Count > 0)
                        {
                            Interfaces inter = typeList.First();
                            typeName = inter.CategoryName;
                        }
                        if (categoryList.Count > 0)
                        {
                            Interfaces inter = categoryList.First();
                            cateName = inter.CategoryName;
                            url = inter.Url;

                            // sn
                            DateTime nowTime = DateTime.Now;
                            string nowTimeStr = nowTime.ToString();
                            string mill = nowTime.Millisecond.ToString();
                            string timestr = nowTimeStr.Replace("/", "").Replace(":", "").Replace(" ", "") + mill;

                            string sn = accessID + interfaces[i] + timestr;

                            userInterList.Add(new UserInterface
                            {
                                AccessID = Session["AccessID"].ToString(),
                                CategoryCode = interfaces[i],
                                CategoryName = cateName,
                                TypeCode = typeCode,
                                TypeName = typeName,
                                Url = url,
                                IsValid = "否",
                                sn = sn
                            });
                        }
                    }
                    db.GetCollection<UserInterface>().InsertManyAsync(userInterList);

                    db.GetCollection<ApplyCloudPlatform>().Where(o => o.AccessID == accessID)
                        .UpdateOneAsync(Builders<ApplyCloudPlatform>.Update
                        .Set(o => o.IsValid_Interface, "否")
                        );

                    return "提交成功";
                }
            }
            return "请重新登录";
        }

        [HttpGet]
        public ActionResult WaitAuditInterface()
        {
            if(Session["AccessID"] != null)
            {
                // 获取用户新增的未通过审核的接口
                Dictionary<string, List<Interfaces>> dic = GetChoosedInterfaceByIsValid("否");
                if(dic.Count > 0)
                {
                    ViewData["dic"] = dic;
                }
                else
                {
                    ViewData["dic"] = null;
                }
            }
           
            return View();
        }
    }
}