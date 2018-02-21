using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Interop;

namespace PCSC.Tests.SCardContextSpecs
{
    public abstract class SCardContextSpec : Spec
    {
        internal readonly ISCardApi Api = A.Fake<ISCardApi>(opts => opts.Strict());
        protected readonly IntPtr ContextHandle = (IntPtr) 12345679;
        protected readonly SCardContext Sut;

        protected SCardContextSpec() {
            IntPtr handle;

            // allow SCardContext.Establish() for all tests
            A.CallTo(() => Api.EstablishContext(
                    A<SCardScope>.Ignored,
                    A<IntPtr>.Ignored,
                    A<IntPtr>.Ignored,
                    out handle))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {ContextHandle});

            // allow SCardContext.Release() for all tests
            A.CallTo(() => Api.ReleaseContext(ContextHandle))
                .Returns(SCardError.Success);

            Sut = new SCardContext(Api);
        }
    }

    [TestFixture]
    public class If_the_user_establishes_the_context : SCardContextSpec
    {
        protected override void BecauseOf() {
            Sut.Establish(SCardScope.User);
        }

        [Test]
        public void Should_the_handle_from_PCSC_API_being_set() {
            Sut.Handle.Should().Be(ContextHandle);
        }
    }

    [TestFixture]
    public class If_the_user_releases_an_established_the_context : SCardContextSpec
    {
        protected override void EstablishContext() {
            Sut.Establish(SCardScope.User);
        }

        protected override void BecauseOf() {
            Sut.Release();
        }

        [Test]
        public void Should_it_have_an_invalid_handle() {
            Sut.Handle.Should().Be(default(IntPtr));
        }
    }

    [TestFixture]
    public class If_the_user_connects_to_a_card_handle : SCardContextSpec
    {
        private ICardHandle _cardHandle;

        protected override void EstablishContext() {
            IntPtr handle;
            SCardProtocol protocol;
            A.CallTo(() => Api.Connect(A<IntPtr>._, A<string>._, A<SCardShareMode>._, A<SCardProtocol>._, out handle,
                    out protocol))
                .WithAnyArguments()
                .Returns(SCardError.Success)
                .AssignsOutAndRefParametersLazily(_ => new object[] {(IntPtr) 123, SCardProtocol.T1});

            Sut.Establish(SCardScope.User);
        }

        protected override void BecauseOf() {
            _cardHandle = Sut.Connect("MyReader", SCardShareMode.Direct, SCardProtocol.Any);
        }

        [Test]
        public void It_should_have_the_API_returned_handle_value_of_123() {
            _cardHandle.Handle.Should().Be((IntPtr) 123);
        }

        [Test]
        public void It_should_have_the_protocol_T1() {
            _cardHandle.Protocol.Should().Be(SCardProtocol.T1);
        }

        [Test]
        public void It_should_have_the_same_ShareMode_as_requested() {
            _cardHandle.Mode.Should().Be(SCardShareMode.Direct);
        }

        [Test]
        public void It_should_have_the_reader_name_set() {
            _cardHandle.ReaderName.Should().Be("MyReader");
        }
    }
}
