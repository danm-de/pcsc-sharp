using System;
using System.Linq;
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

    [TestFixture]
    public class If_the_user_transmits_data_to_the_reader : CardReaderSpec
    {
        private int _bytesReceived;
        private readonly byte[] _sendBuffer = {0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0};
        private readonly byte[] _receiveBuffer = new byte[10];
        private readonly SCardPCI _receivePci = new SCardPCI();

        protected override void EstablishContext() {
            var recvBufferLength = 10;
            A.CallTo(() => Api.Transmit(CardHandle.Handle, SCardPCI.Raw, _sendBuffer, 3, _receivePci.MemoryPtr,
                    _receiveBuffer, ref recvBufferLength))
                .Invokes(_ => {
                    _receiveBuffer[0] = 0xA;
                    _receiveBuffer[1] = 0xB;
                    _receiveBuffer[2] = 0xC;
                })
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {3}); // recvBufferLength = 3
        }

        protected override void BecauseOf() {
            _bytesReceived = Sut.Transmit(
                sendPci: SCardPCI.Raw,
                sendBuffer: _sendBuffer,
                sendBufferLength: 3,
                receivePci: _receivePci,
                receiveBuffer: _receiveBuffer,
                receiveBufferLength: 10);
        }

        protected override void Cleanup() {
            _receivePci.Dispose();
        }

        [Test]
        public void Should_it_call_the_Transmit_API() {
            var recvBufferLength = 10;
            A.CallTo(() => Api.Transmit(CardHandle.Handle, SCardPCI.Raw, _sendBuffer, 3, _receivePci.MemoryPtr,
                    _receiveBuffer, ref recvBufferLength))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_receive_3_bytes() {
            _bytesReceived.Should().Be(3);
        }

        [Test]
        public void Should_the_receive_buffer_being_filled() {
            _receiveBuffer.Take(_bytesReceived)
                .Should()
                .ContainInOrder(0xA, 0xB, 0xC);
        }
    }

    [TestFixture]
    public class If_the_user_sends_an_IO_control_to_the_reader : CardReaderSpec
    {
        private readonly IntPtr _controlCode = (IntPtr) 9999;
        private readonly byte[] _sendBuffer = {0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0};
        private readonly byte[] _receiveBuffer = new byte[10];
        private int _bytesReceived;

        protected override void EstablishContext() {
            var recvBufferLength = 10;
            A.CallTo(() => Api.Control(CardHandle.Handle, _controlCode, _sendBuffer, 3, _receiveBuffer,
                    recvBufferLength, out recvBufferLength))
                .Invokes(_ => {
                    _receiveBuffer[0] = 0xA;
                    _receiveBuffer[1] = 0xB;
                    _receiveBuffer[2] = 0xC;
                    _receiveBuffer[3] = 0xD;
                })
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {4}); // recvBufferLength = 4
        }

        protected override void BecauseOf() {
            _bytesReceived = Sut.Control(_controlCode, _sendBuffer, 3, _receiveBuffer, 10);
        }

        [Test]
        public void Should_it_call_the_Control_API() {
            var recvBufferLength = 10;
            A.CallTo(() => Api.Control(CardHandle.Handle, _controlCode, _sendBuffer, 3, _receiveBuffer,
                    recvBufferLength, out recvBufferLength))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_receive_4_bytes() {
            _bytesReceived.Should().Be(4);
        }

        [Test]
        public void Should_the_receive_buffer_being_filled() {
            _receiveBuffer.Take(_bytesReceived)
                .Should()
                .ContainInOrder(0xA, 0xB, 0xC, 0xD);
        }
    }

    [TestFixture]
    public class If_the_user_requests_the_reader_status : CardReaderSpec
    {
        private ReaderStatus _readerStatus;

        protected override void EstablishContext() {
            string[] readerNames;
            IntPtr state;
            IntPtr protocol;
            byte[] atr;
            A.CallTo(() => Api.Status(CardHandle.Handle, out readerNames, out state, out protocol, out atr))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {
                    new[]{"MyReader"},
                    (IntPtr)(SCardState.Present |SCardState.Powered),
                    (IntPtr)SCardProtocol.Raw,
                    new byte[]{0x01, 0x02, 0x03, 0x04}
                });
        }

        protected override void BecauseOf() {
            _readerStatus = Sut.GetStatus();
        }

        [Test]
        public void Should_the_protocol_be_Raw() {
            _readerStatus.Protocol.Should().Be(SCardProtocol.Raw);
        }

        [Test]
        public void Should_the_card_state_be_Present_and_Powered() {
            _readerStatus.State.Should().Be(SCardState.Present | SCardState.Powered);
        }

        [Test]
        public void Should_the_ATR_be_01_02_03_04() {
            _readerStatus.GetAtr().Should().ContainInOrder(new byte[] {0x1, 0x2, 0x3, 0x4});
        }

        [Test]
        public void Should_the_friendly_reader_name_be_MyReader() {
            _readerStatus.GetReaderNames().Should().ContainSingle(name => name == "MyReader");
        }
    }

    [TestFixture]
    public class If_the_user_receives_a_reader_attribute : CardReaderSpec
    {
        private int _bytesReceived;
        private readonly IntPtr _attributeId = (IntPtr) 4567;
        private readonly byte[] _receiveBuffer = new byte[10];

        protected override void EstablishContext() {
            int receiveBufferLength;
            A.CallTo(() => Api.GetAttrib(CardHandle.Handle, _attributeId, _receiveBuffer, 10, out receiveBufferLength))
                .Invokes(_ => {
                    _receiveBuffer[0] = 0xA;
                    _receiveBuffer[1] = 0xB;
                    _receiveBuffer[2] = 0xC;
                    _receiveBuffer[3] = 0xD;
                })
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {4});
        }

        protected override void BecauseOf() {
            _bytesReceived = Sut.GetAttrib(_attributeId, _receiveBuffer, 10);
        }
        
        [Test]
        public void Should_it_receive_4_bytes() {
            _bytesReceived.Should().Be(4);
        }

        [Test]
        public void Should_the_receive_buffer_being_filled() {
            _receiveBuffer.Take(_bytesReceived)
                .Should()
                .ContainInOrder(0xA, 0xB, 0xC, 0xD);
        }
    }

    [TestFixture]
    public class If_the_user_sets_a_reader_attribute : CardReaderSpec
    {
        private readonly IntPtr _attributeId = (IntPtr)4567;
        private readonly byte[] _sendBuffer = {
            0xA,0xB,0xC,0xD,0x0,0x0,0x0
        };

        protected override void EstablishContext() {
            A.CallTo(() => Api.SetAttrib(CardHandle.Handle, _attributeId, _sendBuffer, 4))
                .Returns(SCardError.Success);
        }

        protected override void BecauseOf() {
            Sut.SetAttrib(_attributeId, _sendBuffer, 4);
        }

        [Test]
        public void Should_it_call_the_SetAttrib_API() {
            A.CallTo(() => Api.SetAttrib(CardHandle.Handle, _attributeId, _sendBuffer, 4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
