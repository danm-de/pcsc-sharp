using System;
using System.Collections.Generic;
using System.Text;

using PCSC.Iso8825;
using PCSC.Iso8825.BasicEncodingRules;
using PCSC.Iso8825.Asn1;

namespace BasicEncodingRulesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // TEST IA5String, value = "test1@rsa.com"
            byte[][] ia5stringBytes = { 
                // DER encoding with short length format
                new byte[]{ 
                    0x16, 0x0d, 
                    0x74, 0x65, 0x73, 0x74, 0x31, 0x40, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d },

                // BER encoding with long length format
                new byte[] { 
                  0x16, 0x81, 0x0d,
                  0x74, 0x65, 0x73, 0x74, 0x31, 0x40, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d},

                // Constructed encoding with short length format. "test1" + "@" + "rsa.com"
                new byte[]{ 
                  0x36, 0x13,
                    0x16, 0x05, 0x74, 0x65, 0x73, 0x74, 0x31,
                    0x16, 0x01, 0x40,
                    0x16, 0x07, 0x72, 0x73, 0x61, 0x2e, 0x63, 0x6f, 0x6d}
            };

            BerTlvPacket[] ia5stringPackets = { 
                new BerTlvPacket(ia5stringBytes[0], 0),
                new BerTlvPacket(ia5stringBytes[1], 0),
                new BerTlvPacket(ia5stringBytes[2], 0) 
            };

            // Display raw information about the BER-TLV packets
            for (int i=0; i < ia5stringPackets.Length; i++)
            {
                BerTlvPacket packet = ia5stringPackets[i];
                Console.WriteLine("Packet {0} has Tag number {1} and the content length is {2}.",
                    i, packet.Tag, packet.ContentLength);
                Console.WriteLine("Class: " + packet.Class.ToString() 
                    + ", Structure type: " + packet.StructureType);
                if (packet.ContentLength > 0)
                {
                    byte[] content = packet.GetContent();
                    Console.Write("Content:");
                    foreach (byte b in content)
                        Console.Write(" {0:X2}", b);
                    Console.WriteLine("\n");
                    if (packet.StructureType == BerStructureType.Constructed)
                    {
                        BerTlvPacket[] subpackets = packet.GetEncapsulatedPackets();
                        if (subpackets != null)
                        {
                            foreach (BerTlvPacket subpacket in subpackets)
                            {
                                Console.WriteLine("  Tag number {0}, content length is {1}.",
                                    subpacket.Tag, subpacket.ContentLength);
                                Console.WriteLine("    Class: " + subpacket.Class.ToString()
                                    + ", Structure type: " + subpacket.StructureType);
                                Console.WriteLine("    Packet starts at {0} and ends at {1}. The content starts at {2}.",
                                    subpacket.PacketStartsAt, subpacket.PacketEndsAt, subpacket.ContentStartsAt);
                            }
                        }
                    }
                }
            }

            DataObject[] dataobjects = new DataObject[] {
                ia5stringPackets[0].GetDataObject(),
                ia5stringPackets[1].GetDataObject()
            };

            // Show us the content
            Console.WriteLine();
            for(int i=0; i< dataobjects.Length; i++)
            {
                if (dataobjects[i] is Asn1IA5String)
                {
                    Asn1IA5String ia5string = (Asn1IA5String)dataobjects[i];
                    Console.WriteLine("Packet {0} is from type {1} with the following content:\n\t{2}",
                        i, dataobjects[i].GetType(), ia5string.ToString());
                }
            }

            byte[][] integerBytes = {
                new byte[] { 0x02, 0x01, 0x00 },            // 0
                new byte[] { 0x02, 0x01, 0x7F },            // 127
                new byte[] { 0x02, 0x02, 0x00, 0x80 },      // 128
                new byte[] { 0x02, 0x02, 0x01, 0x00 },      // 256
                new byte[] { 0x02, 0x01, 0x80 },            // -128
                new byte[] { 0x02, 0x02, 0xFF, 0x7F }       // -129
            };

            BerTlvPacket[] integerPackets = { 
                new BerTlvPacket(integerBytes[0], 0),
                new BerTlvPacket(integerBytes[1], 0),
                new BerTlvPacket(integerBytes[2], 0),
                new BerTlvPacket(integerBytes[3], 0),
                new BerTlvPacket(integerBytes[4], 0),
                new BerTlvPacket(integerBytes[5], 0) 
            };

            for (int i = 0; i < integerPackets.Length; i++)
            {
                DataObject dataobj = integerPackets[i].GetDataObject();
                if (dataobj is Asn1Integer)
                {
                    Asn1Integer asnInt = (Asn1Integer)dataobj;
                    long tmp = asnInt.ToLong();
                    Console.WriteLine("Value of packet {0}: {1}", i, tmp);
                }
            }

            return;
        }
    }
}
