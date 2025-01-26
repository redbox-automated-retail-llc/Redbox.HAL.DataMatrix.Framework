using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal abstract class CortexPacket
    {
        protected readonly byte[] Packet;
        protected readonly List<byte> PacketData = new List<byte>();
        protected readonly BigEndianConverter Converter = new BigEndianConverter();

        protected virtual void ParseData()
        {
        }

        protected CortexPacket()
        {
        }

        protected CortexPacket(CortexPacket cortexPacket_0)
        {
            this.Packet = new byte[cortexPacket_0.Packet.Length];
            Array.Copy((Array)cortexPacket_0.Packet, 0, (Array)this.Packet, 0, cortexPacket_0.Packet.Length);
            this.DataSize = cortexPacket_0.DataSize;
            this.ProtocolVersion = cortexPacket_0.ProtocolVersion;
            this.ID = this.Converter.ToInt32(this.Packet, 4);
            this.PacketNumber = cortexPacket_0.PacketNumber;
            this.Timestamp = cortexPacket_0.Timestamp;
            this.CRC = cortexPacket_0.CRC;
            this.ChecksumOk = cortexPacket_0.ChecksumOk;
            this.RawPacketType = cortexPacket_0.RawPacketType;
            this.ParseData();
        }

        protected CortexPacket(byte[] data) => this.Packet = data;

        protected CortexPacket(byte[] packet, ushort dataSize)
        {
            this.Packet = new byte[packet.Length];
            Array.Copy((Array)packet, 0, (Array)this.Packet, 0, packet.Length);
            this.ProtocolVersion = (int)this.Packet[3];
            this.ID = this.Converter.ToInt32(this.Packet, 4);
            this.PacketNumber = this.Packet[8];
            this.Timestamp = this.Converter.ToInt32(this.Packet, 9);
            this.DataSize = this.Converter.ToUInt16(this.Packet, 13);
            if ((int)dataSize != (int)this.DataSize)
                this.DataSize = dataSize;
            this.ParseData();
            ushort num = (ushort)((uint)(ushort)this.Packet[this.Packet.Length - 2] << 8 | (uint)this.Packet[this.Packet.Length - 1]);
            ushort initialValue = 0;
            for (int index = 0; index < this.Packet.Length - 2; ++index)
                initialValue = CRC16.CRC(initialValue, this.Packet[index]);
            this.CRC = initialValue;
            this.ChecksumOk = (int)this.CRC == (int)num;
        }

        internal virtual PacketType PacketType => PacketType.None;

        internal int ProtocolVersion { get; private set; }

        internal int ID { get; private set; }

        internal byte PacketNumber { get; private set; }

        internal int Timestamp { get; private set; }

        internal ushort DataSize { get; private set; }

        internal char RawPacketType { get; set; }

        internal ushort CRC { get; set; }

        internal bool ChecksumOk { get; private set; }

        internal byte[] PayloadData => this.PacketData.ToArray();

        internal int Length => this.Packet != null ? this.Packet.Length : 0;
    }
}
