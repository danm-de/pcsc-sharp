using System;
using System.Collections.Generic;
using System.Linq;

namespace PCSC
{
    /// <summary>
    /// Smart card monitor factory
    /// </summary>
    public class MonitorFactory : IMonitorFactory
    {
        private static readonly Lazy<IMonitorFactory> _instance = new Lazy<IMonitorFactory>(() => new MonitorFactory(ContextFactory.Instance));
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

        /// <summary>
        /// Creates and starts a smart card event monitor for a specific reader
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerName">Name of the smart card reader that shall be monitored.</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        public ISCardMonitor Start(SCardScope scope, string readerName) {
            if (readerName == null) {
                throw new ArgumentNullException(nameof(readerName));
            }
            return Start(scope, new[] {readerName});
        }

        /// <summary>
        /// Creates and starts a smart card event monitor for one or more readers.
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerNames">Names of the smart card readers that shall be monitored.</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        public ISCardMonitor Start(SCardScope scope, IEnumerable<string> readerNames) {
            return Start(scope, readerNames, null);
        }


        /// <summary>
        /// Creates and starts a smart card event monitor for one or more readers.
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerNames">Names of the smart card readers that shall be monitored.</param>
        /// <param name="preStartAction">Action that will be invoked prior monitor start</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        public ISCardMonitor Start(SCardScope scope, IEnumerable<string> readerNames, Action<ISCardMonitor> preStartAction) {
            if (readerNames == null) {
                throw new ArgumentNullException(nameof(readerNames));
            }

            var readers = readerNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray();

            var monitor = Create(scope);
            try {
                preStartAction?.Invoke(monitor);
                monitor.Start(readers);
                return monitor;
            } catch {
                monitor.Dispose();
                throw;
            }
        }

        /// <summary>
        ///  Creates a smart card event monitor 
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A <see cref="ISCardMonitor"/></returns>
        public ISCardMonitor Create(SCardScope scope) {
            return new SCardMonitor(_contextFactory, scope);
        }

        /// <summary>
        /// Releases the smart card monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smart card monitor that shall be stopped and disposed.</param>
        public void Release(ISCardMonitor monitor) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }
            monitor.Dispose();
        }
    }
}