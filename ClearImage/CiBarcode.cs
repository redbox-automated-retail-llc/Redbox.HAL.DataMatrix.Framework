using System.Runtime.InteropServices;

namespace ClearImage
{
    [Guid("4ED88240-0BE1-11D4-B5F6-009FC6000000")]
    [CoClass(typeof(CiBarcodeClass))]
    [ComVisible(true)]
    [ComImport]
    public interface CiBarcode : ICiBarcode
    {
    }
}
