﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Events;
using NUnit.Framework;
using PCSC.Monitoring;

namespace PCSC.Tests.Monitoring.DeviceMonitorSpecs
{
    [TestFixture]
    public class If_a_new_reader_has_been_attached : Spec
    {
        private const string READER_A = "READER_A";
        private const string READER_B = "READER_B";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _getStatusChangeCall = new AutoResetEvent(false);

        private IDeviceMonitor _sut;
        private IMonitor<IDeviceMonitor> _monitorEvents;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.Infinite)
                .Returns(SCardContext.INFINITE);

            // first call -> only one attached reader
            A.CallTo(() => _context.GetReaders())
                .Returns([READER_A]);

            A.CallTo(() => _context.GetStatusChange(IntPtr.Zero, A<SCardReaderState[]>.That.Matches(
                    states => MatchFirstCall(states))))
                .ReturnsLazily(call => {
                    var states = call.Arguments.Get<SCardReaderState[]>(1)!;
                    states[0].EventStateValue = (1 << 16); // tell the user that there is one reader
                    return SCardError.Success;
                });

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchSecondCall(states))))
                .ReturnsLazily(call => {
                    // second call -> two attached readers
                    A.CallTo(() => _context.GetReaders())
                        .Returns([READER_A, READER_B]);

                    var states = call.Arguments.Get<SCardReaderState[]>(1)!;
                    states[0].EventStateValue = (2 << 16); // tell the user that there two readers

                    _getStatusChangeCall.Set();

                    return SCardError.Success;
                });

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchThirdCall(states))))
                .Returns(SCardError.Cancelled);

            _sut = new DeviceMonitor(_contextFactory, SCardScope.System);
            _monitorEvents = _sut.Monitor();
        }

        private static bool MatchFirstCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue ==(IntPtr) SCRState.Unaware
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        private static bool MatchSecondCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue == (1 << 16) // one connected reader
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        private static bool MatchThirdCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue == (2 << 16) // two connected readers
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        protected override void Cleanup() {
            _sut?.Dispose();
            _monitorEvents.Dispose();
        }

        protected override void BecauseOf() {
            _sut.Start();

            // Give the monitor thread 5 seconds (until we fail this test)
            _getStatusChangeCall.WaitOne(TimeSpan.FromSeconds(5));
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void Should_it_raise_a_StatusChanged_event_containing_READER_B_as_attached() {
            _monitorEvents.Should().Raise(nameof(_sut.StatusChanged))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.AllReaders.SequenceEqual(new[] {READER_A, READER_B})
                            && args.AttachedReaders.SequenceEqual(new[] {READER_B})
                            && args.DetachedReaders.SequenceEqual(Enumerable.Empty<string>()));
        }

        [Test]
        public void Should_it_raise_an_Initialized_event() {
            _monitorEvents.Should().Raise(nameof(_sut.Initialized))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.AllReaders.SequenceEqual(new[] {READER_A})
                            && args.AttachedReaders.SequenceEqual(Enumerable.Empty<string>())
                            && args.DetachedReaders.SequenceEqual(Enumerable.Empty<string>()));
        }
    }

    [TestFixture]
    public class If_a_reader_has_been_detached : Spec
    {
        private const string READER_A = "READER_A";
        private const string READER_B = "READER_B";
        private readonly IContextFactory _contextFactory = A.Fake<IContextFactory>();
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly AutoResetEvent _getStatusChangeCall = new AutoResetEvent(false);

        private IDeviceMonitor _sut;
        private IMonitor<IDeviceMonitor> _monitorEvents;

        protected override void EstablishContext() {
            A.CallTo(() => _contextFactory.Establish(SCardScope.System))
                .Returns(_context);

            A.CallTo(() => _context.IsValid())
                .Returns(true);

            A.CallTo(() => _context.Infinite)
                .Returns(SCardContext.INFINITE);

            A.CallTo(() => _context.GetStatusChange(IntPtr.Zero, A<SCardReaderState[]>.That.Matches(
                    states => MatchFirstCall(states))))
                .ReturnsLazily(call => {
                    // first call -> two attached readers
                    A.CallTo(() => _context.GetReaders())
                        .Returns([READER_A, READER_B]);;

                    var states = call.Arguments.Get<SCardReaderState[]>(1)!;
                    states[0].EventStateValue = (2 << 16); // tell the user that there two readers

                    _getStatusChangeCall.Set();

                    return SCardError.Success;
                });


            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchSecondCall(states))))
                .ReturnsLazily(call => {
                    // second call -> one attached reader
                    A.CallTo(() => _context.GetReaders())
                        .Returns(new[] {READER_A});

                    var states = call.Arguments.Get<SCardReaderState[]>(1)!;
                    states[0].EventStateValue = (1 << 16); // tell the user that there is one reader

                    _getStatusChangeCall.Set();

                    return SCardError.Success;
                });

            A.CallTo(() => _context.GetStatusChange(SCardContext.INFINITE, A<SCardReaderState[]>.That.Matches(
                    states => MatchThirdCall(states))))
                .Returns(SCardError.Cancelled);

            _sut = new DeviceMonitor(_contextFactory, SCardScope.System);
            _monitorEvents = _sut.Monitor();
        }

        private static bool MatchFirstCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue == (IntPtr) SCRState.Unaware
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        private static bool MatchSecondCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue == (2 << 16) // two connected readers
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        private static bool MatchThirdCall(IEnumerable<SCardReaderState> states) {
            var state = states.Single();
            return state.ReaderName == "\\\\?PnP?\\Notification"
                   && state.CurrentStateValue == (1 << 16) // one connected reader
                   && state.EventStateValue == (IntPtr) SCRState.Unknown;
        }

        protected override void Cleanup() {
            _sut?.Dispose();
            _monitorEvents.Dispose();
        }

        protected override void BecauseOf() {
            _sut.Start();

            // Give the monitor thread 5 seconds (until we fail this test)
            _getStatusChangeCall.WaitOne(TimeSpan.FromSeconds(5));
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void Should_it_raise_a_StatusChanged_event_containing_READER_B_as_detached() {
            _monitorEvents.Should().Raise(nameof(_sut.StatusChanged))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.AllReaders.SequenceEqual(new[] {READER_A})
                            && args.DetachedReaders.SequenceEqual(new[] {READER_B})
                            && args.AttachedReaders.SequenceEqual(Enumerable.Empty<string>()));
        }

        [Test]
        public void Should_it_raise_an_Initialized_event() {
            _monitorEvents.Should().Raise(nameof(_sut.Initialized))
                .WithArgs<DeviceChangeEventArgs>(
                    args => args.AllReaders.SequenceEqual(new[] {READER_A, READER_B})
                            && args.AttachedReaders.SequenceEqual(Enumerable.Empty<string>())
                            && args.DetachedReaders.SequenceEqual(Enumerable.Empty<string>()));
        }
    }
}
