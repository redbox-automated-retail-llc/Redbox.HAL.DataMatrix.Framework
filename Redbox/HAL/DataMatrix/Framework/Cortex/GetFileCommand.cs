using Redbox.HAL.Component.Model;
using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class GetFileCommand : TextCommand
    {
        private readonly string ImageName;
        private readonly List<byte> ImageData = new List<byte>();
        private readonly List<int> ProcessedPackets = new List<int>();

        protected override bool CoreValidate()
        {
            return this.Descriptor != null && this.ImageBytes.Length == this.Descriptor.ImageSize;
        }

        protected override bool CoreProcess()
        {
            if (this.LogDetails)
                LogHelper.Instance.Log("[GetFileCommand] CoreProcess");
            try
            {
                foreach (CortexPacket packet1 in this.Packets)
                {
                    CortexPacket packet = packet1;
                    if (packet.PacketType == PacketType.Data)
                    {
                        if (-1 == this.ProcessedPackets.FindIndex((Predicate<int>)(each => each == (int)packet.PacketNumber)))
                        {
                            if (this.LogDetails)
                                LogHelper.Instance.Log(" [GetFileCommand] Process data packet ( data length = {0} )", (object)packet.PayloadData.Length);
                            this.ImageData.AddRange((IEnumerable<byte>)packet.PayloadData);
                            this.ProcessedPackets.Add((int)packet.PacketNumber);
                        }
                    }
                    else
                    {
                        if (packet.PacketType == PacketType.Incomplete)
                            return false;
                        if (packet.RawPacketType == 'g')
                        {
                            if (this.Descriptor == null)
                                this.Descriptor = new ImageDescriptorPacket(packet);
                            if (this.LogDetails)
                                LogHelper.Instance.Log(" [GetFileCommand] Process 'g' packet ( ImageSize = {0} ) packet size = {1}.", (object)this.Descriptor.ImageSize, (object)packet.Length);
                        }
                    }
                }
                return this.Descriptor != null && this.ImageData.Count == this.Descriptor.ImageSize;
            }
            finally
            {
                if (this.LogDetails)
                    LogHelper.Instance.Log("[GetFileCommand] End.");
            }
        }

        protected override string Prefix => "^";

        protected override string Data => this.ImageName;

        protected override int ReadTimeout => 10000;

        internal byte[] ImageBytes => this.ImageData.ToArray();

        internal ImageDescriptorPacket Descriptor { get; private set; }

        internal GetFileCommand(ImagePacket packet) => this.ImageName = packet.ImageName;
    }
}
