using System;

namespace PCSC.Monitoring
{
    /// <summary>
    /// Smart card monitor factory
    /// </summary>
    public interface IMonitorFactory
    {
        /// <summary>
        ///  Creates a smart card event monitor 
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A <see cref="ISCardMonitor"/></returns>
        ISCardMonitor Create(SCardScope scope);

        /// <summary>
        /// Releases the smart card monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smart card monitor that shall be stopped and disposed.</param>
        void Release(ISCardMonitor monitor);
    }
}
