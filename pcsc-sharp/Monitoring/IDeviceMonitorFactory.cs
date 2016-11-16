using System;

namespace PCSC
{
    /// <summary>
    /// Smartcard reader device monitoring factory
    /// </summary>
    public interface IDeviceMonitorFactory
    {
        /// <summary>
        /// Starts device monitoring
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A started <see cref="IDeviceMonitor"/></returns>
        IDeviceMonitor Start(SCardScope scope);

        /// <summary>
        /// Starts device monitoring
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="preStartAction">Action that will be invoked prior monitor start</param>
        /// <returns>A started <see cref="IDeviceMonitor"/></returns>
        IDeviceMonitor Start(SCardScope scope, Action<IDeviceMonitor> preStartAction);

        /// <summary>
        /// Releases the smartcard device monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smartcard device monitor that shall be stopped and disposed.</param>
        void Release(IDeviceMonitor monitor);
    }
}