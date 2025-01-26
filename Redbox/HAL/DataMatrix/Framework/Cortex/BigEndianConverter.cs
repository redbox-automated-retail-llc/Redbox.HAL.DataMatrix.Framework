namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class BigEndianConverter
    {
        internal int ToInt32(byte[] buffer, int startIndex) => this.Read(buffer, startIndex, 4);

        internal short ToInt16(byte[] buffer, int startIndex)
        {
            return (short)this.Read(buffer, startIndex, 2);
        }

        internal ushort ToUInt16(byte[] buffer, int startIndex)
        {
            return (ushort)this.Read(buffer, startIndex, 2);
        }

        internal BigEndianConverter()
        {
        }

        private int Read(byte[] buffer, int startIndex, int bytesToConvert)
        {
            int num = 0;
            for (int index = 0; index < bytesToConvert; ++index)
                num = num << 8 | (int)buffer[startIndex + index];
            return num;
        }
    }
}
