using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using log4net;
using SensorNetwork.Server.Management;

namespace SensorNetwork.Server.Service
{
    public partial class ServiceMain : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Framework web;
        public ServiceMain()
        {
            InitializeComponent();
            this.ServiceName = Program.ServiceName;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                web = new Framework();
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        protected override void OnStop()
        {
            web.StopListener();
        }
    }
}
