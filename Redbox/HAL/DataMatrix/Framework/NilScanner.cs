using Redbox.HAL.Component.Model;
using System;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class NilScanner : IScannerDevice, IDisposable
    {
        public void Dispose()
        {
        }

        public bool TestConfiguration(IMessageSink sink) => false;

        public bool Start() => false;

        public bool Start(bool unused) => false;

        public bool Stop() => false;

        public bool Restart() => false;

        public IScanResult Scan() => (IScanResult)new NilScanResult("UNCONFIGURED DEVICE");

        public string FindInstalledPort() => "NONE";

        public ISnapResult Snap() => (ISnapResult)new SnapResult((string)null);

        public ISnapResult Snap(string fileName) => (ISnapResult)new SnapResult(fileName);

        public bool ValidateSettings() => this.ValidateSettings((IMessageSink)null);

        public bool ValidateSettings(IMessageSink sink) => false;

        public bool RequiresExternalLighting => false;

        public bool IsConnected => false;

        public bool InError => true;

        public bool IsLegacy => true;

        public bool SupportsSecureReads => false;

        public ScannerServices Service => ScannerServices.Emulated;

        public CameraGeneration CurrentCameraGeneration => CameraGeneration.Unknown;

        internal NilScanner()
        {
        }
    }
}
