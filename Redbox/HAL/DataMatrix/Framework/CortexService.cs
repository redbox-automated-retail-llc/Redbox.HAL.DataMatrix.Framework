using Redbox.HAL.Component.Model;
using Redbox.HAL.DataMatrix.Framework.Cortex;
using System;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class CortexService :
      ScannerAdapter,
      IScannerDevice,
      IDisposable,
      IConfigurationObserver
    {
        private ICommPort Port;
        private string ConfiguredPortName = "NONE";
        private bool LogDetailedScan;
        private readonly CommPortReadModes Mode = CommPortReadModes.Callback;
        private readonly ICommChannelConfiguration Configuration;

        public bool Start() => this.Start(false);

        public bool Start(bool unused)
        {
            if (this.IsConnected)
                return true;
            if (this.Port != null)
            {
                if (!this.Port.Open())
                {
                    LogHelper.Instance.Log("[Cortex Service] Unable to open communcation port {0}.", (object)this.Port.DisplayName);
                    return false;
                }
            }
            else
            {
                IPortManagerService service = ServiceLocator.Instance.GetService<IPortManagerService>();
                this.Port = this.ConfiguredPortName == "NONE" ? service.Scan(this.Configuration, new Predicate<ICommPort>(this.TestPort), this.Mode) : service.Scan(this.ConfiguredPortName, this.Configuration, new Predicate<ICommPort>(this.TestPort), this.Mode);
                if (this.Port == null)
                {
                    LogHelper.Instance.Log("[Cortex Service] Unable to locate communcation port.");
                    return false;
                }
                this.ConfiguredPortName = this.Port.PortName;
            }
            new InfaredLightCommand().Send(this.Port);
            this.RuntimeService.Wait(300);
            return this.IsConnected = !this.OnScan().DeviceError;
        }

        public bool Stop()
        {
            if (!this.IsConnected)
                return true;
            if (this.Port != null)
            {
                ServiceLocator.Instance.GetService<IPortManagerService>().Dispose(this.Port);
                this.Port = (ICommPort)null;
            }
            this.IsConnected = false;
            return true;
        }

        public bool Restart()
        {
            this.Stop();
            this.RuntimeService.Wait(500);
            return this.Start();
        }

        public IScanResult Scan()
        {
            if (this.IsConnected)
                return (IScanResult)this.OnScan();
            LogHelper.Instance.Log("[Cortex Service] Scan is called without initialization.");
            ScanResult scanResult = new ScanResult();
            scanResult.ResetOnException();
            scanResult.DeviceError = true;
            return (IScanResult)scanResult;
        }

        public ISnapResult Snap()
        {
            string snapFileName = this.GenerateSnapFileName();
            if (!this.IsConnected)
            {
                LogHelper.Instance.Log("[Cortex Service] Snap: camera is not connected.");
                return (ISnapResult)new SnapResult(snapFileName);
            }
            if (BarcodeConfiguration.Instance.UseCortexHDField)
            {
                using (SetHighDensityFieldCommand densityFieldCommand = new SetHighDensityFieldCommand())
                    densityFieldCommand.Send(this.Port);
            }
            using (TakePictureCommand takePictureCommand = new TakePictureCommand())
            {
                takePictureCommand.Send(this.Port);
                if (takePictureCommand.PortError)
                {
                    LogHelper.Instance.Log("[CortexService]: There was a communication error.");
                    return (ISnapResult)new SnapResult(snapFileName);
                }
                ImagePacket imagePacket = takePictureCommand.ImagePacket;
                if (imagePacket == null)
                {
                    LogHelper.Instance.Log("[CortexService]: unable to locate image packet during snap.");
                    return (ISnapResult)new SnapResult(snapFileName);
                }
                using (GetImageCommand getImageCommand = new GetImageCommand(imagePacket))
                {
                    getImageCommand.Send(this.Port);
                    if (getImageCommand.PortError)
                    {
                        LogHelper.Instance.Log("[CortexService]: failed to read image data from device.");
                        return (ISnapResult)new SnapResult(snapFileName);
                    }
                    LogHelper.Instance.Log("[Cortex Service] Snap time: {0}ms", (object)getImageCommand.ExecutionTime.Milliseconds);
                    getImageCommand.CreateImage(snapFileName);
                }
                new DeleteFileCommand(imagePacket).Send(this.Port);
                return (ISnapResult)new SnapResult(snapFileName);
            }
        }

        public bool ValidateSettings()
        {
            return this.ValidateSettings((IMessageSink)new CortexService.ValidatorSink());
        }

        public bool ValidateSettings(IMessageSink sink)
        {
            return new CortexSettingsValidator(this, this.RuntimeService).Validate(sink);
        }

        public void NotifyConfigurationLoaded() => this.UpdateConfiguration();

        public void NotifyConfigurationChangeStart()
        {
        }

        public void NotifyConfigurationChangeEnd() => this.UpdateConfiguration();

        public bool RequiresExternalLighting => false;

        public bool IsConnected { get; private set; }

        public bool InError => false;

        public bool IsLegacy => false;

        public bool SupportsSecureReads => true;

        public ScannerServices Service => ScannerServices.Cortex;

        public CameraGeneration CurrentCameraGeneration => CameraGeneration.Fifth;

        protected override void DisposeInner() => this.Stop();

        internal CortexService()
        {
            this.Configuration = ServiceLocator.Instance.GetService<IPortManagerService>().CreateConfiguration();
            int cortexPortBufferSize = BarcodeConfiguration.Instance.CortexPortBufferSize;
            this.Configuration.ReceiveBufferSize = new int?(cortexPortBufferSize);
            LogHelper.Instance.Log("[CortexService] Receive buffer size = {0}", (object)cortexPortBufferSize);
            this.Configuration.WriteTimeout = 2000;
        }

        internal ICommPort ConfiguredPort() => this.Port;

        private void UpdateConfiguration()
        {
            this.Configuration.OpenPause = BarcodeConfiguration.Instance.CortexPortOpenWait;
            this.Configuration.WritePause = BarcodeConfiguration.Instance.WritePause;
            this.LogDetailedScan = BarcodeConfiguration.Instance.LogDetailedScan;
            this.Configuration.EnableDebug = this.LogDetailedScan;
        }

        private ScanResult OnScan()
        {
            ScanResult result = new ScanResult(this.Snap());
            using (DecodeCommand decodeCommand = new DecodeCommand())
            {
                decodeCommand.Send(this.Port);
                if (decodeCommand.PortError)
                {
                    LogHelper.Instance.Log("[Cortex Service] Scan: There was a communication error with the device.");
                    result.ResetOnException();
                    result.DeviceError = true;
                    return result;
                }
                decodeCommand.FoundCodes.ForEach((Action<string>)(each => result.Add(each)));
                result.ExecutionTime = decodeCommand.ExecutionTime;
            }
            return result;
        }

        private bool TestPort(ICommPort port)
        {
            using (GetRegisterValueCommand registerValueCommand = new GetRegisterValueCommand(new CortexRegister("21D", "1")))
            {
                registerValueCommand.Send(port);
                bool isCorrectlySet = registerValueCommand.IsCorrectlySet;
                LogHelper.Instance.Log("[Cortex Service] Test port returned {0} on port {1}", (object)isCorrectlySet, (object)port.DisplayName);
                return isCorrectlySet;
            }
        }

        private class ValidatorSink : IMessageSink
        {
            public bool Send(string message)
            {
                LogHelper.Instance.Log("[CortexService] <Validate message> {0}", (object)message);
                return true;
            }
        }
    }
}
