using ClearImage;
using System;

namespace RBDM
{
    public class clsRBDM
    {
        private const int int_0 = 32;
        private const int int_1 = 64;
        private const int int_2 = 128;
        private const int int_3 = 1;
        private const int int_4 = 8192;
        private const int int_5 = 5;
        private const int int_6 = 4;
        public CiServer Ci;
        public CiImage Image;
        public CiBarcodes Barcodes;
        private CiDataMatrix ciDataMatrix_0;
        private CiTools ciTools_0;
        private CiImage ciImage_0;
        private CiAdvColor ciAdvColor_0;
        private int int_7;

        public int Find(int maxBcIn)
        {
            if (this.Ci == null)
                throw new Exception("RBDM Server is not intialized");
            if (this.Image == null)
                throw new Exception("RBDM Image is not intialized");
            this.method_0();
            int result = 0;
            try
            {
                string str = this.Ci.get_Info((EInfoType)7745, 1061);
                string s = this.Ci.get_Info((EInfoType)7746);
                result = 0;
                int.TryParse(s, out result);
                str = this.Ci.get_Info((EInfoType)7747, result ^ 1061);
                this.ciTools_0 = this.Ci.CreateTools();
                this.ciAdvColor_0 = this.Ci.CreateAdvColor();
                this.int_7 = maxBcIn;
                return this.method_5(this.Image);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.ciTools_0 = (CiTools)null;
                this.ciAdvColor_0 = (CiAdvColor)null;
                this.Ci.get_Info((EInfoType)7748, result ^ 1061);
                if (this.ciImage_0 != null)
                    this.ciImage_0.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private bool method_0()
        {
            if (this.ciDataMatrix_0 != null)
                return true;
            try
            {
                this.ciDataMatrix_0 = this.Ci.CreateDataMatrix();
                return true;
            }
            catch (Exception ex)
            {
                this.ciDataMatrix_0 = (CiDataMatrix)null;
                throw ex;
            }
        }

        private bool method_1(CiRect ciRect_0, long long_0, long long_1)
        {
            return long_0 >= (long)ciRect_0.left && long_0 <= (long)ciRect_0.right && long_1 >= (long)ciRect_0.Int32_0 && long_1 <= (long)ciRect_0.bottom;
        }

        private bool method_2(CiBarcode ciBarcode_0, CiImage ciImage_1)
        {
            foreach (ICiBarcode barcode in (ICiBarcodes)this.Barcodes)
            {
                if (this.method_1(barcode.Rect, (long)((ciBarcode_0.Rect.left + ciBarcode_0.Rect.right) / 2), (long)((ciBarcode_0.Rect.Int32_0 + ciBarcode_0.Rect.bottom) / 2)))
                    return false;
            }
            return true;
        }

        private long method_3(
          CiImage ciImage_1,
          int int_8,
          int int_9,
          int int_10,
          int int_11,
          int int_12)
        {
            switch (int_8)
            {
                case 64:
                    if (ciImage_1.BitsPerPixel > 1)
                    {
                        this.ciAdvColor_0.Image = ciImage_1;
                        this.ciAdvColor_0.ConvertToBitonal(EBiTonalConversion.ciBtcEdgeOne, int_9, int_10, int_11);
                        break;
                    }
                    break;
                case 128:
                    if (ciImage_1.BitsPerPixel > 1)
                    {
                        this.ciAdvColor_0.Image = ciImage_1;
                        this.ciAdvColor_0.ConvertToBitonal(EBiTonalConversion.ciBtcLocalThr, int_9, int_10, int_11);
                        break;
                    }
                    break;
            }
            if ((int_12 & 1) != 0)
            {
                this.ciTools_0.Image = ciImage_1;
                this.ciTools_0.Fatten(1, (EMorphDirections)8192);
            }
            this.ciDataMatrix_0.Image = ciImage_1;
            this.ciDataMatrix_0.Find(this.int_7);
            foreach (CiBarcode barcode in (ICiBarcodes)this.ciDataMatrix_0.Barcodes)
            {
                if (this.method_2(barcode, ciImage_1))
                    this.Barcodes.Add(barcode);
            }
            return (long)this.Barcodes.Count;
        }

        private int method_4(CiImage ciImage_1)
        {
            this.method_3(ciImage_1, 32, 0, 0, 0, 0);
            if (this.int_7 != 0 && this.Barcodes.Count >= this.int_7)
                return this.Barcodes.Count;
            this.method_3(ciImage_1, 128, 0, 50, 0, 0);
            if (this.int_7 != 0 && this.Barcodes.Count >= this.int_7)
                return this.Barcodes.Count;
            this.method_3(ciImage_1, 32, 0, 0, 0, 1);
            return this.int_7 != 0 && this.Barcodes.Count >= this.int_7 ? this.Barcodes.Count : this.Barcodes.Count;
        }

        private int method_5(CiImage ciImage_1)
        {
            CiImage ciImage_1_1 = (CiImage)null;
            this.Barcodes = this.Ci.CreateBarcodes();
            try
            {
                this.ciDataMatrix_0.Image = ciImage_1;
                ciImage_1_1 = ciImage_1.Duplicate();
                this.ciAdvColor_0.Image = ciImage_1_1;
                this.ciAdvColor_0.ConvertToGrayscale();
                this.method_4(ciImage_1_1);
                if (this.int_7 != 0 && this.Barcodes.Count >= this.int_7)
                    return this.Barcodes.Count;
                this.ciTools_0.Image = ciImage_1_1;
                this.ciTools_0.Skew(5.0);
                this.method_4(ciImage_1_1);
                return this.int_7 != 0 && this.Barcodes.Count >= this.int_7 ? this.Barcodes.Count : this.Barcodes.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ciImage_1_1 != null && ciImage_1_1.IsValid == EBoolean.ciTrue)
                    ciImage_1_1.Close();
                if (this.ciDataMatrix_0 != null && this.ciDataMatrix_0.Image.IsValid == EBoolean.ciTrue)
                    this.ciDataMatrix_0.Image.Close();
                if (this.ciTools_0 != null && this.ciTools_0.Image.IsValid == EBoolean.ciTrue)
                    this.ciTools_0.Image.Close();
                if (this.ciAdvColor_0 != null && this.ciAdvColor_0.Image.IsValid == EBoolean.ciTrue)
                    this.ciAdvColor_0.Image.Close();
            }
        }
    }
}
