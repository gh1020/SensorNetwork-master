using IM.Data.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Data
{
    /// <summary>
    /// 数据库访问实体
    /// </summary>
    public class ApplicationDbContext : MongoDBContext<ApplicationDbContext>
    {
        private static ApplicationDbContext instance;

        /// <summary>
        /// 构造
        /// </summary>
        protected ApplicationDbContext()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connectionString"></param>
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        public override void OnInitialization()
        {
            
        }

        /// <summary>
        /// 返回默认的
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ApplicationDbContext Default()
        {
            if (instance == null)
                instance = new ApplicationDbContext();
            return instance;
        }
    }
}
