using System.Collections.Generic;
using NUnit.Framework;

namespace PCSC.Iso7816.Tests.CommandApduSpecs
{
    [TestFixture]
    public class If_the_user_calls_ToArray_on_a_command_APDU
    {
        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return new TestCaseData(
                        new CommandApdu(IsoCase.Case4Extended, SCardProtocol.T1) {
                            CLA = 0x00,
                            INS = 0xCB, // GET CHUID
                            P1 = 0x3F, // Parameter 1
                            P2 = 0xFF, // Parameter 2   Skip the FCI
                            Le = 0x00, // Expected length of the returned data
                            Data = new byte[] {0x5C, 0x03, 0x5F, 0xC1, 0x02}
                        })
                    .SetName("GET CHUID")
                    .Returns(new[] {
                        0x00, 0xcb, 0x3f, 0xff,
                        0x00, 0x00, 0x05,
                        0x5c, 0x03, 0x5f, 0xc1, 0x02,
                        0x00, 0x00
                    });
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public byte[] Should_the_result_match_the_expected_result(Apdu apdu) {
            return apdu.ToArray();
        }
    }
}
