using FakeItEasy;
using NUnit.Framework;

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
}
