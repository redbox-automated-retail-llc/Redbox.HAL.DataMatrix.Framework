using System.Collections.Generic;
using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal class ImageDescriptorPacket : ImagePacket
    {
        protected override void ParseData()
        {
            List<byte> byteList = new List<byte>();
            int index1 = 22;
            while (this.Packet[index1] != (byte)9)
                byteList.Add(this.Packet[index1++]);
            this.m_imageName = Encoding.ASCII.GetString(byteList.ToArray());
            int index2 = index1 + 2;
            StringBuilder stringBuilder = new StringBuilder();
            for (; this.Packet[index2] != (byte)41; ++index2)
                stringBuilder.Append((byte)((uint)this.Packet[index2] - 48U));
            this.ImageSize = int.Parse(stringBuilder.ToString());
        }

        internal int ImageSize { get; private set; }

        internal ImageDescriptorPacket(CortexPacket packet)
          : base(packet)
        {
        }
    }
}
