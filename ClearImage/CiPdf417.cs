using System.Runtime.InteropServices;

namespace ClearImage
{
    [Guid("8B79E556-FAD7-4339-8A8F-2C35D5C42C6E")]
    [CoClass(typeof(CiPdf417Class))]
    [ComVisible(true)]
    [ComImport]
    public interface CiPdf417 : ICiPdf417
    {
    }
}
