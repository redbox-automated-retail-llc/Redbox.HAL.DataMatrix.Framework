using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class InformationPacket : NonDataPacket
    {
        protected override void ParseData()
        {
            this.RawPacketType = Encoding.ASCII.GetChars(this.Packet, 21, 1)[0];
            this.FirmwareVersion = Encoding.ASCII.GetString(this.Packet, 22, 4);
            this.SerialNumber = Encoding.ASCII.GetString(this.Packet, 34, 10);
            this.State = Encoding.ASCII.GetString(this.Packet, 44, 1);
            this.OEMIdentifier = Encoding.ASCII.GetString(this.Packet, 45, 2);
            if (this.Packet[70] != (byte)9)
                return;
            this.DecoderVersion = Encoding.ASCII.GetString(this.Packet, 71, this.Packet.Length - 4 - 70);
        }

        internal InformationPacket(CortexPacket packet)
          : base(packet)
        {
        }

        internal override PacketType PacketType => PacketType.Information;

        internal string FirmwareVersion { get; private set; }

        internal string SerialNumber { get; private set; }

        internal string OEMIdentifier { get; private set; }

        internal string DecoderVersion { get; private set; }

        internal string State { get; private set; }
    }
}
