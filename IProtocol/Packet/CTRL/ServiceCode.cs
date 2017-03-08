using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.CTRL
{
    public enum ServiceCode
    {
        None,
        RESET,
        DATACLR,
        RESTORE,
        AJUSTTIME,
        BLUCON,
        BLUDEL,
        BLUCMD,
        FILERENAME,
        FILEOPT,
        QRYFILELIST
    }
}
