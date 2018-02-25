using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Interop;

namespace PCSC.Tests.CardReaderSpecs
{
    [TestFixture]
    public class If_the_user_disposes_a_reader_that_was_owner_of_a_card_handle : Spec
    {
        private readonly ICardHandle _handle = A.Fake<ICardHandle>();
        private CardReader _sut;

        protected override void EstablishContext() {
            _sut = new CardReader(_handle, true);
        }

        protected override void BecauseOf() {
            _sut.Dispose();
        }

        [Test]
        public void Should_the_card_handle_be_disposed() {
            A.CallTo(() => _handle.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }

    [TestFixture]
    public class If_the_user_disposes_a_reader_that_was_not_owner_of_a_card_handle : Spec
    {
        private readonly ICardHandle _handle = A.Fake<ICardHandle>();
        private CardReader _sut;

        protected override void EstablishContext() {
            _sut = new CardReader(_handle, false);
        }

        protected override void BecauseOf() {
            _sut.Dispose();
        }

        [Test]
        public void Should_the_card_handle_not_be_disposed() {
            A.CallTo(() => _handle.Dispose())
                .MustNotHaveHappened();
        }
    }

    public abstract class CardReaderSpec : Spec
    {
        protected readonly CardReader Sut;
        internal readonly ISCardApi Api = A.Fake<ISCardApi>();
        protected readonly ICardHandle CardHandle = A.Fake<ICardHandle>();

        protected CardReaderSpec() {
            A.CallTo(() => CardHandle.Handle).Returns((IntPtr) 123);
            A.CallTo(() => CardHandle.IsConnected).Returns(true);
            A.CallTo(() => CardHandle.Mode).Returns(SCardShareMode.Direct);
            A.CallTo(() => CardHandle.Protocol).Returns(SCardProtocol.Raw);
            A.CallTo(() => CardHandle.ReaderName).Returns("MyReader");

            Sut = new CardReader(Api, CardHandle, true);
        }
    }

    [TestFixture]
    public class If_the_user_reconnects_the_reader_using_SHARED_mode_and_protocol_T1 : CardReaderSpec
    {
        private string _oldReaderName;
        private bool _isConnected;

        protected override void EstablishContext() {
            A.CallTo(() => CardHandle.Reconnect(A<SCardShareMode>.Ignored, A<SCardProtocol>.Ignored,
                    A<SCardReaderDisposition>.Ignored))
                .Invokes(call => {
                    A.CallTo(() => CardHandle.Mode).Returns(call.Arguments.Get<SCardShareMode>(0));
                    A.CallTo(() => CardHandle.Protocol).Returns(call.Arguments.Get<SCardProtocol>(1));
                });

            _oldReaderName = Sut.ReaderName;
            _isConnected = Sut.IsConnected;
        }

        protected override void BecauseOf() {
            Sut.Reconnect(SCardShareMode.Shared, SCardProtocol.T1, SCardReaderDisposition.Eject);
        }

        [Test]
        public void Should_it_reconnect_the_card_handle() {
            A.CallTo(() => CardHandle.Reconnect(A<SCardShareMode>.Ignored, A<SCardProtocol>.Ignored,
                    A<SCardReaderDisposition>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void Should_the_new_protocol_be_T1() {
            Sut.Protocol.Should().Be(SCardProtocol.T1);
        }

        [Test]
        public void Should_be_shared_mode_enabled() {
            Sut.Mode.Should().Be(SCardShareMode.Shared);
        }

        [Test]
        public void Should_the_reader_name_the_same_as_before() {
            Sut.ReaderName.Should().Be(_oldReaderName);
        }

        [Test]
        public void Should_the_connection_state_the_same_as_before() {
            Sut.IsConnected.Should().Be(_isConnected);
        }
    }

    [TestFixture]
    public class If_the_user_begins_a_transaction : CardReaderSpec
    {
        private IDisposable _transaction;

        protected override void BecauseOf() {
            _transaction = Sut.Transaction(SCardReaderDisposition.Leave);
        }

        protected override void Cleanup() {
            _transaction?.Dispose();
        }

        [Test]
        public void Should_it_call_the_BeginTransaction_API() {
            A.CallTo(() => Api.BeginTransaction(CardHandle.Handle))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_not_call_the_EndTransaction_API() {
            A.CallTo(() => Api.EndTransaction(A<IntPtr>.Ignored, A<SCardReaderDisposition>.Ignored))
                .MustNotHaveHappened();
        }

    }

    [TestFixture]
    public class If_the_user_begins_and_ends_a_transaction : CardReaderSpec
    {
        protected override void BecauseOf() {
            using (Sut.Transaction(SCardReaderDisposition.Leave)) {
                // do something
            }
        }

        [Test]
        public void Should_it_call_the_EndTransaction_API() {
            A.CallTo(() => Api.EndTransaction(CardHandle.Handle, SCardReaderDisposition.Leave))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
