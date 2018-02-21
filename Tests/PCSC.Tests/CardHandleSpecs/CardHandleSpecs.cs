using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Exceptions;
using PCSC.Interop;

namespace PCSC.Tests.CardHandleSpecs
{
    [TestFixture]
    public class If_the_user_tries_to_establish_a_reader_connection_using_an_invalid_readername : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>(opt => opt.Strict());

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return new TestCaseData(null, typeof(ArgumentNullException));
                yield return new TestCaseData(string.Empty, typeof(UnknownReaderException));
                yield return new TestCaseData(" ", typeof(UnknownReaderException));
            }
        }

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void It_should_throw(string readerName, Type expectedException) {
            var sut = new CardHandle(_context);
            try {
                sut.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
            } catch (Exception exception) {
                exception.Should().BeOfType(expectedException);
                return;
            }

            throw new Exception($"Expected to throw {expectedException.Name} but did not!");
        }
    }

    [TestFixture]
    public class If_the_user_tries_to_establish_a_reader_connection_using_an_invalid_context : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns(IntPtr.Zero);
            _sut = new CardHandle(_context);
        }

        [Test]
        public void It_should_throw() {
            Invoking(() => _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any))
                .Should().Throw<InvalidContextException>();
        }
    }

    [TestFixture]
    public class If_the_connection_request_was_unsuccessful : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly ISCardApi _api = A.Fake<ISCardApi>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);

            // let the request exit with INVALID_HANDLE
            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => _api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.InvalidHandle);

            _sut = new CardHandle(_api, _context);
        }

        [Test]
        public void It_should_throw() {
            Invoking(() => _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any))
                .Should()
                .Throw<InvalidContextException>();
        }
    }

    [TestFixture]
    public class If_the_connection_request_was_successful : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly ISCardApi _api = A.Fake<ISCardApi>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);

            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => _api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {(IntPtr) 123, SCardProtocol.T1});

            _sut = new CardHandle(_api, _context);
        }

        protected override void BecauseOf() {
            _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any);
        }

        [Test]
        public void It_should_have_the_active_protocol_set() {
            _sut.Protocol
                .Should()
                .Be(SCardProtocol.T1);
        }

        [Test]
        public void It_should_have_the_share_mode_set() {
            _sut.Mode
                .Should()
                .Be(SCardShareMode.Shared);
        }

        [Test]
        public void It_should_have_a_valid_handle() {
            _sut.Handle
                .Should()
                .Be((IntPtr) 123);
        }

        [Test]
        public void It_should_be_connected() {
            _sut.IsConnected
                .Should()
                .BeTrue();
        }

        [Test]
        public void It_should_have_the_readername_set() {
            _sut.ReaderName
                .Should()
                .Be("MyReader");
        }
    }

    [TestFixture]
    public class If_the_user_disposes_a_reader_connection : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly ISCardApi _api = A.Fake<ISCardApi>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);

            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => _api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {(IntPtr) 123, SCardProtocol.T1});

            A.CallTo(() => _api.Disconnect(A<IntPtr>.Ignored, A<SCardReaderDisposition>.Ignored))
                .Returns(SCardError.Success);

            _sut = new CardHandle(_api, _context);
            _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any);
        }

        protected override void BecauseOf() {
            _sut.Dispose();
        }

        [Test]
        public void It_should_have_the_no_protocol_set() {
            _sut.Protocol
                .Should()
                .Be(SCardProtocol.Unset);
        }

        [Test]
        public void It_should_not_have_a_valid_handle() {
            _sut.Handle
                .Should()
                .Be(IntPtr.Zero);
        }

        [Test]
        public void It_should_have_no_readername_set() {
            _sut.ReaderName
                .Should()
                .BeNull();
        }

        [Test]
        public void It_should_not_be_connected() {
            _sut.IsConnected
                .Should()
                .BeFalse();
        }

        [Test]
        public void It_should_have_disconnected_the_reader() {
            A.CallTo(() => _api.Disconnect((IntPtr) 123, A<SCardReaderDisposition>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }

    [TestFixture]
    public class If_the_user_disconnects : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly ISCardApi _api = A.Fake<ISCardApi>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);

            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => _api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {(IntPtr) 123, SCardProtocol.T1});

            A.CallTo(() => _api.Disconnect(A<IntPtr>.Ignored, A<SCardReaderDisposition>.Ignored))
                .Returns(SCardError.Success);

            _sut = new CardHandle(_api, _context);
            _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any);
        }

        protected override void BecauseOf() {
            _sut.Disconnect(SCardReaderDisposition.Reset);
        }

        [Test]
        public void It_should_have_the_no_protocol_set() {
            _sut.Protocol
                .Should()
                .Be(SCardProtocol.Unset);
        }

        [Test]
        public void It_should_not_have_a_valid_handle() {
            _sut.Handle
                .Should()
                .Be(IntPtr.Zero);
        }

        [Test]
        public void It_should_have_no_readername_set() {
            _sut.ReaderName
                .Should()
                .BeNull();
        }

        [Test]
        public void It_should_not_be_connected() {
            _sut.IsConnected
                .Should()
                .BeFalse();
        }

        [Test]
        public void It_should_have_disconnected_the_reader() {
            A.CallTo(() => _api.Disconnect((IntPtr) 123, SCardReaderDisposition.Reset))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }

    [TestFixture]
    public class If_the_user_reconnects : Spec
    {
        private readonly ISCardContext _context = A.Fake<ISCardContext>();
        private readonly ISCardApi _api = A.Fake<ISCardApi>();
        private CardHandle _sut;

        protected override void EstablishContext() {
            A.CallTo(() => _context.Handle).Returns((IntPtr) 1);
            A.CallTo(() => _context.IsValid()).Returns(true);

            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => _api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {(IntPtr) 123, SCardProtocol.T1});

            protocol = default(SCardProtocol);
            A.CallTo(() => _api.Reconnect((IntPtr) 123, SCardShareMode.Direct, SCardProtocol.Any,
                    SCardReaderDisposition.Reset, out protocol))
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {SCardProtocol.T15});

            _sut = new CardHandle(_api, _context);
            _sut.Connect("MyReader", SCardShareMode.Shared, SCardProtocol.Any);
        }

        protected override void BecauseOf() {
            _sut.Reconnect(SCardShareMode.Direct, SCardProtocol.Any, SCardReaderDisposition.Reset);
        }

        [Test]
        public void It_should_have_the_active_protocol_set() {
            _sut.Protocol
                .Should()
                .Be(SCardProtocol.T15);
        }

        [Test]
        public void It_should_have_the_share_mode_set() {
            _sut.Mode
                .Should()
                .Be(SCardShareMode.Direct);
        }

        [Test]
        public void It_should_have_a_valid_handle() {
            _sut.Handle
                .Should()
                .Be((IntPtr) 123);
        }

        [Test]
        public void It_should_be_connected() {
            _sut.IsConnected
                .Should()
                .BeTrue();
        }

        [Test]
        public void It_should_have_the_readername_set() {
            _sut.ReaderName
                .Should()
                .Be("MyReader");
        }
    }
}
