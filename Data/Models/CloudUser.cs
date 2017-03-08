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
    [Table("tCloudUsers"), BsonIgnoreExtraElements(Inherited = true)]
    public class CloudUser
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        /// <summary>
        /// 云平台厂商代码
        /// </summary>
        [Index]
        public string cloud_code { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Index]
        public string username { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Index]
        public string password { get; set; }

    }
}
