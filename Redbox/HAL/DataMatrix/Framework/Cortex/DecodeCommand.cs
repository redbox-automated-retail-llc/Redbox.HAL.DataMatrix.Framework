using Redbox.HAL.Component.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class DecodeCommand : AbstractCortexCommand
    {
        internal readonly List<string> FoundCodes = new List<string>();
        private readonly List<int> DataPackets = new List<int>();
        private readonly byte[] m_rawCommand = new byte[4]
        {
      (byte) 36,
      (byte) 1,
      (byte) 5,
      (byte) 0
        };
        private readonly char[] Separator = new char[1] { ' ' };

        protected override bool CoreValidate()
        {
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (CortexPacket packet in this.Packets)
            {
                if (packet.RawPacketType == 'd')
                    ++num1;
                else if (packet.RawPacketType == 'r')
                    ++num2;
                else if (packet.PacketType == PacketType.Data)
                    ++num3;
            }
            if (num1 == 0)
                return false;
            return num2 > 0 || num3 > 0;
        }

        protected override bool CoreProcess()
        {
            bool flag = false;
            if (this.LogDetails)
                LogHelper.Instance.Log("[Decode Command] start.");
            try
            {
                foreach (CortexPacket packet1 in this.Packets)
                {
                    CortexPacket packet = packet1;
                    if (packet.PacketType == PacketType.Incomplete)
                        return false;
                    if (packet.PacketType == PacketType.Data)
                    {
                        string str = Encoding.ASCII.GetString(packet.PayloadData);
                        if (this.LogDetails)
                            LogHelper.Instance.Log(" [DecodeCommand] Data packet: barcode data = {0} number = {1}", (object)str, (object)packet.PacketNumber);
                        if (this.DataPackets.FindAll((Predicate<int>)(each => each == (int)packet.PacketNumber)).Count == 0)
                        {
                            this.DataPackets.Add((int)packet.PacketNumber);
                            string[] strArray = str.Split(this.Separator);
                            if ((int)Convert.ChangeType((object)strArray[2], typeof(int)) == 0)
                            {
                                this.DecodeFailure = true;
                            }
                            else
                            {
                                for (int index = 3; index < strArray.Length; ++index)
                                    this.FoundCodes.Add(strArray[index]);
                            }
                        }
                    }
                    else
                    {
                        if (this.LogDetails)
                            LogHelper.Instance.Log(" [DecodeCommand] Reponse packet '{0}'", (object)packet.RawPacketType);
                        switch (packet.RawPacketType)
                        {
                            case 'd':
                                flag = true;
                                continue;
                            case 'e':
                            case 'r':
                                this.DecodeFailure = true;
                                continue;
                            default:
                                continue;
                        }
                    }
                }
                if (!flag || this.DataPackets.Count == 0)
                    return false;
                this.DumpPackets(this.Packets);
                if (this.LogDetails)
                {
                    LogHelper.Instance.Log("  [DecodeCommand] Decode results:");
                    this.FoundCodes.ForEach((Action<string>)(code => LogHelper.Instance.Log("  Scanned code {0}", (object)code)));
                }
                return this.DataPackets.Count > 0 || this.DecodeFailure;
            }
            finally
            {
                if (this.LogDetails)
                    LogHelper.Instance.Log("[Decode Command] End.");
            }
        }

        protected override void OnDispose(bool disposing) => this.DataPackets.Clear();

        protected override byte[] CoreCommand => this.m_rawCommand;

        protected override int ReadTimeout => BarcodeConfiguration.Instance.ScanTimeout;

        internal bool DecodeFailure { get; private set; }

        internal DecodeCommand()
        {
        }
    }
}
