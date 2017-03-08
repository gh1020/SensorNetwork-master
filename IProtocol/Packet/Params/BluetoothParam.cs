using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.4.7	蓝牙设备查询
    /// </summary>
    public class BluetoothParam : BytesConverter
    {

        public List<BluetoothConfig> Items { get; set; }


        public override bool Parse(BinaryReader br)
        {
            Items = new List<BluetoothConfig>();
            while (br.GetLength()>38)
            {
                var t= new BluetoothConfig(br);
                Items.Add(t);
            }
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
           if(Items?.Count>0)
            {
                foreach (var v in Items)
                    v.ToBytes(bw);
            }
            return true;
        }
    }

    public class BluetoothConfig
    {
        public string Name { get; set; }
        public string MAC { get; set; }

        public BluetoothConfig() { }

        public BluetoothConfig(BinaryReader br)
        {
            Parse(br);
        }

        public bool Parse(BinaryReader br)
        {
            Name = br.ReadStingWithFixLength(32);
            MAC = br.ReadStingWithFixLength(6);
            return true;

        }


        public bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Name, 32);
            bw.WriteStingWithFixLength(MAC, 6);
            return true;
        }
    }
}
