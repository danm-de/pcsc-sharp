using System;

namespace PCSC.Monitoring
{
    /// <summary>
    /// Smartcard reader device monitoring factory
    /// </summary>
    public interface IDeviceMonitorFactory
    {
        /// <summary>
        /// Creates a device monitor
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A <see cref="IDeviceMonitor"/></returns>
        IDeviceMonitor Create(SCardScope scope);
        
        /// <summary>
        /// Releases the smartcard device monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smartcard device monitor that shall be stopped and disposed.</param>
        void Release(IDeviceMonitor monitor);
    }
}