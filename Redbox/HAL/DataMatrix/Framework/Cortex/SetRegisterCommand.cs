namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class SetRegisterCommand : TextCommand
    {
        protected override string Prefix => "C";

        protected override string Data
        {
            get
            {
                return string.Format("({0}){1}", (object)this.Register.Register, (object)this.Register.Value);
            }
        }

        internal string Name { get; private set; }

        internal CortexRegister Register { get; private set; }

        internal SetRegisterCommand(CortexRegister register, string name)
        {
            this.Register = register;
            this.Name = name;
        }
    }
}
