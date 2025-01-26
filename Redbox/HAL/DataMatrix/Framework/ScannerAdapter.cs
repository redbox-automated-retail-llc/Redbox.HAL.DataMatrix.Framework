using Redbox.HAL.Component.Model;
using System;
using System.IO;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal abstract class ScannerAdapter : IDisposable
    {
        protected readonly IRuntimeService RuntimeService;
        private bool m_disposed;

        public void Dispose()
        {
            if (this.m_disposed)
                return;
            this.m_disposed = true;
            this.DisposeInner();
        }

        protected string GenerateSnapFileName()
        {
            string uniqueFile = this.RuntimeService.GenerateUniqueFile("jpg");
            return BarcodeConfiguration.Instance.UseRuntimePath ? Path.Combine(this.RuntimeService.RuntimePath("Video"), uniqueFile) : Path.Combine(BarcodeConfiguration.Instance.WorkingPath, uniqueFile);
        }

        protected ScannerAdapter()
        {
            this.RuntimeService = ServiceLocator.Instance.GetService<IRuntimeService>();
        }

        protected abstract void DisposeInner();
    }
}
