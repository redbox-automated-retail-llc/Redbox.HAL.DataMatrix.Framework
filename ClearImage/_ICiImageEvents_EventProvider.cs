using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace ClearImage
{
    public class _ICiImageEvents_EventProvider : _ICiImageEvents_Event, IDisposable
    {
        private IConnectionPointContainer m_ConnectionPointContainer;
        private ArrayList m_aEventSinkHelpers;
        private IConnectionPoint m_ConnectionPoint;

        private void Init()
        {
            IConnectionPoint ppCP = (IConnectionPoint)null;
            Guid riid = new Guid(new byte[16]
            {
        (byte) 139,
        (byte) 241,
        (byte) 188,
        (byte) 242,
        (byte) 39,
        (byte) 11,
        (byte) 212,
        (byte) 17,
        (byte) 181,
        (byte) 245,
        (byte) 156,
        (byte) 199,
        (byte) 103,
        (byte) 0,
        (byte) 0,
        (byte) 0
            });
            this.m_ConnectionPointContainer.FindConnectionPoint(ref riid, out ppCP);
            this.m_ConnectionPoint = ppCP;
            this.m_aEventSinkHelpers = new ArrayList();
        }

        public virtual event _ICiImageEvents_LogEventHandler Log
        {
            add
            {
                Monitor.Enter((object)this);
                try
                {
                    if (this.m_ConnectionPoint == null)
                        this.Init();
                    _ICiImageEvents_SinkHelper pUnkSink = new _ICiImageEvents_SinkHelper();
                    int pdwCookie = 0;
                    this.m_ConnectionPoint.Advise((object)pUnkSink, out pdwCookie);
                    pUnkSink.m_dwCookie = pdwCookie;
                    pUnkSink.m_LogDelegate = value;
                    this.m_aEventSinkHelpers.Add((object)pUnkSink);
                }
                finally
                {
                    Monitor.Exit((object)this);
                }
            }
            remove
            {
                Monitor.Enter((object)this);
                try
                {
                    if (this.m_aEventSinkHelpers == null)
                        return;
                    int count = this.m_aEventSinkHelpers.Count;
                    int index = 0;
                    if (0 >= count)
                        return;
                    _ICiImageEvents_SinkHelper aEventSinkHelper;
                    do
                    {
                        aEventSinkHelper = (_ICiImageEvents_SinkHelper)this.m_aEventSinkHelpers[index];
                        if (aEventSinkHelper.m_LogDelegate == null || ((aEventSinkHelper.m_LogDelegate.Equals((object)value) ? 1 : 0) & (int)byte.MaxValue) == 0)
                            ++index;
                        else
                            goto label_8;
                    }
                    while (index < count);
                    return;
                label_8:
                    this.m_aEventSinkHelpers.RemoveAt(index);
                    this.m_ConnectionPoint.Unadvise(aEventSinkHelper.m_dwCookie);
                    if (count > 1)
                        return;
                    Marshal.ReleaseComObject((object)this.m_ConnectionPoint);
                    this.m_ConnectionPoint = (IConnectionPoint)null;
                    this.m_aEventSinkHelpers = (ArrayList)null;
                }
                finally
                {
                    Monitor.Exit((object)this);
                }
            }
        }

        public _ICiImageEvents_EventProvider(object object_0)
        {
            this.m_ConnectionPointContainer = (IConnectionPointContainer)object_0;
        }

        public virtual void Dispose()
        {
            Monitor.Enter((object)this);
            try
            {
                if (this.m_ConnectionPoint == null)
                    return;
                int count = this.m_aEventSinkHelpers.Count;
                int index = 0;
                if (0 < count)
                {
                    do
                    {
                        this.m_ConnectionPoint.Unadvise(((_ICiImageEvents_SinkHelper)this.m_aEventSinkHelpers[index]).m_dwCookie);
                        ++index;
                    }
                    while (index < count);
                }
                Marshal.ReleaseComObject((object)this.m_ConnectionPoint);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Monitor.Exit((object)this);
            }

            GC.SuppressFinalize((object)this);
        }
    }
}
