using Redbox.HAL.Component.Model;
using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework
{
    public sealed class ScannerDeviceFactory : IScannerDeviceService, IConfigurationObserver
    {
        private IScannerDevice m_configuredDevice;
        private ScannerServices m_configuredService;
        private readonly List<IConfigurationObserver> ConfigurableScanners = new List<IConfigurationObserver>();
        private readonly CortexService CortexDevice = new CortexService();
        private readonly LegacyScanner LegacyDevice = new LegacyScanner();
        private readonly NilScanner m_nilInstance = new NilScanner();

        public IScannerDevice GetConfiguredDevice() => this.m_configuredDevice;

        public void Shutdown()
        {
            LogHelper.Instance.Log("[ScannerFactory] Shutdown");
            if (this.m_configuredDevice == null)
                return;
            this.m_configuredDevice.Stop();
        }

        public DateTime? IRHardwareInstall => Redbox.HAL.Controller.Framework.KioskConfiguration.Instance.IRHardwareInstallDate;

        public CameraGeneration CurrentCameraGeneration
        {
            get
            {
                return this.m_configuredDevice != null ? this.m_configuredDevice.CurrentCameraGeneration : CameraGeneration.Unknown;
            }
        }

        public void NotifyConfigurationLoaded()
        {
            LogHelper.Instance.Log("[ScannerFactory] Configuration load.");
            this.ConfiguredService = BarcodeConfiguration.Instance.ScannerService;
            this.ConfigurableScanners.ForEach((Action<IConfigurationObserver>)(each => each.NotifyConfigurationLoaded()));
        }

        public void NotifyConfigurationChangeStart()
        {
            LogHelper.Instance.Log("[ScannerFactory] Configuration change start.");
            this.m_configuredDevice.Stop();
            this.m_configuredDevice = (IScannerDevice)this.m_nilInstance;
            this.ConfigurableScanners.ForEach((Action<IConfigurationObserver>)(each => each.NotifyConfigurationChangeStart()));
        }

        public void NotifyConfigurationChangeEnd()
        {
            LogHelper.Instance.Log("[ScannerFactory] Configuration change end.");
            this.ConfiguredService = BarcodeConfiguration.Instance.ScannerService;
            this.ConfigurableScanners.ForEach((Action<IConfigurationObserver>)(each => each.NotifyConfigurationChangeEnd()));
            this.m_configuredDevice.Start();
        }

        public ScannerDeviceFactory(ScannerServices service) => this.ConfiguredService = service;

        internal ScannerDeviceFactory()
        {
            this.m_configuredDevice = (IScannerDevice)this.m_nilInstance;
            this.ConfigurableScanners.Add((IConfigurationObserver)this.CortexDevice);
            this.ConfigurableScanners.Add((IConfigurationObserver)this.LegacyDevice);
        }

        private ScannerServices ConfiguredService
        {
            get => this.m_configuredService;
            set
            {
                this.m_configuredService = value;
                switch (this.m_configuredService)
                {
                    case ScannerServices.Legacy:
                        this.m_configuredDevice = (IScannerDevice)this.LegacyDevice;
                        break;
                    case ScannerServices.Cortex:
                        this.m_configuredDevice = (IScannerDevice)this.CortexDevice;
                        break;
                    default:
                        this.m_configuredDevice = (IScannerDevice)this.m_nilInstance;
                        break;
                }
            }
        }
    }
}
