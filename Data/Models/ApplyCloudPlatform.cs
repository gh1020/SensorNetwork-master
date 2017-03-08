using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Data.Models
{

    [Table("tApplyCloudPlatform"), BsonIgnoreExtraElements(Inherited = true)]
    public class ApplyCloudPlatform
    {
        /// <summary>
        ///  平台账户信息
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        /// <summary>
        /// 平台编号
        /// </summary>
        [Index]
        public string PlatformCode { get; set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        [Index]
        public string PlatformName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Index]
        public string AccessID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Index]
        public string AccessPwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Index]
        public string IsValid { get; set; }

        /// <summary>
        /// 是否有需要审核的接口
        /// </summary>
        [Index]
        public string IsValid_Interface { get; set; }

        /// <summary>
        /// ZMQ接口
        /// </summary>
        //[Index]
        //[Required(ErrorMessage = "*ZMQ接口不能为空！")]
        //public string ZMQInterface { get; set; }

        /// <summary>
        /// Http接口
        /// </summary>
        [Index]
        public string HTTPInterface { get; set; }

        /// <summary>
        /// 行业分类名称
        /// </summary>
        [Index]
        public string CategoryName { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [Index]
        public string CreateTime { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        [Index]
        public string Operator { get; set; }

        /// <summary>
        /// 唯一编码
        /// </summary>
        [Index]
        public string sn { get; set; }
    }
}
