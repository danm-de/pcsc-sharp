using System;

namespace PCSC.Monitoring
{
    /// <summary>
    /// Smartcard reader device monitoring factory
    /// </summary>
    public sealed class DeviceMonitorFactory : IDeviceMonitorFactory
    {
        private static readonly Lazy<IDeviceMonitorFactory> _instance =
            new Lazy<IDeviceMonitorFactory>(() => new DeviceMonitorFactory(ContextFactory.Instance));

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
        
        /// <inheritdoc />
        public IDeviceMonitor Create(SCardScope scope) {
            return new DeviceMonitor(_contextFactory, scope);
        }
        
        /// <inheritdoc />
        public void Release(IDeviceMonitor monitor) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }

            monitor.Dispose();
        }
    }
}
