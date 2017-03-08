using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SensorNetwork.Data.Models;
using log4net;
using MongoDB.Driver;

namespace Web.Controllers
{


    //[MyAuthorize]
    public class DocumentController : Controller
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SensorNetwork.Data.ApplicationDbContext db = SensorNetwork.Data.ApplicationDbContext.Default();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Menu()
        {
            if (Session["AccessID"] != null)
            {
                //  查询用户申请并通过审核的接口
                //var builder = Builders<UserInterface>.Filter;
                //var filter1 = builder.Eq("AccessID", Session["AccessID".ToString()])
                //             & builder.Eq("IsValid","是");
                //List<UserInterface> userInterList = db.GetCollection<UserInterface>().Find(filter1).ToList();

                List<UserInterface> userInterList = db.GetCollection<UserInterface>().Where(o => o.AccessID == Session["AccessID"].ToString()).Where(o => o.IsValid == "是").ToList();

                Dictionary<string, List<UserInterface>> dic = new Dictionary<string, List<UserInterface>>();
                 foreach(UserInterface inter in userInterList)
                 {
                      if (!dic.ContainsKey(inter.TypeName))
                      {
                         dic.Add(inter.TypeName, new List<UserInterface>());
                      }
                 }
                 foreach(string key in dic.Keys)
                 {
                      foreach(UserInterface inter in userInterList)
                      {
                         if(key == inter.TypeName)
                          {
                             dic[key].Add(inter);
                           }
                       }
                  }
                  ViewData["dic"] = dic;
                  ViewData["AccessID"] = Session["AccessID"].ToString();

                //查询用户申请但未通过审核的接口
                //var filter2 = builder.Eq("AccessID", Session["AccessID"].ToString())
                //              & builder.Eq("IsValid","否");
                //List<UserInterface> unAllowList = db.GetCollection<UserInterface>().Find(filter2).ToList();

                List<UserInterface> unAllowList = db.GetCollection<UserInterface>().Where(o => o.AccessID == Session["AccessID"].ToString()).Where(o => o.IsValid == "否").ToList();
                 ViewData["hasNotPass"] = unAllowList.Count;
                
            }
            else
            {
                ViewData["dic"] = null;
                ViewData["AccessID"] = null;
            }
            return View("Menu");
        }
        public ActionResult Main()
        {
            return View("Main");
        }
        public ActionResult Top()
        {
            if (Session["AccessID"] != null)
            {
                ViewData["AccessID"] = Session["AccessID"].ToString();
                //var filterBuilder = Builders<UserInterface>.Filter;
                //var filter = filterBuilder.Eq("AccessID", Session["AccessID"].ToString());
                //List<UserInterface> interList = db.GetCollection<UserInterface>().Find(filter).ToList();
                List<UserInterface> interList = db.GetCollection<UserInterface>().Where(o => o.AccessID == Session["AccessID"].ToString()).ToList();
                if (interList.Count == 0)
                {
                    ViewData["interCount"] = "zero";
                }
                else
                {
                    ViewData["interCount"] = null;
                }

                //var builder = Builders<ApplyCloudPlatform>.Filter;
                //var filter1 = builder.Eq("AccessID", Session["AccessID"].ToString());
                //List<ApplyCloudPlatform> listPlatform = db.GetCollection<ApplyCloudPlatform>().Find(filter1).ToList();
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
            return View("Top");
        }

        // GET: Document/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Document/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Document/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Document/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Document/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Document/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Document/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
