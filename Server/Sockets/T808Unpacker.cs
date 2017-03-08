using SensorNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetwork.Uart.Sockets
{
    public class T808Unpacker : IM.Sockets.IUnpacker<byte[]>
    {

        public static readonly byte[] FrameTag = BitConverter.GetBytes(TPKGHead.Tag);
        public static int MIN_PACKET_LENGTH = 36;//最小包长度 = 包标志+包头+校验码的c
        private BeginFramerState state;
        private ushort PacketDataLength;

        private enum BeginFramerState
        {
            Begin = 0,
            ReadLengthField = 1,
            ReceivingData = 2,
        }


        public override IEnumerable<byte[]> TryUnpack()
        {
        start:

            lock (Buffer)
            {
                if (BufferWriteIndex - BufferReadIndex < MIN_PACKET_LENGTH)
                    yield break;

                if (state == BeginFramerState.Begin)
                {
                    for (; BufferReadIndex < BufferWriteIndex; BufferReadIndex++)
                    {
                        if (Buffer[BufferReadIndex] != FrameTag[0] || Buffer[BufferReadIndex + 1] != FrameTag[1])
                            continue;
                        while (Buffer[++BufferReadIndex] == FrameTag[0] && Buffer[BufferReadIndex + 1] == FrameTag[1]) { }
                        PacketStartIndex = BufferReadIndex - 1;
                        state = BeginFramerState.ReadLengthField;
                        break;
                    }
                }
                if (state == BeginFramerState.ReadLengthField)
                {
                    if (BufferWriteIndex - PacketStartIndex > 4)
                    {
                        PacketDataLength = (ushort)(BitConverter.ToUInt16(Buffer, PacketStartIndex + 2));
                        //PacketDataLength = (ushort)(Buffer[PacketStartIndex + 3] + (Buffer[PacketStartIndex + 4] << 8) + 4);
                        state = BeginFramerState.ReceivingData;
                    }
                }
                if (state == BeginFramerState.ReceivingData)
                {
                    if (BufferWriteIndex - PacketStartIndex >= PacketDataLength)
                    {
                        BufferReadIndex = PacketStartIndex + PacketDataLength - 1;
                        this.state = BeginFramerState.Begin;

                        var raw = new byte[PacketDataLength];
                        System.Buffer.BlockCopy(Buffer, PacketStartIndex, raw, 0, PacketDataLength);

                        var hasData = MoveData();
                        yield return raw;
                        if (hasData)
                            goto start;
                        else
                            yield break;

                    }
                }
            }
        }

    }
}
