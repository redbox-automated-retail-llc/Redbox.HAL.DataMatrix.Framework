using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal class NonDataPacket : CortexPacket
    {
        protected override void ParseData()
        {
            this.RawPacketType = Encoding.ASCII.GetChars(this.Packet, 21, 1)[0];
            if (this.DataSize == (ushort)0)
                return;
            for (int index = 22; this.Packet[index] != (byte)4; ++index)
                this.PacketData.Add(this.Packet[index]);
        }

        internal override PacketType PacketType => PacketType.Response;

        internal NonDataPacket(CortexPacket packet)
          : base(packet)
        {
        }

        internal NonDataPacket(byte[] packet, ushort dataSize)
          : base(packet, dataSize)
        {
        }
    }
}
