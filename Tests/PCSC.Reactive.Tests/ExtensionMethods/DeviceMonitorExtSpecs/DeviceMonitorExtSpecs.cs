using System;
using System.Linq;
using System.Reactive;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using PCSC.Reactive.Events;

namespace PCSC.Reactive.Tests.ExtensionMethods.DeviceMonitorExtSpecs {
    [TestFixture]
    public class If_a_device_monitor_signals_initialization : Spec
    {
        private const string READER_A = "Reader A";
        private const string READER_B = "Reader B";
        private readonly IDeviceMonitor _monitor = A.Fake<IDeviceMonitor>();
        private readonly TestScheduler _scheduler = new TestScheduler();
        private ITestableObserver<DeviceMonitorEvent> _observer;
        private IDisposable _subscription;
        private DeviceChangeEventArgs _eventArgs;

        protected override void EstablishContext() {
            _eventArgs = new DeviceChangeEventArgs(
                new[] {READER_A, READER_B},
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());

            _observer = _scheduler.CreateObserver<DeviceMonitorEvent>();

            _subscription = _monitor
                .ObserveEvents()
                .Subscribe(_observer);
        }

        protected override void BecauseOf() {
            _monitor.Initialized += Raise.With<DeviceChangeEvent>(_monitor, _eventArgs);
        }

        protected override void Cleanup() {
            _subscription?.Dispose();
        }

        [Test]
        public void Should_the_observable_emit_a_DeviceMonitorInitialized_event() {
            _observer.Messages
                .Should()
                .ContainSingle(
                    rec =>
                        rec.Value.Kind == NotificationKind.OnNext
                        && rec.Value.Value is DeviceMonitorInitialized
                        && rec.Value.Value.EventArgs == _eventArgs
                        && rec.Value.Value.Readers.SequenceEqual(new[] { READER_A, READER_B }));
        }
    }

    [TestFixture]
    public class If_a_device_monitor_signals_a_new_attached_reader : Spec
    {
        private const string READER_A = "Reader A";
        private const string READER_B = "Reader B";
        private readonly IDeviceMonitor _monitor = A.Fake<IDeviceMonitor>();
        private readonly TestScheduler _scheduler = new TestScheduler();
        private ITestableObserver<DeviceMonitorEvent> _observer;
        private IDisposable _subscription;
        private DeviceChangeEventArgs _eventArgs;

        protected override void EstablishContext() {
            _eventArgs = new DeviceChangeEventArgs(
                new[] { READER_A, READER_B },
                new[] { READER_B },
                Enumerable.Empty<string>());

            _observer = _scheduler.CreateObserver<DeviceMonitorEvent>();

            _subscription = _monitor
                .ObserveEvents()
                .Subscribe(_observer);
        }

        protected override void BecauseOf() {
            _monitor.StatusChanged += Raise.With<DeviceChangeEvent>(_monitor, _eventArgs);
        }

        protected override void Cleanup() {
            _subscription?.Dispose();
        }

        [Test]
        public void Should_the_observable_emit_a_ReadersAttached_event() {
            _observer.Messages
                .Should()
                .ContainSingle(
                    rec =>
                        rec.Value.Kind == NotificationKind.OnNext
                        && rec.Value.Value is ReadersAttached
                        && rec.Value.Value.EventArgs == _eventArgs
                        && rec.Value.Value.Readers.SequenceEqual(new [] {READER_B}));
        }
    }

    [TestFixture]
    public class If_a_device_monitor_signals_a_detached_reader : Spec
    {
        private const string READER_A = "Reader A";
        private const string READER_B = "Reader B";
        private readonly IDeviceMonitor _monitor = A.Fake<IDeviceMonitor>();
        private readonly TestScheduler _scheduler = new TestScheduler();
        private ITestableObserver<DeviceMonitorEvent> _observer;
        private IDisposable _subscription;
        private DeviceChangeEventArgs _eventArgs;

        protected override void EstablishContext() {
            _eventArgs = new DeviceChangeEventArgs(
                new[] { READER_A },
                Enumerable.Empty<string>(),
                new[] { READER_B });

            _observer = _scheduler.CreateObserver<DeviceMonitorEvent>();
            
            _subscription = _monitor
                .ObserveEvents()
                .Subscribe(_observer);
        }

        protected override void BecauseOf() {
            _monitor.StatusChanged += Raise.With<DeviceChangeEvent>(_monitor, _eventArgs);
        }

        protected override void Cleanup() {
            _subscription?.Dispose();
        }

        [Test]
        public void Should_the_observable_emit_a_ReadersDetached_event() {
            _observer.Messages
                .Should()
                .ContainSingle(
                    rec =>
                        rec.Value.Kind == NotificationKind.OnNext
                        && rec.Value.Value is ReadersDetached
                        && rec.Value.Value.EventArgs == _eventArgs
                        && rec.Value.Value.Readers.SequenceEqual(new[] { READER_B }));
        }
    }

    [TestFixture]
    public class If_a_device_monitor_signals_an_exception : Spec
    {
        private readonly IDeviceMonitor _monitor = A.Fake<IDeviceMonitor>();
        private readonly TestScheduler _scheduler = new TestScheduler();
        private ITestableObserver<DeviceMonitorEvent> _observer;
        private IDisposable _subscription;
        private DeviceMonitorExceptionEventArgs _eventArgs;

        protected override void EstablishContext() {
            _eventArgs = new DeviceMonitorExceptionEventArgs(new Exception());

            _observer = _scheduler.CreateObserver<DeviceMonitorEvent>();
            
            _subscription = _monitor
                .ObserveEvents()
                .Subscribe(_observer);
        }

        protected override void BecauseOf() {
            _monitor.MonitorException += Raise.With<DeviceMonitorExceptionEvent>(_monitor, _eventArgs);
        }

        protected override void Cleanup() {
            _subscription?.Dispose();
        }

        [Test]
        public void Should_the_observable_forward_the_received_exception_into_the_error_channel() {
            _observer.Messages
                .Should()
                .ContainSingle(
                    rec =>
                        rec.Value.Kind == NotificationKind.OnError
                        && rec.Value.Exception == _eventArgs.Exception);
        }
    }
}