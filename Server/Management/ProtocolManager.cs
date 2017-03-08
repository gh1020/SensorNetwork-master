using SensorNetwork.Protocol;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Server.Management
{
    /// <summary>
    /// 协议管理器
    /// </summary>
    public class ProtocolManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<Assembly> assemblies = new List<Assembly>();
        private List<IProtocol> Protocols = null;
        private Type[] Types;
        private INetworkTransmitter transmitter;
        public ProtocolManager(INetworkTransmitter transmitter)
        {
            this.transmitter = transmitter;
        }
        public void Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Protocols");
            log.InfoFormat("load protocol at path: {0}.", path);
            if (Directory.Exists(path) == false)
                return;
            var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            assemblies.Clear();
            foreach (var dll in files)
            {
                log.InfoFormat("try loading {0}", dll);
                try
                {
                    Assembly loadedAssembly = Assembly.LoadFile(dll);
                    log.InfoFormat("loaded {0}", dll);
                    assemblies.Add(loadedAssembly);
                }
                catch (FileLoadException loadEx)
                {
                    log.Error(loadEx);
                }
                catch (BadImageFormatException imgEx)
                {
                    log.Error(imgEx);
                }
            }
            Type baseType = typeof(IProtocol);
            Types = assemblies.SelectMany(o => o.GetTypes())
                 .Where(o => o.IsInterface == false && o.IsAbstract == false && o.GetInterfaces().FirstOrDefault(v => v == baseType) != null)
                 .ToArray();
            Protocols = Types.Select(o =>
             {
                 try
                 {
                     var p = Activator.CreateInstance(o) as IProtocol;
                     p.SetNetworkTransmitter(transmitter);
                     return p;
                 }
                 catch { }
                 return null;
             }).Where(o => o != null).ToList();

        }

        public IProtocol Find(string uid)
        {
            if (Protocols == null)
            {
                Load();
                if (Protocols == null)
                    Protocols = new List<IProtocol>();
            }
            return Protocols.FirstOrDefault(o => o.IsMatch(uid));
        }
    }
}
