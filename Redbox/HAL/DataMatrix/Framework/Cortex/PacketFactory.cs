using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class PacketFactory
    {
        private readonly BigEndianConverter Converter = new BigEndianConverter();
        private static PacketFactory m_instance = new PacketFactory();
        private const ushort PacketOverhead = 17;

        internal CortexPacket ParseSingle(byte[] data)
        {
            CortexPacket cortexPacket = (CortexPacket)new NilPacket((byte[])null);
            return data != null && data.Length != 0 ? this.ParseInner(data, 0) : cortexPacket;
        }

        internal List<CortexPacket> Parse(byte[] data)
        {
            List<CortexPacket> cortexPacketList = new List<CortexPacket>();
            CortexPacket inner;
            for (int index = 0; index < data.Length; index += inner.Length)
            {
                int startIndex = index;
                inner = this.ParseInner(data, startIndex);
                cortexPacketList.Add(inner);
                if (inner.PacketType == PacketType.Incomplete || inner.PacketType == PacketType.Invalid)
                    break;
            }
            return cortexPacketList;
        }

        internal static PacketFactory Instance => PacketFactory.m_instance;

        private bool IsPacketStart(byte[] data, int startIndex)
        {
            return startIndex <= data.Length && startIndex + 3 <= data.Length && data[startIndex] == (byte)1 && data[startIndex + 1] == (byte)88 && data[startIndex + 2] == (byte)82;
        }

        private bool IsNonDataPacket(byte[] data, int start)
        {
            return start <= data.Length && start + 18 <= data.Length && data[start + 15] == (byte)1 && data[start + 16] == (byte)88 && data[start + 17] == (byte)30;
        }

        private CortexPacket ParseInner(byte[] data, int startIndex)
        {
            if (startIndex + 15 > data.Length)
                return (CortexPacket)new IncompletePacket();
            if (!this.IsPacketStart(data, startIndex))
                return (CortexPacket)new NilPacket(data);
            ushort uint16 = this.Converter.ToUInt16(data, startIndex + 13);
            int length = (int)uint16 + 17;
            byte[] numArray = new byte[length];
            if (startIndex + length > data.Length)
                return (CortexPacket)new IncompletePacket();
            Array.Copy((Array)data, startIndex, (Array)numArray, 0, length);
            return !this.IsNonDataPacket(data, startIndex) ? (CortexPacket)new DataPacket(numArray, uint16) : (CortexPacket)new NonDataPacket(numArray, uint16);
        }

        private PacketFactory()
        {
        }
    }
}
