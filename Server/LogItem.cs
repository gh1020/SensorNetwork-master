using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Server
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }
    }
}
