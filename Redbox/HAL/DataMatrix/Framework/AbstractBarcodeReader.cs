using Redbox.HAL.Component.Model;
using System;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal abstract class AbstractBarcodeReader : IBarcodeReader, IDisposable
    {
        private bool Disposed;

        public void Dispose()
        {
            if (this.Disposed)
                return;
            this.OnDispose(true);
        }

        public IScanResult Scan(string file)
        {
            ScanResult result = new ScanResult(file);
            this.OnScan(result);
            return (IScanResult)result;
        }

        public IScanResult Scan(ISnapResult isnapResult_0)
        {
            ScanResult result = new ScanResult(isnapResult_0);
            this.OnScan(result);
            return (IScanResult)result;
        }

        public bool IsLicensed { get; protected set; }

        public BarcodeServices Service { get; private set; }

        protected abstract void OnScan(ScanResult result);

        protected virtual void OnDispose(bool disposing)
        {
        }

        protected AbstractBarcodeReader(BarcodeServices barcodeServices_0)
        {
            this.Service = barcodeServices_0;
        }
    }
}
