using System.Collections.Generic;
using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal class ImagePacket : CortexPacket
    {
        protected const int DataStart = 22;
        protected string m_imageName;

        protected override void ParseData()
        {
            List<byte> byteList = new List<byte>();
            int index = 22;
            while (this.Packet[index] != (byte)4)
                byteList.Add(this.Packet[index++]);
            this.m_imageName = Encoding.ASCII.GetString(byteList.ToArray());
        }

        internal string ImageName => this.m_imageName;

        internal ImagePacket(CortexPacket packet)
          : base(packet)
        {
        }
    }
}
