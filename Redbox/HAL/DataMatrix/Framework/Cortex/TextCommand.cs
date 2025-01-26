using System.Collections.Generic;
using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal abstract class TextCommand : AbstractCortexCommand
    {
        private byte[] m_command;

        protected override byte[] CoreCommand
        {
            get
            {
                if (this.m_command != null)
                    return this.m_command;
                List<byte> byteList = new List<byte>();
                try
                {
                    byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(this.Prefix));
                    if (this.Data == null)
                    {
                        byteList.Add((byte)0);
                    }
                    else
                    {
                        byteList.Add((byte)this.Data.Length);
                        byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(this.Data));
                    }
                    byteList.Add((byte)0);
                    return this.m_command = byteList.ToArray();
                }
                finally
                {
                    byteList.Clear();
                }
            }
        }

        protected abstract string Prefix { get; }

        protected abstract string Data { get; }
    }
}
