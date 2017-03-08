using IM.Shared;
using SensorNetwork.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace SensorNetwork.Server.Service
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();
            this.serviceInstaller1.ServiceName = Program.ServiceName;
            serviceInstaller1.DisplayName = AppAssembly.AssemblyTitle;
            serviceInstaller1.Description = AppAssembly.AssemblyDescription;
        }
    }
}
