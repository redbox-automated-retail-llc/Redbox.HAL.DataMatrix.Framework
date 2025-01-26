using System.Runtime.InteropServices;

namespace ClearImage
{
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class _ICiImageEvents_SinkHelper : _ICiImageEvents
    {
        public _ICiImageEvents_LogEventHandler m_LogDelegate;
        public int m_dwCookie;

        public virtual void Log(string LogSignature, string LogRecord, string RsltRecord)
        {
            if (this.m_LogDelegate == null)
                return;
            this.m_LogDelegate(LogSignature, LogRecord, RsltRecord);
        }

        internal _ICiImageEvents_SinkHelper()
        {
            this.m_dwCookie = 0;
            this.m_LogDelegate = (_ICiImageEvents_LogEventHandler)null;
        }
    }
}
