using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using SensorNetwork.Server.Management;
using System.Web.Hosting;
using System.Threading;

[assembly: OwinStartup(typeof(Web.Startup))]

namespace Web
{
    public partial class Startup
    {
        Framework framework;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            HostingEnvironment.QueueBackgroundWorkItem(o => StartFramework(o));//启动TCP SERVER服务
        }

        private void StartFramework(CancellationToken o)
        {
            framework = new Framework();
            while (o.IsCancellationRequested == false)
            {
                Thread.Sleep(1000);
            }
            framework.StopListener();
        }

    }
}
