using Redbox.HAL.Component.Model;

namespace Redbox.HAL.DataMatrix.Framework
{
    public sealed class BarcodeReaderFactory : IBarcodeReaderFactory
    {
        private readonly IBarcodeReader NilReader = (IBarcodeReader)new Redbox.HAL.DataMatrix.Framework.NilReader();
        private ClearImageReaderV2 m_clearImageReader;
        private CortexDecoder m_cortexDecoder;
        private IBarcodeReader Reader;

        public string ImagePath => BarcodeConfiguration.Instance.WorkingPath;

        public IBarcodeReader GetConfiguredReader() => this.Reader;

        public void Initialize(ErrorList errors)
        {
            this.SetConfiguredReader();
            if (this.Reader != null)
                return;
            errors.Add(Error.NewError("D001", "No reader found", "Unable to find a licensed reader"));
        }

        public BarcodeReaderFactory()
        {
            LogHelper.Instance.Log("Barcode Reader Factory - constructor.");
        }

        private void SetConfiguredReader()
        {
            LogHelper.Instance.Log("Set Configured Reader -- get cortex decoder");
            IBarcodeReader barcodeReader = (IBarcodeReader)this.GetCortexDecoder();
            if (barcodeReader == null || !barcodeReader.IsLicensed)
            {
                LogHelper.Instance.Log("Set Configured Reader -- get inlite reader");
                barcodeReader = (IBarcodeReader)this.GetInliteReader();
            }
            if (barcodeReader == null)
                barcodeReader = this.NilReader;
            this.Reader = barcodeReader;
            LogHelper.Instance.Log("[BarcodeReaderFactory] Configured reader {0} {1} licensed.", (object)this.Reader.Service, this.Reader.IsLicensed ? (object)"is" : (object)"is not");
        }

        private CortexDecoder GetCortexDecoder()
        {
            if (this.m_cortexDecoder == null)
                this.m_cortexDecoder = new CortexDecoder();
            return this.m_cortexDecoder;
        }

        private ClearImageReaderV2 GetInliteReader()
        {
            if (this.m_clearImageReader == null)
            {
                ClearImageReaderV2 clearImageReaderV2 = new ClearImageReaderV2();
                if (clearImageReaderV2.IsLicensed)
                    this.m_clearImageReader = clearImageReaderV2;
            }
            return this.m_clearImageReader;
        }
    }
}
