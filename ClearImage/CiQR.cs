using System.Runtime.InteropServices;

namespace ClearImage
{
    [Guid("8B79E556-FAD7-4339-8A8F-2C35D5C42C70")]
    [CoClass(typeof(CiQRClass))]
    [ComVisible(true)]
    [ComImport]
    public interface CiQR : ICiQR
    {
    }
}
