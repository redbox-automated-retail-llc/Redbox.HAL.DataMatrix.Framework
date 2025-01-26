using ClearImage;
using RBDM;
using Redbox.HAL.Component.Model;
using Redbox.HAL.Component.Model.Timers;
using System;
using System.Runtime.InteropServices;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class ClearImageReaderV2 : AbstractBarcodeReader
    {
        private readonly clsRBDM TheReader;

        protected override void OnScan(ScanResult result)
        {
            if (!this.IsLicensed)
            {
                LogHelper.Instance.Log("This version isn't licensed.");
                result.ResetOnException();
            }
            else
            {
                int maxBcIn = 6;
                for (int index = 0; index < 2; ++index)
                {
                    bool flag = false;
                    try
                    {
                        this.TheReader.Image.Open(result.ImagePath);
                        using (ExecutionTimer executionTimer = new ExecutionTimer())
                        {
                            int num = this.TheReader.Find(maxBcIn);
                            executionTimer.Stop();
                            result.ExecutionTime = executionTimer.Elapsed;
                            if (num > 0)
                            {
                                foreach (CiBarcode barcode in (ICiBarcodes)this.TheReader.Barcodes)
                                    result.Add(barcode.Text);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        flag = true;
                        LogHelper.Instance.Log("[ClearImageReaderV2] Exception during scan.", ex);
                        result.ResetOnException();
                    }
                    try
                    {
                        this.TheReader.Image.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Log("[ClearImageReaderV2] Exception closing barcode image.", ex);
                    }
                    if (!flag)
                        return;
                }
                result.ResetOnException();
            }
        }

        internal ClearImageReaderV2()
          : base(BarcodeServices.Inlite)
        {
            try
            {
                int int32 = Marshal.GetHINSTANCE(this.GetType().Module).ToInt32();
                CiServer ciServer = (CiServer)new CiServerClass();
                ciServer.OpenExt(int32, 74391114, 0);
                this.TheReader = new clsRBDM()
                {
                    Ci = ciServer,
                    Image = ciServer.CreateImage()
                };
                this.IsLicensed = true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log("An unhandled exception was raised in BarcodeReader.InitializeClearImage. Calling CiServer.OpenExt to send license key failed.", ex);
                this.TheReader = (clsRBDM)null;
                this.IsLicensed = false;
            }
        }
    }
}
