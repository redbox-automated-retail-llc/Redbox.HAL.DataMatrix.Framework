namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class CortexRegister
    {
        internal string Register { get; private set; }

        internal string Value { get; private set; }

        internal CortexRegister(string string_0, string string_1)
        {
            this.Register = string_0;
            this.Value = string_1;
        }
    }
}
