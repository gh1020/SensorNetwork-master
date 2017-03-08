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
    [Table("dSendStructure"), BsonIgnoreExtraElements(Inherited = true)]
    public class SendStructure
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        /// <summary>
        /// 传感器编号
        /// </summary>
        [Index]
        public string SensorSerialNumber { get; set; }

        /// <summary>
        /// UID前16位
        /// </summary>
        [Index]
        public string UID_16 { get; set; }

        /// <summary>
        /// 传感器中文名
        /// </summary>
        [Index]
        public string SensorChineseFullname { get; set; }
        /// <summary>
        /// 字节中文名
        /// </summary>
        [Index]
        public string ByteChineseName { get; set; }
        /// <summary>
        /// 字节英文名
        /// </summary>
        [Index]
        public string ByteEnglishName { get; set; }
        /// <summary>
        /// 字节开始位置
        /// </summary>
        [Index]
        public string LocationBegin { get; set; }
        /// <summary>
        /// 字节结束位置
        /// </summary>
        [Index]
        public string LocationEnd { get; set; }
        /// <summary>
        /// 字节默认值
        /// </summary>
        [Index]
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否为数据域
        /// </summary>
        [Index]
        public string IsDataArea { get; set; }
        /// <summary>
        /// 数据描述
        /// </summary>
        [Index]
        public string DataDesc { get; set; }
        /// <summary>
        /// 公式
        /// </summary>
        [Index]
        public string Formula { get; set; }
        /// <summary>
        /// 其它描述
        /// </summary>
        [Index]
        public string OtherDescription { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Index]
        public string CreatedDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Index]
        public string Operator { get; set; }
        /// <summary>
        /// sn号
        /// </summary>
        [Index]
        public string sn { get; set; }
    }
}
