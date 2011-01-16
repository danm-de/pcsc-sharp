using System;
using System.Collections.Generic;
using System.Text;

using PCSC.Iso8825;
using PCSC.Iso8825.BasicEncodingRules;

namespace BerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // TEST IA5String, value = "test1@rsa.com"

            // DER encoding
            byte[] ia5stringBytes1 = { 0x16, 0x0d, 0x74, 0x65, 0x73, 0x74, 0x31, 0x40, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d };

            // BER encoding with long form of length octets
            byte[] ia5stringBytes2 = { 0x16, 0x81, 0x0d,
                0x74, 0x65, 0x73, 0x74, 0x31, 0x40, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d};

            // constructed encoding. "test1" + "@" + "rsa.com"
            byte[] ia5stringBytes3 = {
                0x36, 0x13,
                0x16, 0x05, 0x74, 0x65, 0x73, 0x74, 0x31,
                0x16, 0x01, 0x40,
                0x16, 0x07, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d};

            BerTlvPacket ia5string1 = new BerTlvPacket(ia5stringBytes1, 0);
            BerTlvPacket ia5string2 = new BerTlvPacket(ia5stringBytes2, 0);
            BerTlvPacket ia5string3 = new BerTlvPacket(ia5stringBytes3, 0);
            return;
        }
    }
}
