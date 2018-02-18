using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PCSC.Reactive.Events;

namespace PCSC.Reactive
{
    /// <summary>
    /// Extension methods for <see cref="IDeviceMonitorFactory"/>
    /// </summary>
    public static class DeviceMonitorFactoryExtensionMethods
    {
        /// <summary>
        /// Creates an observable for smartcard reader device events.
        /// </summary>
        /// <param name="factory">Factory to use for <see cref="IDeviceMonitor"/> creation.</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="scheduler">The scheduler to run the add and remove event handler logic on.</param>
        /// <returns></returns>
        public static IObservable<DeviceMonitorEvent> CreateObservable(this IDeviceMonitorFactory factory, SCardScope scope, IScheduler scheduler = null) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            return Observable.Create<DeviceMonitorEvent>(obs => {
                var monitor = factory.Create(scope);
                var useScheduler = scheduler ?? Scheduler.ForCurrentContext();

                var subscription = monitor
                    .ObserveEvents(useScheduler)
                    .Subscribe(obs);

                monitor.Start();

                return new CompositeDisposable(subscription, monitor);
            });
        }
    }
}