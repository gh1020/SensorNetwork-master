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
    /// <summary>
    /// 终端连接时间
    /// </summary>
    [Table("tTerminalConnectionLogs"), BsonIgnoreExtraElements(Inherited = true)]
    public class TerminalConnectionLog
    {
        [BsonId]
        public Guid id { get; set; }

        [Index]
        public string terminal_id { get; set; }

        /// <summary>
        /// 服务器Id
        /// </summary>
        [Index, BsonIgnoreIfNull]
        public Guid? server_id { get; set; }

        /// <summary>
        /// 回话id
        /// </summary>
        [Index, BsonIgnoreIfNull]
        public int? session { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        [Index]
        public DateTime connected { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime disconnected { get; set; }

        [BsonIgnoreIfNull]
        public string ipaddress { get; set; }
    }
}
