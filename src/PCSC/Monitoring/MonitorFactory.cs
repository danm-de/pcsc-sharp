using System;

namespace PCSC.Monitoring
{
    /// <summary>
    /// Smart card monitor factory
    /// </summary>
    public class MonitorFactory : IMonitorFactory
    {
        private static readonly Lazy<IMonitorFactory> _instance =
            new Lazy<IMonitorFactory>(() => new MonitorFactory(ContextFactory.Instance));

        private readonly IContextFactory _contextFactory;

        /// <summary>
        /// Default factory instance. Uses <see cref="ContextFactory.Instance"/> for context creation.
        /// </summary>
        public static IMonitorFactory Instance => _instance.Value;

        /// <summary>
        /// Creates a new monitor factory. 
        /// </summary>
        /// <param name="contextFactory">Context factory that creates <see cref="ISCardContext"/> for the new <see cref="ISCardMonitor"/> instances.</param>
        public MonitorFactory(IContextFactory contextFactory) {
            _contextFactory = contextFactory;
        }
        
        /// <inheritdoc />
        public ISCardMonitor Create(SCardScope scope) {
            return new SCardMonitor(_contextFactory, scope);
        }
        
        /// <inheritdoc />
        public void Release(ISCardMonitor monitor) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }

            monitor.Dispose();
        }
    }
}
