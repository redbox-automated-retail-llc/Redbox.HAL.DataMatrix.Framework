using System.Runtime.InteropServices;

namespace ClearImage
{
    [CoClass(typeof(CiImageClass))]
    [Guid("F2BCF189-0B27-11D4-B5F5-9CC767000000")]
    [ComVisible(true)]
    [ComImport]
    public interface CiImage : ICiImage, _ICiImageEvents_Event
    {
    }
}
