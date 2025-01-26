using Redbox.HAL.Component.Model;
using System;
using System.Collections.Generic;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class ScanResult : IScanResult, IDisposable
    {
        private readonly ISnapResult SnapResult;
        private readonly List<DecodeResult> RawDecodeResults = new List<DecodeResult>();
        private bool Disposed;
        private bool DeleteOnDispose;

        public void Dispose()
        {
            if (this.Disposed)
                return;
            this.Disposed = true;
            if (this.DeleteOnDispose && this.DecodeResults.Count > 0 && this.SnapResult != null)
                this.SnapResult.Dispose();
            this.RawDecodeResults.Clear();
        }

        public bool DeviceError { get; internal set; }

        public TimeSpan ExecutionTime { get; internal set; }

        public List<IDecodeResult> DecodeResults
        {
            get
            {
                List<IDecodeResult> list_0 = new List<IDecodeResult>();
                this.RawDecodeResults.ForEach((Action<DecodeResult>)(each => list_0.Add((IDecodeResult)each)));
                return list_0;
            }
        }

        public string ImagePath { get; private set; }

        internal void ResetOnException()
        {
            this.ExecutionTime = new TimeSpan();
            this.RawDecodeResults.Clear();
        }

        internal void Add(string code)
        {
            if (code.Equals("redbox", StringComparison.CurrentCultureIgnoreCase))
                return;
            DecodeResult decodeResult = this.RawDecodeResults.Find((Predicate<DecodeResult>)(each => each.Matrix == code));
            if (decodeResult == null)
                this.RawDecodeResults.Add(new DecodeResult(code));
            else
                decodeResult.IncrementCount();
        }

        internal ScanResult(ISnapResult isnapResult_0)
          : this(isnapResult_0.Path)
        {
            this.SnapResult = isnapResult_0;
            this.ImagePath = isnapResult_0.Path;
        }

        internal ScanResult(string imagePath)
        {
            this.ExecutionTime = new TimeSpan();
            this.ImagePath = imagePath;
        }

        internal ScanResult()
          : this(string.Empty)
        {
        }
    }
}
