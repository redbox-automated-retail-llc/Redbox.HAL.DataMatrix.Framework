using Redbox.HAL.Component.Model;
using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class NilScanResult : IScanResult, IDisposable
    {
        private readonly TimeSpan Time;
        private readonly List<IDecodeResult> Codes = new List<IDecodeResult>();

        public void Dispose()
        {
        }

        public bool DeviceError => false;

        public string HardwareErrorDescription { get; private set; }

        public List<IDecodeResult> DecodeResults => this.Codes;

        public TimeSpan ExecutionTime => this.Time;

        public string ImagePath => string.Empty;

        internal NilScanResult(string error) => this.HardwareErrorDescription = error;
    }
}
