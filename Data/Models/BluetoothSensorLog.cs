using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
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
    [Table("tBluetoothSensorLog"), BsonIgnoreExtraElements(Inherited = true)]
    public class BluetoothSensorLog
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
        public string GatewayID { get; set; }
        /// <summary>
        /// 蓝牙设备UID
        /// </summary>
        [Index]
        public string UID { get; set; }
        /// <summary>
        /// Json包
        /// </summary>
        [Index]
        public string Json { get; set; }      

        /// <summary>
        /// 创建时间
        /// </summary>
        [Index]
        public DateTime Created { get; set; }
    }
}
