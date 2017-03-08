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
    [Table("yUserInterface"), BsonIgnoreExtraElements(Inherited = true)]
    public  class UserInterface
    {
        /// <summary>
        /// 用户选择的接口信息
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Index]
        public string AccessID { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Index]
        public string CategoryName { get; set; }

        /// <summary>
        /// 大分类编号
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 大分类名称
        /// </summary>
        [Index]
        public string TypeName { get; set; }

        /// <summary>
        /// 是否通过审核
        /// </summary>
        [Index]
        public string IsValid { get; set; }

        /// <summary>
        /// Url地址
        /// </summary>
        [Index]
        public string Url { get; set; }

        /// <summary>
        /// 唯一标示
        /// </summary>
        [Index]
        public string sn { get; set; }
    }
}
