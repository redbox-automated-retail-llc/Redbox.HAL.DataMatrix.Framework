using Redbox.HAL.Component.Model;
using System;
using System.IO;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class SnapResult : ISnapResult, IDisposable
    {
        private bool Disposed;

        public void Dispose()
        {
            if (this.Disposed)
                return;
            this.Disposed = true;
            if (!this.TestPath(this.Path))
                return;
            try
            {
                File.Delete(this.Path);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log("[SnapResult] unable to delete path '{0}'", (object)this.Path);
                LogHelper.Instance.Log(ex.Message);
            }
        }

        public string Path { get; private set; }

        public bool SnapOk { get; private set; }

        internal SnapResult(string path)
        {
            this.SnapOk = this.TestPath(path);
            this.Path = this.SnapOk ? path : string.Empty;
        }

        private bool TestPath(string path) => !string.IsNullOrEmpty(path) && File.Exists(path);
    }
}
