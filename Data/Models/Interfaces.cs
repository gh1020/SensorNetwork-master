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
    [Table("yInterface"), BsonIgnoreExtraElements(Inherited = true)]
    public class Interfaces
    {
        /// <summary>
        /// 接口信息
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 父分类编号
        /// </summary>
        [Index]
        public string CategoryDistinction { get; set; }

        /// <summary>
        /// 接口编号
        /// </summary>
        [Index]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [Index]
        public string CategoryName { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Index]
        public string IsValid { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        [Index]
        public string Url { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>
        [Index]
        public string Description { get; set; }

        /// <summary>
        /// 唯一编号
        /// </summary>
        [Index]
        public string sn { get; set; }

       
    }
}
