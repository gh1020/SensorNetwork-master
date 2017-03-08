using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using SensorNetwork.Uart.Sockets;
using System.Diagnostics;
using System.Configuration;
using SensorNetwork.Protocol;
using SensorNetwork.Server.Management;
using SensorNetwork.Server;
using System.Collections.Concurrent;
using SensorNetwork.Data;

namespace SensorNetwork.Server.Management
{
    public partial class Framework : INetworkTransmitter
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApplicationDbContext db;
        public static ConcurrentQueue<LogItem> logs = new ConcurrentQueue<LogItem>();
        /// <summary>
        /// 是否真正销毁中
        /// </summary>
        public bool Disposing { get; private set; }
        private System.Threading.Timer timer;
        //public ProtocolManager ProtocolManager { get; private set; }
        private Guid ServerId;

        private DateTime startTime = DateTime.Now;

        public static Framework Instance { get; private set; }
        /// <summary>
        /// Web端登录领牌
        /// </summary>
        public uint WebLoginToken { get; private set; }

        public static void AddLog(string desc, params object[] args)
        {
            AddLog("server", null, desc, args);
        }
        public static void AddLog(string id, object data, string desc, params object[] args)
        {
            logs.Enqueue(new LogItem()
            {
                desc = string.Format(desc, args),
                id = id,
                time = DateTime.Now,
                data = data
            });
            //限制只保存1000条
            while (logs.AsQueryable().Count() > 1000)
            {
                LogItem l;
                logs.TryDequeue(out l);
            }
        }

        public Framework()
        {
            Instance = this;
            log.InfoFormat("Initializing ...");
#if !STANDALONE
            db =  ApplicationDbContext.Default();
#endif
            WebLoginToken = ConfigurationManager.AppSettings.GetValue<uint>("WebLoginToken", 245415524);
            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                ExitThread();
            }

            try
            {
                ServerId = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["ServerId"]);
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                ExitThread();
            }

#if !STANDALONE
            SetTerminalConnectionLogToDisconnected();
#endif
            //间隔
            timer = new System.Threading.Timer(PeriodWorkThread, null, 10000, 60000);
            if (StartListener() == false)
                ExitThread();
            //ProtocolManager = new ProtocolManager(this);
            //ProtocolManager.Load();
#if !STANDALONE
            StartUpdateTerminalStateThread();//刷新网关终端状态信息写入数据库
#endif
            log.InfoFormat("Initialized.");
            //var t = db.TerminalCommandLogs.Find(o => o.TerminalId == 15818693649 && o.RetryTimes == 10).SortByDescending(o => o.Sent).FirstOrDefaultAsync().GetAwaiter().GetResult();
            //TerminalCommandLogger.SendCommandToAgentAsync(new AsyncSocketConnection() { TerminalId = 15818693649 },t);
            AddLog("服务器已启动。");
        }



        private new void ExitThread()
        {
            Process.GetCurrentProcess().Kill();
        }

    }
}
