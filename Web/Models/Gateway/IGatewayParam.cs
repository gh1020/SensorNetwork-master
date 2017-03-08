using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.Gateway
{
    /// <summary>
    /// 设置网关参数
    /// </summary>
    /// <typeparam name="TParam">模板参数，代表任意</typeparam>
    public class IGatewayParam<TParam>
    {
        /// <summary>
        /// 网关id
        /// </summary>
        [Required]
        public string id { get; set; }
        /// <summary>
        /// 网关数据
        /// </summary>
        [Required]
        public TParam data { get; set; }
    }
}