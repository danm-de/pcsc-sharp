using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace PCSC.Test.Monitoring.DeviceMonitorFactorySpecs
{
    [TestFixture]
    public class If_the_user_starts_the_device_monitor : Spec
    {
        private const string READER_A = "READER_A";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _getStatusChangeCall = new AutoResetEvent(false);

        private DeviceMonitorFactory _sut;
        private bool _monitorHasBeenStarted;
        private IDeviceMonitor _monitor;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.GetReaders())
                .Returns(new[] {READER_A});

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => Match(states))))
                .Invokes(call => {
                    _getStatusChangeCall.Set();
                    _monitorHasBeenStarted = true;
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                })
                .Returns(SCardError.Success);

            _sut = new DeviceMonitorFactory(_contextFactory);
        }

        private static bool Match(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                && state.CurrentStateValue == (IntPtr)(1 << 16)
                && state.EventStateValue == (IntPtr)SCRState.Unknown;
        }

        protected override void Cleanup() {
            _monitor?.Dispose();
        }

        protected override void BecauseOf() {
            _monitor = _sut.Start(SCardScope.System);

            // Give the monitor thread 5 seconds (until we fail this test)
            _getStatusChangeCall.WaitOne(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void Shall_it_start_the_monitor_thread() {
            _monitorHasBeenStarted.Should().BeTrue();
        }
    }
}