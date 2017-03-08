using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace io.nulldata.SensorNetwork.Data.Models
{
    /// <summary>
    /// 网关
    /// </summary>
    [Table("tGateway"), BsonIgnoreExtraElements(Inherited = true)]
    public class Gateway
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [Index]
        public string GatewayID { get; set; }
        public string GatewayName { get; set; }
        public string Manufacturer { get; set; }
        public string ProductionDate { get; set; }
        public string IsValid { get; set; }
        public string Description { get; set; }
        public string ApplyCloudPlatformCode { get; set; }
        public string ApplyCloudPlatformName { get; set; }
        public string CreateTime { get; set; }
        public string Operator { get; set; }
        public string sn { get; set; }


        /// <summary>
        /// 应用用户id
        /// </summary>
        [Index]
        public string app_user_id { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// port
        /// </summary>
        public int? port { get; set; }
        /// <summary>
        /// mac
        /// </summary>
        public string mac { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime last_updated { get; set; }
    }
}
