using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PCSC.Iso7816;

namespace PCSC.Tests.Iso7816.ResponseSpecs
{
    [TestFixture]
    public class When_creating_a_Response_using_the_public_constructor : Spec
    {
        private ResponseApdu[] _expectedApdus;
        private SCardPCI[] _expectedPcis;
        private Response _sut;
        
        protected override void EstablishContext() {
            _expectedApdus = new[] {
                new ResponseApdu(new byte[] {0x1, 0x2}, 2, IsoCase.Case1, SCardProtocol.T0),
                new ResponseApdu(new byte[] {0x3, 0x4}, 2, IsoCase.Case1, SCardProtocol.T0),
            };

            _expectedPcis = new[] {
                new SCardPCI(SCardProtocol.T0, 0)
            };
        }

        protected override void BecauseOf() {
            _sut = new Response(_expectedApdus, _expectedPcis);
        }

        [Test]
        public void It_should_contain_all_ResponseApdus() {
            _sut.ToArray()
                .ShouldBeEquivalentTo(_expectedApdus);
        }

        [Test]
        public void It_should_contain_all_SCardPCIs() {
            Enumerable.Range(0, _sut.PciCount)
                .Select(i => _sut.GetPci(i))
                .ShouldBeEquivalentTo(_expectedPcis);
        }
    }
}