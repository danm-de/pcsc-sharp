using System;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Monitoring;

namespace PCSC.Tests.Monitoring.MonitorFactorySpecs
{
    [TestFixture]
    public class If_the_user_creates_a_monitor : Spec
    {
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();

        private MonitorFactory _sut;
        private ISCardMonitor _monitor;
        private bool _monitorHasBeenStarted;

        protected override void EstablishContext() {
			A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

			A.CallTo(() => _context.IsValid())
				.Returns(true);

			A.CallTo(() => _context.GetStatusChange(A<IntPtr>.Ignored, A<SCardReaderState[]>.Ignored))
                .Invokes(call => { _monitorHasBeenStarted = true; })
                .Returns(SCardError.Success);

            _sut = new MonitorFactory(_contextFactory);
        }

        protected override void Cleanup() {
            _monitor?.Dispose();
        }

        protected override void BecauseOf() {
            _monitor = _sut.Create(SCardScope.System);

            // Give the monitor thread 1 seconds (until we fail this test)
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void Shall_it_not_start_the_monitor() {
            _monitorHasBeenStarted.Should().BeFalse();
        }
    }
}
