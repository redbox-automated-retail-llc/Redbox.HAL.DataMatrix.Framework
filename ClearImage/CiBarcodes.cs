using System.Runtime.InteropServices;

namespace ClearImage
{
    [Guid("41F7F4D4-9FC1-46C6-92E5-2A3457CE3D5E")]
    [CoClass(typeof(CiBarcodesClass))]
    [ComVisible(true)]
    [ComImport]
    public interface CiBarcodes : ICiBarcodes
    {
    }
}
