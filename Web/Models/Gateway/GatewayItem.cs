using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Web.Models.Gateway
{
    /// <summary>
    /// 网关列表
    /// </summary>
    public class GatewayItem
    {
        /// <summary>
        /// 是否已经通过验证
        /// </summary>
        public bool is_authenticated { get; internal set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool is_valid { get; internal set; }
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime last_alive { get; internal set; }
        /// <summary>
        /// 网关地址
        /// </summary>
        public string ipaddress { get; internal set; }
        /// <summary>
        /// 回话id
        /// </summary>
        public string session_id { get; internal set; }
        /// <summary>
        /// 网关id
        /// </summary>
        public string id { get; internal set; }
        /// <summary>
        /// 网关令牌
        /// </summary>
        public Guid token { get; internal set; }
    }
}