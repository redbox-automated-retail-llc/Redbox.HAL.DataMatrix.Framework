namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class DataPacket : CortexPacket
    {
        protected override void ParseData()
        {
            for (int index = 0; index < (int)this.DataSize; ++index)
                this.PacketData.Add(this.Packet[index + 15]);
        }

        internal DataPacket(byte[] packet, ushort dataSize)
          : base(packet, dataSize)
        {
        }

        internal override PacketType PacketType => PacketType.Data;
    }
}
