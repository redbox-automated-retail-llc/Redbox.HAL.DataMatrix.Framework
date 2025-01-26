using Redbox.HAL.Component.Model;
using Redbox.HAL.Component.Model.Timers;
using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal abstract class AbstractCortexCommand : IDisposable
    {
        protected readonly List<CortexPacket> Packets = new List<CortexPacket>();
        protected readonly bool LogDetails = BarcodeConfiguration.Instance.LogDetailedScan;
        private readonly byte[] Prefix = new byte[4]
        {
      (byte) 238,
      (byte) 238,
      (byte) 238,
      (byte) 238
        };
        private byte[] m_preppedCommand;
        private bool Disposed;

        public void Dispose() => this.Dispose(true);

        public byte[] Command
        {
            get
            {
                if (this.m_preppedCommand == null)
                    this.m_preppedCommand = this.PrepCommand(this.CoreCommand);
                return this.m_preppedCommand;
            }
        }

        public CortexPacket[] ParsedPackets => this.Packets.ToArray();

        public void Send(ICommPort port)
        {
            port.ValidateResponse = new Predicate<IChannelResponse>(this.OnTest);
            if (port != null && port.IsOpen)
            {
                using (ExecutionTimer executionTimer = new ExecutionTimer())
                {
                    if (!port.SendRecv(this.Command, this.ReadTimeout).CommOk)
                    {
                        LogHelper.Instance.Log("[Cortex Service] Communication error with the device.");
                        this.PortError = true;
                    }
                    else if (!this.OnValidateResponse())
                        this.PortError = true;
                    executionTimer.Stop();
                    this.ExecutionTime = executionTimer.Elapsed;
                }
            }
            else
                LogHelper.Instance.Log("[Cortex Service] The port is not open sending a command.");
        }

        protected virtual bool CoreValidate() => true;

        protected virtual bool CoreProcess()
        {
            foreach (CortexPacket packet in this.Packets)
            {
                if (packet.PacketType == PacketType.Incomplete)
                    return false;
                if (packet.RawPacketType == 'd')
                    return true;
            }
            return false;
        }

        protected virtual void OnDispose(bool disposing)
        {
        }

        protected bool OnTest(IChannelResponse ichannelResponse_0)
        {
            return this.OnTest(ichannelResponse_0.RawResponse);
        }

        protected virtual bool OnTest(byte[] array)
        {
            this.Packets.Clear();
            List<CortexPacket> collection = PacketFactory.Instance.Parse(array);
            if (collection.Count == 0 || collection.FindAll((Predicate<CortexPacket>)(each => each.PacketType == PacketType.Incomplete)).Count > 0)
                return false;
            this.Packets.AddRange((IEnumerable<CortexPacket>)collection);
            return this.CoreProcess();
        }

        protected virtual bool OnValidateResponse()
        {
            if (this.Packets.Count == 0)
            {
                LogHelper.Instance.Log("[Cortex Commands] Validate response found no response packets.");
                return false;
            }
            foreach (CortexPacket packet in this.Packets)
            {
                if (packet.PacketType == PacketType.Incomplete)
                {
                    LogHelper.Instance.Log("[Cortex Commands] Validate response found an incomplete packet.");
                    return false;
                }
            }
            return this.CoreValidate();
        }

        protected void DumpPackets(List<CortexPacket> packets)
        {
            if (!this.LogDetails)
                return;
            LogHelper.Instance.Log(" [AbstractCortexCommand] packet statistics ");
            LogHelper.Instance.Log("  Packet received count: {0}", (object)packets.Count);
            packets.ForEach((Action<CortexPacket>)(cortexPacket_0 => LogHelper.Instance.Log("  packet type {0} raw {1} size {2}", (object)cortexPacket_0.PacketType, (object)cortexPacket_0.RawPacketType, (object)cortexPacket_0.Length)));
        }

        protected virtual int ReadTimeout => 2000;

        protected abstract byte[] CoreCommand { get; }

        internal bool PortError { get; private set; }

        internal TimeSpan ExecutionTime { get; private set; }

        private bool Test(IChannelResponse ichannelResponse_0) => this.OnTest(ichannelResponse_0);

        private byte[] PrepCommand(byte[] baseCommand)
        {
            List<byte> byteList = new List<byte>();
            try
            {
                byteList.AddRange((IEnumerable<byte>)this.Prefix);
                ushort initialValue = 0;
                for (int index = 0; index < baseCommand.Length; ++index)
                {
                    initialValue = CRC16.CRC(initialValue, baseCommand[index]);
                    byteList.Add(baseCommand[index]);
                }
                byte num1 = (byte)((int)initialValue >> 8 & (int)sbyte.MaxValue);
                byteList.Add(num1);
                byte num2 = (byte)((uint)initialValue & (uint)sbyte.MaxValue);
                byteList.Add(num2);
                return byteList.ToArray();
            }
            finally
            {
                byteList.Clear();
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;
            this.Disposed = true;
            this.OnDispose(disposing);
            this.Packets.Clear();
        }
    }
}
