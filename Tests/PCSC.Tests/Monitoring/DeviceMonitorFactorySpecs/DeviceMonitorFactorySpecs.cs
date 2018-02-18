using System;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Monitoring;

namespace PCSC.Tests.Monitoring.DeviceMonitorFactorySpecs
{
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