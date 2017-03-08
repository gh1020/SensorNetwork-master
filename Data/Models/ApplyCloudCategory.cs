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
    [Table("yApplyCloudCategory"), BsonIgnoreExtraElements(Inherited = true)]
    public class ApplyCloudCategory
    {
        /// <summary>
        /// 平台行业分类
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 类型编号
        /// </summary>
        [Index]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 类型名
        /// </summary>
        [Index]
        public string CategoryName { get; set; }
    }
}
