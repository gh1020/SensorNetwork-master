using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// 外接蓝牙设备数据
    /// </summary>
    public class BluetoothDataItem
    {
   
            /// <summary>
            /// 
            /// </summary>
 
            public string id { get; set; }

            /// <summary>
            /// 网关id
            /// </summary>
    
            public string GatewayID { get; set; }
            /// <summary>
            /// 蓝牙设备UID
            /// </summary>
     
            public string UID { get; set; }
            /// <summary>
            /// 检测结果1
            /// </summary>
 
            public double TestResult1 { get; set; }
            /// <summary>
            /// 检测结果1
            /// </summary>
       
            public string TestResult1Desc { get; set; }
            /// <summary>
            /// 检测结果2
            /// </summary>
       
            public double TestResult2 { get; set; }
            /// <summary>
            /// 检测结果1描述
            /// </summary>
       
            public string TestResult2Desc { get; set; }
            /// <summary>
            /// 检测结果3
            /// </summary>
         
            public double TestResult3 { get; set; }
            /// <summary>
            /// 检测结果3描述
            /// </summary>
        
            public string TestResult3Desc { get; set; }
            /// <summary>
            /// 检测结果4
            /// </summary>
     
            public double TestResult4 { get; set; }
            /// <summary>
            /// 检测结果4描述
            /// </summary>
      
            public string TestResult4Desc { get; set; }
            /// <summary>
            /// 检测结果5
            /// </summary>
 
            public double TestResult5 { get; set; }
            /// <summary>
            /// 检测结果5描述
            /// </summary>
  
            public string TestResult5Desc { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
         
            public DateTime Created { get; set; }
 
    }
}