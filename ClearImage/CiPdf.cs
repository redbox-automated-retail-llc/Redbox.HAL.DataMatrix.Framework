using System.Runtime.InteropServices;

namespace ClearImage
{
    [Guid("70A6F899-6298-447E-951C-07430C0FF812")]
    [CoClass(typeof(CiPdfClass))]
    [ComVisible(true)]
    [ComImport]
    public interface CiPdf : ICiPdf
    {
    }
}
