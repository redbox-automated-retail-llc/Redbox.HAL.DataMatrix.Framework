using Redbox.HAL.Component.Model;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class NilCameraPlugin : ICameraPlugin
    {
        public bool Start() => false;

        public void Snap(string file)
        {
        }

        public bool Stop() => false;

        public void InitWithProperties(IDictionary<string, object> props)
        {
        }

        public bool IsRunning => false;

        public bool SupportsReset => false;
    }
}
