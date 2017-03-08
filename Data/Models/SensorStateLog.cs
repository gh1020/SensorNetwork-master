using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Data.Models
{
    /// <summary>
    /// 传感器状态记录
    /// </summary>
    [Table("tSensorStateLogs"), BsonIgnoreExtraElements(Inherited = true)]
    public class SensorStateLog
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 网关id
        /// </summary>
        [Index]
        public string gatewayid { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        [Index]
        public string equipid { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public double data { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        [Index]
        public int type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Index]
        public DateTime created { get; set; }
    }
}
