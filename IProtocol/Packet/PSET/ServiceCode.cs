using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PSET
{
    public enum ServiceCode
    {
        None,
        BASE,
        NET,
        INTF,
        NTP,
        FTP,
        PKEY,
        INTERVAL
    }
}
