using SensorNetwork.Server.Management;
using IM.Shared;
using SensorNetwork.Server.Service;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SensorNetwork.Server
{
    class Program
    {

        public const string ServiceName = "SensorNetworkService";

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleIcon(IntPtr hIcon);
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static   MonitorService ms = new MonitorService();
        static void Main(string[] args)
        {
            //new StatTask4().DoTest();
            int i1, i2;
            System.Threading.ThreadPool.GetMaxThreads(out i1, out i2);
            System.Threading.ThreadPool.SetMaxThreads(i1 * 2, i2 * 2);
            //log4net.Config.XmlConfigurator.Configure();
            log.DebugFormat("{0} 作为[{1}]已经启动。", AppAssembly.AssemblyTitle, Environment.UserInteractive ? "应用程序" : "服务");
            Environment.CurrentDirectory = Application.StartupPath;
            if (Environment.UserInteractive)
            {
                RunAsWinForm(args);
            }
            else
            {
                RunAsService(args);
            }

        }

        private static void RunAsWinForm(string[] args)
        {
            var icon = SensorNetwork.Server.Properties.Resources.App;
            SetConsoleIcon(icon.Handle);

            Arguments arg = new Arguments(args);
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool isSilentInstall = arg.HasProperty("s") || arg.HasProperty("silent");
            bool isStartService = arg.HasProperty("start");
            bool isStopService = arg.HasProperty("stop");

            if (arg.HasProperty("i") || arg.HasProperty("install"))
            {
                if (AppAssembly.IsRunAsAdministrator == false)
                {
                    AppAssembly.Restart(true, true);
                    return;
                }
                try
                {
                    Install(false, new string[] { });
                    if (isStartService)
                    {
                        ServiceControl(true, isSilentInstall);
                    }
                    if (isSilentInstall == false)
                    {
                        MessageBox.Show("服务安装成功。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    if (isStartService)
                    {
                        ServiceControl(true, isSilentInstall);
                    }
                    if (isSilentInstall == false)
                    {
                        MessageBox.Show(ex.Message, "安装服务失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (arg.HasProperty("u") || arg.HasProperty("uninstall"))
            {
                if (AppAssembly.IsRunAsAdministrator == false)
                {
                    AppAssembly.Restart(true, true);
                    return;
                }
                try
                {
                    Install(true, new string[] { });
                    if (isSilentInstall == false)
                    {
                        MessageBox.Show("服务卸载成功。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    if (isSilentInstall == false)
                    {
                        MessageBox.Show(ex.Message, "卸载服务失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (isStartService)
            {
                if (AppAssembly.IsRunAsAdministrator == false)
                {
                    AppAssembly.Restart(true, true);
                    return;
                }
                bool b = ServiceControl(true, isSilentInstall);
                if (b && isSilentInstall == false)
                {
                    MessageBox.Show("服务启动完成。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (isStopService)
            {
                if (AppAssembly.IsRunAsAdministrator == false)
                {
                    AppAssembly.Restart(true, true);
                    return;
                }
                bool b = ServiceControl(false, isSilentInstall);
                if (b && isSilentInstall == false)
                {
                    MessageBox.Show("服务停止完成。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (arg.HasProperty("?") || arg.HasProperty("h") || arg.HasProperty("help"))
            {
                MessageBox.Show(GetDescription(), "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                Console.Title = string.Format("RD.Server, Started:{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                InitializeEnvironment();
                try
                {
                    Framework frm = new Framework();
                }
                catch (Exception ex)
                {
                    log.Fatal(ex);

                    if (Environment.UserInteractive)
                        Console.ReadKey();
                }
            }
        }

        public static string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("编译日期:{0}", AppAssembly.ExecutingAssemblyCompiledDate());
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("运行参数：");
            sb.AppendLine("-i -install   \t安装服务");
            sb.AppendLine("-u -uninstall \t卸载服务");
            sb.AppendLine("-s -silent    \t静默安装或者卸载");
            sb.AppendLine("-start        \t启动服务");
            sb.AppendLine("-stop         \t停止服务");
            sb.AppendLine("-h -? -help   \t显示帮助信息");
            return sb.ToString();
        }

        public static bool ServiceControl(bool isStart, bool isSilent)
        {
            try
            {
                ServiceController sc = new ServiceController(ServiceName);
                if (isStart && sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.Refresh();
                }
                else if (isStart == false && sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.Refresh();
                }

                return true;
            }
            catch (Exception e)
            {
                if (isSilent == false)
                {
                    MessageBox.Show(e.Message);
                }
            }

            return false;
        }

        private static void RunAsService(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceMain()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static bool Install(bool isUninstall, string[] args)
        {
            try
            {
                log.DebugFormat(isUninstall ? "Uninstalling" : "Installing");
                using (AssemblyInstaller inst = new AssemblyInstaller(typeof(Service.ServiceInstaller).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (isUninstall)
                        {
                            inst.Uninstall(state);
                        }
                        else
                        {
                            inst.Install(state);
                            inst.Commit(state);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        log.Debug(ex.Message);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                throw ex;
            }
            //return false;
        }


        #region 通用部分
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        public static void InitializeEnvironment()
        {
            TypeCache.DoInitialize();
            Environment.CurrentDirectory = Application.StartupPath;
            Thread.CurrentThread.Name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            log.Info("Has already started.");
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        #region 未处理异常捕获
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnUnhandledException(sender, e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            OnUnhandledException(sender, e.ExceptionObject);
        }


        private static void OnUnhandledException(object sender, object exception)
        {
            log.Fatal(sender);
            log.Fatal(exception);
            var logger = log.Logger as Logger;
            if (logger != null)
            {
                foreach (IAppender appender in logger.Appenders)
                {
                    var buffered = appender as BufferingAppenderSkeleton;
                    if (buffered != null)
                    {
                        buffered.Flush();
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
