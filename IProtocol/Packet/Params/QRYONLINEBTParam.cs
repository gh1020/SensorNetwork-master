using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.Params
{
    /// <summary>
    /// 5.4.8	已连接的蓝牙设备列表
    /// </summary>
    public class QRYONLINEBTParam : BytesConverter
    {

        public List<BluetoothDevice> Items { get; set; }


        public override bool Parse(BinaryReader br)
        {
            Items = new List<BluetoothDevice>();
            // byte len = br.ReadByte();
            while (br.GetLength() > 56)
            {
                var t = new BluetoothDevice(br);
                Items.Add(t);
            }
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            if (Items?.Count > 0)
            {
                foreach (var v in Items)
                    v.ToBytes(bw);
            }
            return true;
        }
    }

    public class BluetoothDevice
    {
        public string UID { get; set; }
        public string Name { get; set; }

        public BluetoothDevice() { }

        public BluetoothDevice(BinaryReader br)
        {
            Parse(br);
        }

        public bool Parse(BinaryReader br)
        {
            UID = br.ReadStingWithFixLength(24);
            Name = br.ReadStingWithFixLength(32);
            return true;

        }


        public bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(UID, 24);
            bw.WriteStingWithFixLength(Name, 32);
            return true;
        }
    }
}
