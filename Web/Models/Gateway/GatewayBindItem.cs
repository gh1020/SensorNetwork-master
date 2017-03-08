using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Gateway
{
    /// <summary>
    /// 网关绑定信息
    /// </summary>
    public class GatewayBindItem
    {
        /// <summary>
        /// 网关id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// port
        /// </summary>
        public int? port { get; set; }
        /// <summary>
        /// mac
        /// </summary>
        public string mac { get; set; }

        /// <summary>
        /// 最后绑定时间
        /// </summary>
        public DateTime? last_updated { get; set; }
    }
}