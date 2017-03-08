using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol
{
    public interface IProtocol
    {
        /// <summary>
        /// 设置网络传输控制
        /// </summary>
        /// <param name="transmitter"></param>
        void SetNetworkTransmitter(INetworkTransmitter transmitter);
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool IsMatch(string uid);
        /// <summary>
        /// 查询温度
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        double GetTemperature(string uid);
        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        double SetTemperature(string uid,double value);
        /// <summary>
        /// 判断是否在线
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool IsOnline(string uid);

        string Name { get; }

        int Version { get; }
    }
}
