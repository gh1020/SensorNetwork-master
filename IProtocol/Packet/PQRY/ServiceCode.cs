using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.PQRY
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
        VIDEOSOURCE,
        UPREQ,
        QRYBT,
        INTERVAL,
        QRYONLINEBT
    }
}
