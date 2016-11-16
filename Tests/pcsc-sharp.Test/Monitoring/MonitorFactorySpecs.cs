using System;
using System.Linq;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
namespace PCSC.Test.Monitoring.MonitorFactorySpecs
{
    public class If_the_user_starts_a_monitor_for_a_single_smart_card_reader : Spec
    {
        private const string REQUESTED_READER = "MY_READER";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _get_status_change_call = new AutoResetEvent(false);

        private MonitorFactory _sut;
        private ISCardMonitor _monitor;
        private bool _monitor_has_been_started;

        protected override void EstablishContext() {
			A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

			A.CallTo(() => _context.IsValid())
				.Returns(true);

			A.CallTo(() => _context.GetStatusChange(IntPtr.Zero, A<SCardReaderState[]>.That.Matches(
                    states => states.Any(s => s.ReaderName == REQUESTED_READER))))
                .Invokes(call => {
                    _get_status_change_call.Set();
                    _monitor_has_been_started = true;
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                })
                .Returns(SCardError.Success);

            _sut = new MonitorFactory(_contextFactory);
        }

        protected override void Cleanup() {
            _monitor?.Dispose();
        }

        protected override void BecauseOf() {
            _monitor = _sut.Start(SCardScope.System, REQUESTED_READER);

            // Give the monitor thread 5 seconds (until we fail this test)
            _get_status_change_call.WaitOne(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void Shall_it_start_the_monitor() {
            _monitor_has_been_started.Should().BeTrue();
        }

        [Test]
        public void Shall_the_requested_reader_listed_as_monitored_device() {
            _monitor.ReaderNames.Should().Contain(REQUESTED_READER);
        }
    }
}