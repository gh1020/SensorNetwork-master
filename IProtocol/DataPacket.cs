using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Protocol
{
    public enum UartCMD
    {
        NullCmd = 0,

    }

    public class TPKGHead : BytesConverter
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int index = 0;
        public const ushort Tag = 0xaaff;
        /// <summary>
        /// 命令id
        /// </summary>
        public uint CommandId { get { return (uint)(((ushort)Serv_Type << 16) | Serv_Code); } }

        public void SetNextSessionId()
        {
            SessionId = (uint)System.Threading.Interlocked.Increment(ref index);
            Flags |= TPKGHeadFlags.SIDV;
        }

        /// <summary>
        /// 报文序列号，步长为1，递增
        /// </summary>
        public uint Seq_Id { get; set; }
        /// <summary>
        /// 通信类型，0：TCP 1:UDP
        /// </summary>
        public CommunicationType Comm_Type { get; set; }
        /// <summary>
        /// 加密类型，0：不加密
        /// </summary>
        public EncryptionType Enc_Type { get; set; }
        /// <summary>
        /// 协议主版本号
        /// </summary>
        public byte Major_Ver { get; set; }
        /// <summary>
        /// 协议次版本号
        /// </summary>
        public byte Minor_Ver { get; set; }
        /// <summary>
        /// 终端编码
        /// </summary>
        public string Term_Code { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType Serv_Type { get { return Data?.Serv_Type ?? ServiceType.None; } }
        /// <summary>
        /// 服务代码
        /// </summary>
        public ushort Serv_Code { get { return Data?.Serv_Code ?? 0; } }

        /// <summary>
        /// IP地址 4
        /// </summary>
      //  public byte[] IPAddress { get; set; }
        /// <summary>
        /// 监听端口
        /// </summary>
       // public ushort Port { get; set; }
        /// <summary>
        /// 物理地址 6
        /// </summary>
      //  public byte[] MAC_Addr { get; set; }
        /// <summary>
        /// 报文标志
        /// </summary>
        public TPKGHeadFlags Flags { get; set; } = TPKGHeadFlags.FIN | TPKGHeadFlags.FIR;
        /// <summary>
        /// 消息优先级
        /// </summary>
        public byte Priority { get; set; }
        /// <summary>
        /// 保留字段
        /// </summary>
        public uint Reserved { get; set; }


        public IPacketData Data { get; set; }

        /// <summary>
        /// 会话ID	SIDV标志=1有效
        /// </summary>
        public uint? SessionId { get; set; }
        /// <summary>
        /// 16	密码	PWV标志=1有效
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 时间戳,秒、分、时、日、月、年,TPV标志=1有效
        /// </summary>
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// 过期时间,秒，TPV标志=1有效
        /// </summary>
        public int? Expire_time { get; set; }

        public override bool ToBytes(BinaryWriter bw)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                BinaryWriter bww = new BinaryWriter(ms);
                //bww.Write(Tag);
                bww.Write(Seq_Id);
                bww.Write((byte)Comm_Type);
                bww.Write((byte)Enc_Type);
                bww.Write(Major_Ver);
                bww.Write(Minor_Ver);
                bww.WriteStingWithFixLength(Term_Code, 12);
                bww.Write((ushort)Serv_Type);
                bww.Write((ushort)Serv_Code);
               // bww.WriteBytesWithFixLength(IPAddress, 4);
               // bww.Write(Port);
               // bww.WriteBytesWithFixLength(MAC_Addr, 6);
                bww.Write((byte)Flags);
                bww.Write(Priority);
                bww.Write(Reserved);

                if (Data != null)
                    Data.ToBytes(bww);

                if (Flags.HasFlag(TPKGHeadFlags.SIDV))
                    bww.Write(SessionId ?? 0);
                if (Flags.HasFlag(TPKGHeadFlags.PWV))
                    bww.WriteStingWithFixLength(Password, 16);
                if (Flags.HasFlag(TPKGHeadFlags.TPV))
                {
                    bww.Write(new byte[] {
                        (byte)(Timestamp?.Second??0),
                        (byte)(Timestamp?.Minute??0),
                        (byte)(Timestamp?.Hour??0),
                        (byte)(Timestamp?.Day??0),
                        (byte)(Timestamp?.Minute??0),
                        (byte)(Timestamp?.Year??0),
                    });

                    bww.Write(Expire_time ?? 0);
                }


                var bytes = ms.ToArray();
                ms.Dispose();
                bw.Write(Tag);
                bw.Write((ushort)(bytes.Length + 8));
                bw.Write(Crc32.Compute(bytes));
                bw.Write(bytes);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
            }

        }

        public override bool Parse(BinaryReader br)
        {
            MemoryStream ms = null;
            BinaryReader brr = null;
            try
            {
                if (br.ReadUInt16() != Tag)
                    throw new Exception("包头标记错误");
                var len = br.ReadUInt16();
                var crc = br.ReadUInt32();
                var bytes = br.ReadBytes((int)len - 8);
                if (Crc32.Compute(bytes) != crc)
                    throw new Exception("crc32校验错误。");

                ms = new MemoryStream(bytes);
                ms.Position = 0;
                brr = new BinaryReader(ms);
                Seq_Id = brr.ReadUInt32();
                Comm_Type = (CommunicationType)brr.ReadByte();
                Enc_Type = (EncryptionType)brr.ReadByte();
                Major_Ver = brr.ReadByte();
                Minor_Ver = brr.ReadByte();
                Term_Code = brr.ReadStingWithFixLength(12);
                var Serv_Type = (ServiceType)brr.ReadUInt16();
                var Serv_Code = brr.ReadUInt16();
              //  IPAddress = brr.ReadBytes(4);
             //   Port = brr.ReadUInt16();
             //   MAC_Addr = brr.ReadBytes(6);
                Flags = (TPKGHeadFlags)brr.ReadByte();
                Priority = brr.ReadByte();
                Reserved = brr.ReadUInt32();

                Data = PacketFactory.Create(brr, Serv_Type, Serv_Code);//解析数据域，根据服务类型和服务代码确定是哪个具体功能

                if (Flags.HasFlag(TPKGHeadFlags.SIDV))//附加域
                    SessionId = brr.ReadUInt32();
                if (Flags.HasFlag(TPKGHeadFlags.PWV))
                    Password = brr.ReadStingWithFixLength(16);
                if (Flags.HasFlag(TPKGHeadFlags.TPV))
                {
                    var t = brr.ReadBytes(6);
                    Timestamp = new DateTime(t[5], t[4], t[3], t[2], t[1], t[0]);
                    Expire_time = brr.ReadInt32();
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            finally
            {
                if (brr != null)
#if WindowsCE
                    brr.Close();
#else
                    brr.Dispose();
#endif
                if (ms != null)
                    ms.Dispose();
            }
        }

    }
    /// <summary>
    /// 通讯类型
    /// </summary>
    public enum CommunicationType
    {
        TCP,
        UDP
    }
    /// <summary>
    /// 加密类型
    /// </summary>
    public enum EncryptionType
    {
        None,
    }

    [Flags]
    public enum TPKGHeadFlags
    {
        CON = 0x1,
        FIN = 0x2,
        FIR = 0x4,
        TPV = 0x8,
        PWV = 0x10,
        SIDV = 0x20
    }


}
