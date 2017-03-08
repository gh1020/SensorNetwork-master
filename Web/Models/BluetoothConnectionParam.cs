using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BluetoothConnectionParam
    {
        public string GatewayID { get; set; }
        public string UID { get; set; }
        public string MAC { get; set; }
        public byte PinCode_Len { get; set; }
        public string PinCode { get; set; }
    }
}