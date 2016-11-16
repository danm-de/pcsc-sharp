using System;
using System.Linq;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Events;
using NUnit.Framework;

namespace PCSC.Tests.Monitoring.DeviceMonitorFactorySpecs
{
    [TestFixture]
    public class If_the_user_starts_a_device_monitor : Spec
    {
        private const string READER_A = "READER_A";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _getStatusChangeCall = new AutoResetEvent(false);
        
        private DeviceMonitorFactory _sut;
        private bool _monitorHasBeenStarted;
        private IDeviceMonitor _monitor;
        private IEventMonitor _monitorEvents;
        private int _getStatusChangeCalled;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.GetReaders())
                .Returns(new[] {READER_A});

            A.CallTo(() => _context.GetStatusChange(A<IntPtr>.Ignored, A<SCardReaderState[]>.Ignored))
                .ReturnsLazily(call => {
                    _getStatusChangeCalled++;
                    _monitorHasBeenStarted = true;
                    _getStatusChangeCall.Set();

                    return _getStatusChangeCalled == 1
                        ? SCardError.Success
                        : SCardError.Cancelled;
                });

            _sut = new DeviceMonitorFactory(_contextFactory);
        }

        protected override void Cleanup() {
            _monitor?.Dispose();
            _monitorEvents.Reset();
        }

        protected override void BecauseOf() {
            _monitor = _sut.Start(SCardScope.System, PreAction);

            // Give the monitor thread 5 seconds (until we fail this test)
            _getStatusChangeCall.WaitOne(TimeSpan.FromSeconds(5));
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        private void PreAction(IDeviceMonitor monitor) {
            _monitorEvents = monitor.MonitorEvents();
        }

        [Test]
        public void Should_it_start_the_monitor_thread() {
            _monitorHasBeenStarted.Should().BeTrue();
        }

        [Test]
        public void Should_it_raise_the_Initialized_event() {
            _monitor
                .ShouldRaise(nameof(_monitor.Initialized))
                .WithArgs<DeviceChangeEventArgs>(args => args.AllReaders.SequenceEqual(new[] { READER_A }));
        }
    }

    [TestFixture]
    public class If_the_user_creates_a_device_monitor : Spec
    {
        private const string READER_A = "READER_A";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _getStatusChangeCall = new AutoResetEvent(false);

        private DeviceMonitorFactory _sut;
        private bool _monitorHasBeenStarted;
        private IDeviceMonitor _monitor;
        private int _getStatusChangeCalled;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.GetReaders())
                .Returns(new[] { READER_A });

            A.CallTo(() => _context.GetStatusChange(A<IntPtr>.Ignored, A<SCardReaderState[]>.Ignored))
                .ReturnsLazily(call => {
                    _getStatusChangeCalled++;
                    _monitorHasBeenStarted = true;
                    _getStatusChangeCall.Set();

                    return _getStatusChangeCalled == 1
                        ? SCardError.Success
                        : SCardError.Cancelled;
                });

            _sut = new DeviceMonitorFactory(_contextFactory);
        }

        protected override void Cleanup() {
            _monitor?.Dispose();
        }

        protected override void BecauseOf() {
            _monitor = _sut.Create(SCardScope.System);
        }
        
        [Test]
        public void Should_it_not_start_the_monitor_thread() {
            _monitorHasBeenStarted.Should().BeFalse();
        }
    }
}