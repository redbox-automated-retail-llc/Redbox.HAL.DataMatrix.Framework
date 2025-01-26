using Redbox.HAL.Component.Model;
using Redbox.HAL.DataMatrix.Framework.Cortex;
using System;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class CortexSettingsValidator
    {
        private readonly IRuntimeService RuntimeService;
        private readonly CortexService Service;
        private readonly char[] Separator = new char[1] { ':' };
        private static readonly CortexRegister[] Registers = new CortexRegister[17]
        {
      new CortexRegister("93", "1"),
      new CortexRegister("26", "0"),
      new CortexRegister("d1", "2"),
      new CortexRegister("a8", "100"),
      new CortexRegister("a9", "20"),
      new CortexRegister("66", "100"),
      new CortexRegister("1b7", "0"),
      new CortexRegister("256", "2"),
      new CortexRegister("34", "5"),
      new CortexRegister("b4", "1"),
      new CortexRegister("45", "1000"),
      new CortexRegister("a5", "1000"),
      new CortexRegister("a3", "1000"),
      new CortexRegister("21D", "1"),
      new CortexRegister("42", "0"),
      new CortexRegister("08", "2"),
      new CortexRegister("1b", "6")
        };

        internal CortexSettingsValidator(CortexService service, IRuntimeService iruntimeService_0)
        {
            this.Service = service;
            this.RuntimeService = iruntimeService_0;
        }

        internal bool Validate(IMessageSink sink)
        {
            int errors = 0;
            Array.ForEach<CortexRegister>(CortexSettingsValidator.Registers, (Action<CortexRegister>)(register =>
            {
                GetRegisterValueCommand registerValueCommand = new GetRegisterValueCommand(register);
                registerValueCommand.Send(this.Service.ConfiguredPort());
                if (!registerValueCommand.IsCorrectlySet)
                {
                    ++errors;
                    sink.Send(string.Format("The register {0} received an unexpected response", (object)register.Register));
                }
                else
                    sink.Send(string.Format("Register {0} matched expected value", (object)register.Register));
                this.RuntimeService.SpinWait(200);
            }));
            return errors == 0;
        }
    }
}
