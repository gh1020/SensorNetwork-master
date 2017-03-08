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
    [Table("tProtocolResolveRule"), BsonIgnoreExtraElements(Inherited = true)]
    public class ProtocolResolveRule
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        /// <summary>
        /// UID前16位
        /// </summary>
        [Index]
        public string UIDPrefix { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>
        [Index]
        public string FunctionDescription { get; set; }

        /// <summary>
        /// 获取数据的指令
        /// </summary>
        [Index]
        public string GetDataCMD { get; set; }
        /// <summary>
        /// 数据1的位置
        /// </summary>
        [Index]
        public string TestResult1Postion { get; set; }
        /// <summary>
        /// 数据1的描述
        /// </summary>
        [Index]
        public string TestResult1Desc { get; set; }
        /// <summary>
        /// 数据1的计算公式
        /// </summary>
        [Index]
        public string TestResult1Formula { get; set; }

        /// <summary>
        /// 数据2的位置
        /// </summary>
        [Index]
        public string TestResult2Postion { get; set; }
        /// <summary>
        /// 数据2的描述
        /// </summary>
        [Index]
        public string TestResult2Desc { get; set; }
        /// <summary>
        /// 数据2的计算公式
        /// </summary>
        [Index]
        public string TestResult2Formula { get; set; }

        /// <summary>
        /// 数据3的位置
        /// </summary>
        [Index]
        public string TestResult3Postion { get; set; }
        /// <summary>
        /// 数据3的描述
        /// </summary>
        [Index]
        public string TestResult3Desc { get; set; }
        /// <summary>
        /// 数据3的计算公式
        /// </summary>
        [Index]
        public string TestResult3Formula { get; set; }

        /// <summary>
        /// 数据4的位置
        /// </summary>
        [Index]
        public string TestResult4Postion { get; set; }
        /// <summary>
        /// 数据4的描述
        /// </summary>
        [Index]
        public string TestResult4Desc { get; set; }
        /// <summary>
        /// 数据4的计算公式
        /// </summary>
        [Index]
        public string TestResult4Formula { get; set; }

        /// <summary>
        /// 数据5的位置
        /// </summary>
        [Index]
        public string TestResult5Postion { get; set; }
        /// <summary>
        /// 数据5的描述
        /// </summary>
        [Index]
        public string TestResult5Desc { get; set; }
        /// <summary>
        /// 数据5的计算公式
        /// </summary>
        [Index]
        public string TestResult5Formula { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Index]
        public string CreateTime { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        [Index]
        public string Operator { get; set; }
        /// <summary>
        /// sn
        /// </summary>
        [Index]
        public string sn { get; set; }

    }
}
