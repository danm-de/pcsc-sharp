using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PCSC.Reactive.Events;

namespace PCSC.Reactive
{
    /// <summary>
    /// Device monitor extension methods
    /// </summary>
    public static class DeviceMonitorExtensionMethods
    {
        /// <summary>
        /// Listen to all device events of a given <see cref="IDeviceMonitor"/>.
        /// </summary>
        /// <param name="monitor">The device monitor to listen on.</param>
        /// <param name="scheduler">The scheduler to run the add and remove event handler logic on.</param>
        /// <returns>An observable of reader attached/detached events.</returns>
        public static IObservable<DeviceMonitorEvent> ObserveEvents(this IDeviceMonitor monitor, IScheduler scheduler = null) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }

            var useScheduler = scheduler ?? Scheduler.ForCurrentContext();

            var initialized = Observable.FromEventPattern<DeviceChangeEvent, DeviceChangeEventArgs>(
                    handler => monitor.Initialized += handler,
                    handler => monitor.Initialized -= handler,
                    useScheduler)
                .Select(ev => ev.EventArgs)
                .Select(args => new DeviceMonitorInitialized(args))
                .Replay();

            var initializedConnected = initialized.Connect();

            var statusChanged = Observable.FromEventPattern<DeviceChangeEvent, DeviceChangeEventArgs>(
                    handler => monitor.StatusChanged += handler,
                    handler => monitor.StatusChanged -= handler,
                    useScheduler)
                .Select(ev => ev.EventArgs)
                .SelectMany(CreateEvents);

            var monitorException = Observable.FromEventPattern<DeviceMonitorExceptionEvent, DeviceMonitorExceptionEventArgs>(
                    handler => monitor.MonitorException += handler,
                    handler => monitor.MonitorException -= handler,
                    useScheduler)
                .Select(ev => ev.EventArgs);

            var monitorEvents = initialized
                .Cast<DeviceMonitorEvent>()
                .Merge(statusChanged);

            return Observable.Create<DeviceMonitorEvent>(obs => {
                var normalEvents = monitorEvents.Subscribe(obs.OnNext);

                return new CompositeDisposable(
                    normalEvents,
                    initializedConnected,
                    monitorException.Take(1).Subscribe(args => {
                        normalEvents.Dispose();
                        initializedConnected.Dispose();
                        obs.OnError(args.Exception);
                    }));
            });
        }

        private static IEnumerable<DeviceMonitorEvent> CreateEvents(DeviceChangeEventArgs args) {
            if (args.DetachedReaders.Any()) {
                yield return new ReadersDetached(args);
            }

            if (args.AttachedReaders.Any()) {
                yield return new ReadersAttached(args);
            }
        }
    }
}