using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PCSC.Reactive.Events;

namespace PCSC.Reactive
{
    public static class SCardMonitorExtensionMethods
    {
        /// <summary>
        /// Listen to all smart card events of a given <see cref="ISCardMonitor"/>.
        /// </summary>
        /// <param name="monitor">The smart card monitor to listen on.</param>
        /// <param name="scheduler">The scheduler to run the add and remove event handler logic on.</param>
        /// <returns>An observable of all smart card events.</returns>
        public static IObservable<MonitorEvent> ObserveEvents(this ISCardMonitor monitor,
            IScheduler scheduler = null) {
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }

            var useScheduler = scheduler ?? Scheduler.ForCurrentContext();

            var initialized = Observable.FromEventPattern<CardInitializedEvent, CardStatusEventArgs>(
                handler => monitor.Initialized += handler,
                handler => monitor.Initialized -= handler,
                useScheduler)
                .Select(ev => ev.EventArgs)
                .Select(args => new MonitorInitialized(args.ReaderName, args.Atr, args.State))
                .Replay();
                
            var initializedConnected = initialized.Connect();

            var cardInserted = Observable.FromEventPattern<CardInsertedEvent, CardStatusEventArgs>(
                handler => monitor.CardInserted += handler,
                handler => monitor.CardInserted -= handler,
                useScheduler)
                .Select(ev => ev.EventArgs)
                .Select(args => new CardInserted(args.ReaderName, args.Atr, args.State));

            var cardRemoved = Observable.FromEventPattern<CardRemovedEvent, CardStatusEventArgs>(
                handler => monitor.CardRemoved += handler,
                handler => monitor.CardRemoved -= handler,
                useScheduler)
                .Select(ev => ev.EventArgs)
                .Select(args => new CardRemoved(args.ReaderName, args.Atr, args.State));

            var statusChanged = Observable.FromEventPattern<StatusChangeEvent, StatusChangeEventArgs>(
                handler => monitor.StatusChanged += handler,
                handler => monitor.StatusChanged -= handler,
                useScheduler)
                .Select(ev => ev.EventArgs)
                .Select(args => new CardStatusChanged(args.ReaderName, args.Atr, args.LastState, args.NewState));

            var monitorEvents = initialized
                .Cast<MonitorEvent>()
                .Merge(cardInserted)
                .Merge(cardRemoved)
                .Merge(statusChanged);
                
            var monitorException = Observable.FromEventPattern<MonitorExceptionEvent, PCSCException>(
                handler => monitor.MonitorException += handler,
                handler => monitor.MonitorException -= handler,
                scheduler)
                .Select(ev => ev.EventArgs);

            return Observable.Create<MonitorEvent>(obs => {
                var normalEvents = monitorEvents
                    .Subscribe(obs.OnNext);

                return new CompositeDisposable(
                    normalEvents,
                    initializedConnected,
                    monitorException.Take(1).Subscribe(ex => {
                        normalEvents.Dispose();
                        initializedConnected.Dispose();
                        obs.OnError(ex);
                    }));
            });
        }
    }
}