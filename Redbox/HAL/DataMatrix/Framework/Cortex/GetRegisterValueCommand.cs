using Redbox.HAL.Component.Model;
using System;
using System.Globalization;
using System.Text;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class GetRegisterValueCommand : TextCommand
    {
        private readonly CortexRegister Register;

        protected override string Prefix => "G";

        protected override string Data => string.Format("({0})", (object)this.Register.Register);

        internal bool IsCorrectlySet => this.OnCheckExpected();

        internal GetRegisterValueCommand(CortexRegister register) => this.Register = register;

        private bool OnCheckExpected()
        {
            CortexPacket[] parsedPackets = this.ParsedPackets;
            if (parsedPackets.Length != 0 && parsedPackets[0].RawPacketType == 'd')
            {
                int num = int.Parse(this.Register.Value);
                string s = Encoding.ASCII.GetString(parsedPackets[0].PayloadData, 0, 8);
                try
                {
                    if (int.Parse(s, NumberStyles.HexNumber) == num)
                        return true;
                    LogHelper.Instance.Log("[GetRegisterValue] register {0} expected value {1} found {2}", (object)this.Register.Register, (object)this.Register.Value, (object)s);
                    return false;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(string.Format("[GetRegisterValue] caught an unhandled exception for register {0} ( payload = {1} )", (object)this.Register, (object)s), ex);
                    return false;
                }
            }
            else
            {
                LogHelper.Instance.Log("The response packet for GetRegister is malformed ( found '{0}' packet ).", parsedPackets.Length == 0 ? (object)"nil" : (object)parsedPackets[0].RawPacketType.ToString());
                return false;
            }
        }
    }
}
