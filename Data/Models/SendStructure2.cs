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
    [Table("dSendStructure2"), BsonIgnoreExtraElements(Inherited = true)]
    public class SendStructure2
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        /// <summary>
        /// 传感器编号
        /// </summary>        
        public string SensorSerialNumber { get; set; }

        /// <summary>
        /// UID前16位
        /// </summary>        
        public string UID_16 { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>        
        public string StructName { get; set; }
        /// <summary>
        /// 协议长度
        /// </summary>        
        public string Length { get; set; }
        /// <summary>
        /// 命令字
        /// </summary>        
        public string OrderCode { get; set; }
        /// <summary>
        /// 中文全称
        /// </summary>        
        public string SensorChineseFullname { get; set; }
        /// <summary>
        /// 字节中文名
        /// </summary>        
        public string ByteChineseName { get; set; }
        /// <summary>
        /// 字节英文名
        /// </summary>        
        public string ByteEnglishName { get; set; }
        /// <summary>
        /// 字节开始位置
        /// </summary>
        public int LocationBegin { get; set; }
        /// <summary>
        /// 字节结束位置
        /// </summary>        
        public int LocationEnd { get; set; }
        /// <summary>
        /// 字节默认值
        /// </summary>        
        public string DefaultValue { get; set; }
        /// <summary>
        /// 字节域描述
        /// </summary>        
        public string DataAreaDec { get; set; }
        /// <summary>
        /// 数据描述
        /// </summary>        
        public string DataDesc { get; set; }
        /// <summary>
        /// 公式
        /// </summary>        
        public string Formula { get; set; }
        /// <summary>
        /// 其它描述
        /// </summary>        
        public string OtherDescription { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>        
        public string CreatedDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>       
        public string Operator { get; set; }
        /// <summary>
        /// sn号
        /// </summary>        
        public string sn { get; set; }
    }
}
