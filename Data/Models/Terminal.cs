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
    [Table("tTerminals"), BsonIgnoreExtraElements(Inherited = true)]
    public class Terminal
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 设备id
        /// </summary>
        [Index(Unique = true), BsonIgnoreIfNull]
        public string deviceid { get; set; }


        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime? expried { set; get; }


        /// <summary>
        /// 品牌
        /// </summary>
        [BsonIgnoreIfNull]
        public string model_no { set; get; }


        /// <summary>
        /// 制造商ID
        /// </summary>
        [BsonIgnoreIfNull]
        public string manufacture_id { get; set; }

        /// <summary>
        /// 型号或版本号
        /// </summary>
        [BsonIgnoreIfNull]
        public string version { set; get; }


        /// <summary>
        /// 是否在线
        /// </summary>
        public bool? online { get; set; }

        /// <summary>
        /// 最后活动
        /// </summary>
        [BsonIgnoreIfNull]
        public DateTime? last_alive { get; set; }


        /// <summary>
        /// 最后来源IP地址
        /// </summary>
        [BsonIgnoreIfNull]
        public string last_ipaddress { set; get; }
        /// <summary>
        /// 最后连接时间
        /// </summary>
        [BsonIgnoreIfNull]
        public DateTime? last_connected { set; get; }
        /// <summary>
        /// 最后断开时间
        /// </summary>
        [BsonIgnoreIfNull]
        public DateTime? last_disconnected { set; get; }


        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime created { set; get; }


    }
}
