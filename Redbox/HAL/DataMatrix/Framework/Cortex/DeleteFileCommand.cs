namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal sealed class DeleteFileCommand : TextCommand
    {
        private readonly string ImageName;

        protected override string Prefix => "9";

        protected override string Data => this.ImageName;

        internal DeleteFileCommand(ImagePacket packet) => this.ImageName = packet.ImageName;
    }
}
