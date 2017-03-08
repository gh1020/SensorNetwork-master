using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol
{
    public enum ServiceType
    {
        None,
        RESP,
        LINK,
        PSET,
        PQRY,
        CTRL,
        DQRY
    }
}
