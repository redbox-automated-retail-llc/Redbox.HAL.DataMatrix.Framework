namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class TakePictureCommand : AbstractCortexCommand
    {
        private readonly byte[] m_rawCommand = new byte[4]
        {
      (byte) 36,
      (byte) 1,
      (byte) 7,
      (byte) 0
        };

        protected override bool CoreValidate()
        {
            int num = 0;
            foreach (CortexPacket packet in this.Packets)
            {
                if (packet.RawPacketType == 'm')
                    ++num;
                else if (packet.RawPacketType == 'd')
                    ++num;
            }
            return num >= 2;
        }

        protected override bool CoreProcess()
        {
            bool flag1 = false;
            bool flag2 = false;
            foreach (CortexPacket packet in this.Packets)
            {
                if (packet.RawPacketType == 'd')
                    flag1 = true;
                else if (packet.RawPacketType == 'm')
                    flag2 = true;
                else if (packet.PacketType == PacketType.Incomplete)
                {
                    flag2 = false;
                    flag1 = false;
                }
            }
            return flag1 & flag2;
        }

        protected override byte[] CoreCommand => this.m_rawCommand;

        protected override int ReadTimeout => 8000;

        internal ImagePacket ImagePacket
        {
            get
            {
                ImagePacket imagePacket = (ImagePacket)null;
                foreach (CortexPacket parsedPacket in this.ParsedPackets)
                {
                    if (parsedPacket.RawPacketType == 'm')
                        imagePacket = new ImagePacket(parsedPacket);
                }
                return imagePacket;
            }
        }
    }
}
