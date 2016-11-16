using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Events;
using NUnit.Framework;

namespace PCSC.Test.Monitoring.DeviceMonitorSpecs
{
    [TestFixture]
    public class If_the_user_monitors_for_device_changes_and_a_new_reader_has_been_attached : Spec
    {
        private const string READER_A = "READER_A";
        private const string READER_B = "READER_B";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _deviceEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _cancelEvent = new AutoResetEvent(false);
        private IEventMonitor _monitorEvents;

        private IDeviceMonitor _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.GetReaders())
                .Returns(new[] { READER_A });

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchFirstCall(states))))
                .ReturnsLazily(call => {
                    _cancelEvent.WaitOne(TimeSpan.FromSeconds(5));
                    return SCardError.Cancelled;
                });

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchSecondCall(states))))
                .ReturnsLazily(call => {
                    var success = _deviceEvent.WaitOne(TimeSpan.FromSeconds(5));
                    return success ? SCardError.Success : SCardError.Cancelled;
                });

            _sut = new DeviceMonitor(_contextFactory, SCardScope.System);
            _monitorEvents = _sut.MonitorEvents();

            _sut.Start();
        }

        private static bool MatchFirstCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                && state.CurrentStateValue == (IntPtr)(1 << 16)
                && state.EventStateValue == (IntPtr)SCRState.Unknown;
        }

        private static bool MatchSecondCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                && state.CurrentStateValue == (IntPtr)(2 << 16)
                && state.EventStateValue == (IntPtr)SCRState.Unknown;
        }

        protected override void Cleanup() {
            _cancelEvent.Set();
            _sut?.Dispose();
            _monitorEvents.Reset();
        }

        protected override void BecauseOf() {
            A.CallTo(() => _context.GetReaders())
                .Returns(new[] { READER_A, READER_B });

            _deviceEvent.Set();
        }

        [Test]
        public void Shall_it_raise_a_status_change_event() {
            _sut.ShouldRaise(nameof(_sut.Initialized))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.All.SequenceEqual(new[] { READER_A, READER_B })
                            && args.Attached.SequenceEqual(new [] { READER_B })
                            && args.Detached.SequenceEqual(Enumerable.Empty<string>()));
        }

        [Test]
        public void Shall_it_raise_a_initial_event() {
            _sut.ShouldRaise(nameof(_sut.Initialized))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.All.SequenceEqual(new[] {READER_A})
                            && args.Attached.SequenceEqual(Enumerable.Empty<string>())
                            && args.Detached.SequenceEqual(Enumerable.Empty<string>()));
        }
    }
}