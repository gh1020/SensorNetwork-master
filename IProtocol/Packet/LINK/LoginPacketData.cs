using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol.Packet.LINK
{
    public class LoginPacketData : BasePacketData, IRequestPacketData
    {
        public override ServiceCode Serv_Code2 { get { return ServiceCode.LOGIN; } }

        public string Cloud_Code { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string AppUserID { get; set; }

        public override bool Parse(BinaryReader br)
        {
            Cloud_Code = br.ReadStingWithFixLength(4);
            Username = br.ReadStingWithFixLength(32);
            Password = br.ReadStingWithFixLength(16);
            AppUserID = br.ReadStingWithFixLength(16);
            return true;

        }


        public override bool ToBytes(BinaryWriter bw)
        {
            bw.WriteStingWithFixLength(Cloud_Code, 4);
            bw.WriteStingWithFixLength(Username, 32);
            bw.WriteStingWithFixLength(Password, 16);
            bw.WriteStingWithFixLength(AppUserID, 16);
            return true;
        }
    }
}
