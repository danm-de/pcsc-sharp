using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PCSC.Monitoring;
using PCSC.Reactive.Events;

namespace PCSC.Reactive
{
    /// <summary>
    /// Extension methods for <see cref="IMonitorFactory"/>
    /// </summary>
    public static class MonitorFactoryExt
    {
        /// <summary>
        /// Creates an observable for smart card events.
        /// </summary>
        /// <param name="factory">Factory to use for <see cref="ISCardMonitor"/> creation.</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerName">Name of the smart card reader that shall be monitored.</param>
        /// <param name="scheduler">The scheduler to run the add and remove event handler logic on.</param>
        /// <returns></returns>
        public static IObservable<MonitorEvent> CreateObservable(this IMonitorFactory factory, SCardScope scope, string readerName, IScheduler scheduler = null) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }
            if (readerName == null) {
                throw new ArgumentNullException(nameof(readerName));
            }
            return factory.CreateObservable(scope, new[] { readerName }, scheduler);
        }

        /// <summary>
        /// Creates an observable for smart card events.
        /// </summary>
        /// <param name="factory">Factory to use for <see cref="ISCardMonitor"/> creation.</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerNames">Name of the smart card reader that shall be monitored.</param>
        /// <param name="scheduler">The scheduler to run the add and remove event handler logic on.</param>
        /// <returns></returns>
        public static IObservable<MonitorEvent> CreateObservable(this IMonitorFactory factory, SCardScope scope, IEnumerable<string> readerNames, IScheduler scheduler = null) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }
            if (readerNames == null) {
                throw new ArgumentNullException(nameof(readerNames));
            }

            return Observable.Create<MonitorEvent>(obs => {
                var monitor = factory.Create(scope);
                var useScheduler = scheduler ?? Scheduler.ForCurrentContext();

                var readers = readerNames
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToArray();

                var subscription = monitor
                    .ObserveEvents(useScheduler)
                    .Subscribe(obs);

                monitor.Start(readers);

                return new CompositeDisposable(subscription, monitor);
            });
        }
    }
}
