using Redbox.HAL.Component.Model;
using Redbox.HAL.Component.Model.Timers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class GetImageCommand : TextCommand
    {
        private readonly ImagePacket ImagePacket;
        private readonly string ImageName;
        private byte[] ImageBytes;

        protected override string Prefix => "^";

        protected override string Data => this.ImageName;

        protected override int ReadTimeout => 10000;

        protected override bool OnValidateResponse()
        {
            return this.Descriptor != null && this.ImageBytes != null;
        }

        protected override bool OnTest(byte[] array)
        {
            if (this.Descriptor == null)
            {
                CortexPacket packet = PacketFactory.Instance.Parse(array).Find((Predicate<CortexPacket>)(each => each.RawPacketType == 'g'));
                if (packet != null)
                {
                    this.Descriptor = new ImageDescriptorPacket(packet);
                    if (this.LogDetails)
                        LogHelper.Instance.Log(" [GetImageCommand] Process 'g' packet ( ImageSize = {0} ) packet size = {1}.", (object)this.Descriptor.ImageSize, (object)packet.Length);
                }
                return false;
            }
            if (array.Length < this.Descriptor.ImageSize + 37 + this.Descriptor.Length)
                return false;
            List<CortexPacket> packets = PacketFactory.Instance.Parse(array);
            if (packets.Count == 0 || packets[0].RawPacketType != 'g' || packets[packets.Count - 1].RawPacketType != 'd')
                return false;
            List<CortexPacket> all = packets.FindAll((Predicate<CortexPacket>)(each => each.PacketType == PacketType.Data));
            this.ImageBytes = new byte[this.Descriptor.ImageSize];
            int offset = 0;
            Action<CortexPacket> action = (Action<CortexPacket>)(each =>
            {
                Array.Copy((Array)each.PayloadData, 0, (Array)this.ImageBytes, offset, each.PayloadData.Length);
                offset += each.PayloadData.Length;
            });
            all.ForEach(action);
            this.DumpPackets(packets);
            return true;
        }

        protected override void OnDispose(bool disposing) => this.ImageBytes = (byte[])null;

        internal ImageDescriptorPacket Descriptor { get; private set; }

        internal bool CreateImage(string fileName)
        {
            if (this.ImageBytes != null && this.Descriptor != null)
            {
                if (this.ImageBytes.Length == this.Descriptor.ImageSize)
                {
                    try
                    {
                        using (ExecutionTimer executionTimer = new ExecutionTimer())
                        {
                            using (Image image = Image.FromStream((Stream)new MemoryStream(this.ImageBytes)))
                                image.Save(fileName, ImageFormat.Jpeg);
                            executionTimer.Stop();
                            LogHelper.Instance.Log("[GetImageCommand] image construction time = {0}ms", (object)executionTimer.ElapsedMilliseconds);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Log(string.Format("[GetImageCommand] Exception: unable to construct image '{0}'. Image bytes = {1} ( expected {2} )", (object)fileName, (object)this.ImageBytes.Length, (object)this.Descriptor.ImageSize), ex);
                        return false;
                    }
                }
            }
            LogHelper.Instance.Log("[GetImageCommand] Unable to construct image. Imag bytes = {0} ( expected {1} )", (object)this.ImageBytes.Length, (object)this.Descriptor.ImageSize);
            return false;
        }

        internal GetImageCommand(ImagePacket packet)
        {
            this.ImagePacket = packet;
            this.ImageName = packet.ImageName;
        }
    }
}
