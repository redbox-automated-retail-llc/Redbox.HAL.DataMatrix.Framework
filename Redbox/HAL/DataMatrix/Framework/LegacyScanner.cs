using Redbox.HAL.Component.Model;
using System;
using System.Xml;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class LegacyScanner :
      ScannerAdapter,
      IScannerDevice,
      IDisposable,
      IConfigurationObserver
    {
        private ICameraPlugin Plugin;
        private CameraGeneration _currentCameraGeneration;

        public bool Start() => this.Start(false);

        public bool Start(bool test)
        {
            if (this.IsConnected)
                return true;
            try
            {
                this.InError = false;
                this.IsConnected = this.Plugin.Start();
                if (!test)
                    return this.IsConnected;
                using (ISnapResult snapResult = this.Snap())
                    return snapResult.SnapOk;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log("CameraService.Start() caught an exception.", ex);
                this.IsConnected = false;
                return false;
            }
        }

        public ISnapResult Snap()
        {
            string snapFileName = this.GenerateSnapFileName();
            this.Plugin.Snap(snapFileName);
            SnapResult snapResult = new SnapResult(snapFileName);
            if (snapResult.SnapOk)
                return (ISnapResult)snapResult;
            this.InError = true;
            return (ISnapResult)snapResult;
        }

        public bool ValidateSettings() => this.ValidateSettings((IMessageSink)null);

        public bool ValidateSettings(IMessageSink imessageSink_0) => false;

        public bool TestConfiguration(IMessageSink sink) => true;

        public bool Stop()
        {
            if (!this.IsConnected)
                return false;
            this.IsConnected = false;
            try
            {
                return this.Plugin.Stop();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log("LegacyCameraService.Stop() caught an exception.", ex);
                return false;
            }
        }

        public bool Restart()
        {
            if (!this.Plugin.SupportsReset)
                return false;
            this.Stop();
            ServiceLocator.Instance.GetService<IRuntimeService>().SpinWait(500);
            LogHelper.Instance.Log("[Legacy Camera Service] Cycle camera driver.");
            IUsbDeviceService service = ServiceLocator.Instance.GetService<IUsbDeviceService>();
            IDeviceDescriptor activeCamera = service.FindActiveCamera(false);
            if (activeCamera != null)
            {
                LogHelper.Instance.Log("Found camera {0}", (object)activeCamera.Friendlyname);
                CameraGeneration cameraGeneration = this.From(activeCamera);
                if (CameraGeneration.Third == cameraGeneration || CameraGeneration.Fourth == cameraGeneration)
                    LogHelper.Instance.Log("The running driver for the camera {0}", activeCamera.MatchDriver() ? (object)"is correct" : (object)"is not correct.");
                DeviceStatus deviceStatus = service.FindDeviceStatus(activeCamera);
                if ((deviceStatus & DeviceStatus.Found) != DeviceStatus.None && (deviceStatus & DeviceStatus.Enabled) != DeviceStatus.None)
                    LogHelper.Instance.Log(" RESET camera returned {0}", activeCamera.ResetDriver() ? (object)"OK" : (object)"FAILURE");
            }
            return this.Start();
        }

        public IScanResult Scan()
        {
            ISnapResult sr = this.Snap();
            if (sr.SnapOk)
                return ServiceLocator.Instance.GetService<IBarcodeReaderFactory>().GetConfiguredReader().Scan(sr);
            return (IScanResult)new ScanResult()
            {
                DeviceError = true
            };
        }

        public void NotifyConfigurationLoaded() => this.OnLoadPlugin();

        public void NotifyConfigurationChangeStart()
        {
        }

        public void NotifyConfigurationChangeEnd() => this.OnLoadPlugin();

        public bool RequiresExternalLighting => true;

        public bool IsConnected { get; private set; }

        public bool SupportsRestart => this.Plugin.SupportsReset;

        public bool InError { get; private set; }

        public bool IsLegacy => true;

        public bool SupportsSecureReads
        {
            get
            {
                if (!(ServiceLocator.Instance.GetService<IScannerDeviceService>().IRHardwareInstall.HasValue & ServiceLocator.Instance.GetService<IBarcodeReaderFactory>().GetConfiguredReader().Service == BarcodeServices.Cortex))
                    return false;
                return this.CurrentCameraGeneration == CameraGeneration.Fifth || this.CurrentCameraGeneration == CameraGeneration.Fourth;
            }
        }

        public ScannerServices Service => ScannerServices.Legacy;

        protected override void DisposeInner() => this.Plugin.Stop();

        public CameraGeneration CurrentCameraGeneration
        {
            get
            {
                if (this._currentCameraGeneration == CameraGeneration.Unknown)
                {
                    LogHelper.Instance.Log("[Legacy Camera Service] locate camera generation.");
                    IDeviceDescriptor activeCamera = ServiceLocator.Instance.GetService<IUsbDeviceService>().FindActiveCamera(false);
                    if (activeCamera != null)
                    {
                        LogHelper.Instance.Log("Found camera {0}", (object)activeCamera.Friendlyname);
                        this._currentCameraGeneration = this.From(activeCamera);
                    }
                }
                return this._currentCameraGeneration;
            }
        }

        internal LegacyScanner()
        {
        }

        private void OnLoadPlugin()
        {
            if (BarcodeConfiguration.Instance.ScannerService != this.Service || this.Plugin != null)
                return;
            this.LoadPlugin(BarcodeConfiguration.Instance.CameraPlugin);
        }

        private void LoadPlugin(string driver)
        {
            ICameraPlugin plugin;
            CameraPluginDescriptor pluginDescriptor = PluginLoader.Instance.LocatePlugin(driver, out plugin);
            if (pluginDescriptor == null)
            {
                LogHelper.Instance.Log(string.Format("Unable to locate plugin {0}", (object)driver), LogEntryType.Error);
                this.Plugin = (ICameraPlugin)new NilCameraPlugin();
            }
            else
            {
                LogHelper.Instance.Log(string.Format("Successfully loaded camera plugin {0}", (object)driver));
                this.Plugin = plugin;
                if (pluginDescriptor.DescriptorPath != null)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(pluginDescriptor.DescriptorPath);
                    if (xmlDocument.DocumentElement != null)
                    {
                        XmlNode xmlNode = xmlDocument.SelectSingleNode("Plugin/Properties");
                        if (xmlNode != null)
                        {
                            foreach (XmlNode childNode in xmlNode.ChildNodes)
                            {
                                if (childNode.Name.Equals("property"))
                                {
                                    string key = childNode.Attributes["name"].Value;
                                    string str = childNode.Attributes["value"].Value;
                                    XmlAttribute attribute = childNode.Attributes["type"];
                                    if (attribute == null)
                                    {
                                        BarcodeConfiguration.Instance.Properties[key] = (object)str;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Type type = Type.GetType(attribute.Value);
                                            BarcodeConfiguration.Instance.Properties[key] = Convert.ChangeType((object)str, type);
                                        }
                                        catch
                                        {
                                            BarcodeConfiguration.Instance.Properties[key] = (object)str;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.Plugin.InitWithProperties(BarcodeConfiguration.Instance.Properties);
        }

        private CameraGeneration From(IDeviceDescriptor descriptor)
        {
            if (descriptor == null)
                return CameraGeneration.Unknown;
            if (descriptor.Vendor.Equals("1871") && (descriptor.Product.Equals("0d01", StringComparison.CurrentCultureIgnoreCase) || descriptor.Product.Equals("0f01", StringComparison.CurrentCultureIgnoreCase)))
                return CameraGeneration.Fourth;
            return descriptor.Vendor.Equals("0c45", StringComparison.CurrentCultureIgnoreCase) && descriptor.Product.Equals("627b", StringComparison.CurrentCultureIgnoreCase) ? CameraGeneration.Third : CameraGeneration.Unknown;
        }
    }
}
