using System;

namespace PCSC
{
    /// <summary>
    /// Smartcard reader device monitoring factory
    /// </summary>
    public sealed class DeviceMonitorFactory : IDeviceMonitorFactory
    {
        private static readonly Lazy<IDeviceMonitorFactory> _instance = new Lazy<IDeviceMonitorFactory>(() => new DeviceMonitorFactory(ContextFactory.Instance));
        private readonly IContextFactory _contextFactory;

        /// <summary>
        /// Default factory instance. Uses <see cref="ContextFactory.Instance"/> for context creation.
        /// </summary>
        public static IDeviceMonitorFactory Instance => _instance.Value;

        /// <summary>
        /// Creates a monitor instance
        /// </summary>
        /// <param name="contextFactory">Context factory to use</param>
        public DeviceMonitorFactory(IContextFactory contextFactory) {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Creates a device monitor
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A <see cref="IDeviceMonitor"/></returns>
        public IDeviceMonitor Create(SCardScope scope) {
            return new DeviceMonitor(_contextFactory, scope);
        }
        
        /// <summary>
        /// Releases the smartcard device monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smartcard device monitor that shall be stopped and disposed.</param>
        public void Release(IDeviceMonitor monitor) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }
            monitor.Dispose();
        }
    }
}