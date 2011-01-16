/*
Copyright (C) 2010
    Daniel Mueller <daniel@danm.de>

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;

//using System.DirectoryServices.Protocols;
using PCSC.Iso8825;

namespace PCSC.Iso8825.BasicEncodingRules
{
    // Basic Encoding Rule Tuple, see ISO/IEC 8825-1
    public class BerTlvPacket: TlvData
    {
        private const byte BER_CLASS_MASK                   = 0xC0; // B8,B7 = 11
        private const byte BER_NUMBER_MASK                  = 0x1F; // B5,B4,B3,B2,B1 = 11111
        private const byte BER_TAG_TYPE_MASK                = 0x20; // B6 = 1
        
        private const byte BER_ANOTHER_TAG_BYTE_FOLLOWS     = 0x80; // B8 = 1
        private const byte BER_HIGH_TAG_NUMBER_MASK         = 0x7F; // B7,B6,B5,B4,B3,B2,B1 = 1111111

        private const byte BER_LENGTH_BYTE_IS_IN_LONG_FORM  = 0x80; // B8 = 1
        private const byte BER_LENGTH_IS_IN_INDEFINITE_FORM = 0x80; // B8 = 1
        private const byte BER_LENGTH_SHORT_FORM_MASK       = 0x7F; // B7,B6,B5,B4,B3,B2,B1 = 1111111
     
        protected BerTlvPacket() { }

        public BerTlvPacket(byte[] packet)
        {
            this.packet = packet;
            this.packetStartIndex = 0;
            AnalyzeBerTlv();
        }
        public BerTlvPacket(byte[] packet, int packetStart)
        {
            this.packet = packet;
            this.packetStartIndex = packetStart;
            AnalyzeBerTlv();
        }

        private void AnalyzeBerTlv()
        {
            if (packet == null || packet.Length <= packetStartIndex)
                throw new InvalidBerTlvPacketException();

            // Size of the byte array 
            int packetSize = packet.Length;

            /*
             * Step 1: Analyze the initial tag byte:
             * 
             ***********************************************
             * 8  7     6     5  4  3  2  1  * Bit number  *
             ***********************************************
             * Class * P/C *  Tag number     * Description *
             ***********************************************/

            // First we analyze the leading tag byte (identifer octet)
            byte leadingOctet = packet[packetStartIndex + 0];

            // Class of tag
            berClass = (BerClassType)(leadingOctet & BER_CLASS_MASK);
            
            // Primitive or constructed type
            structureType = (BerStructureType)(leadingOctet & BER_TAG_TYPE_MASK);

            // Is the tag number greater then 30?
            hasHighTagNumber = ((leadingOctet & BER_NUMBER_MASK) == BER_NUMBER_MASK);

            // Counts the (subsequent) octets
            int octetcounter = packetStartIndex + 1;

            // Stores a tempory copy of the subsequent octet 
            byte subsequent;

            if (hasHighTagNumber)
            {
                /* Oh duh, a high tag number follows..
                 * We try to store it in 'long' (which is 64bit).
                 */

                /*
                 * |Leading octet | -> |2nd octet|  ..  |last octet|
                 * |Class|PC|11111| -> |1|xxxxxxx|  ..  |0| xxxxxxx|
                 * 
                 * Concatenates the bits  xxxxxxx + ..  +   xxxxxxx = Number of tag
                 */

                // The high tag number
                long bigtag = 0;

                do // repeat until no further subsequent byte follows
                {
                    if (octetcounter >= packetSize)
                        throw new InvalidBerTlvPacketException("Subsequent tag byte " + (octetcounter) + " missing.");
                    subsequent = packet[octetcounter++];

                    // only bits 7 to 1 are relevant
                    byte v = (byte)(subsequent & BER_HIGH_TAG_NUMBER_MASK);

                    if ((octetcounter - packetStartIndex) > 9)
                    {
                        // we reached the 64bit limit of 'long'
                        tagOverflow = true;
                    }
                    else
                    {
                        // shift 
                        bigtag = bigtag << 7;

                        // add new value
                        bigtag |= (long)v;
                    }
                } while ((subsequent & BER_ANOTHER_TAG_BYTE_FOLLOWS) == BER_ANOTHER_TAG_BYTE_FOLLOWS);

                tag = bigtag;
            }
            else
            {
                tag = (leadingOctet & BER_NUMBER_MASK);
            }

            /*
             * Step 2: Analyze the (content) Length field.
             */

            /* octetcounter points to the Length data field now! */

            if (octetcounter >= packetSize)
                throw new InvalidBerTlvPacketException("Length byte missing.");
            subsequent = packet[octetcounter];

            contentLength = 0;

            // Is the length field in definite or indefinite form?
            if (structureType == BerStructureType.Constructed &&
                subsequent == BER_LENGTH_IS_IN_INDEFINITE_FORM )
            {
                // Length octets are in indefinite form
                lengthEncoding = BerLengthEncoding.IndefiniteForm;
                octetcounter++;
            }
            else
            {
                // Length octets are in definite form
                lengthEncoding = BerLengthEncoding.DefiniteForm;

                if (subsequent == 255)
                    throw new InvalidBerTlvPacketException("Invalid length byte with all bits set to 1 found.");

                if ((subsequent & BER_LENGTH_BYTE_IS_IN_LONG_FORM) == BER_LENGTH_BYTE_IS_IN_LONG_FORM)
                {
                    // length field is in long form
                    // B7-B1 encodes the number of bytes followed to calculate the content length
                    int numberOfLenOctets = (subsequent & BER_LENGTH_SHORT_FORM_MASK);

                    // are we able to handle the content size?
                    if (numberOfLenOctets > sizeof(int))
                        throw new InvalidBerTlvPacketException(
                            "The content length is to big: 2^(" + (numberOfLenOctets * 8) + ")", 
                            new OverflowException("The content length is to big: 2^(" + (numberOfLenOctets * 8) + ")"));

                    octetcounter++; // next octet
                    for (int i = 1; i <= numberOfLenOctets; i++)
                    {
                        if (octetcounter >= packetSize)
                            throw new InvalidBerTlvPacketException("Length byte(s)missing.");

                        subsequent = packet[octetcounter++];
  
                        // shift
                        contentLength = (contentLength << 8);
                        contentLength |= (int)subsequent;
                    }
                }
                else
                {
                    // length field is in short form
                    contentLength = (subsequent & BER_LENGTH_SHORT_FORM_MASK);
                    octetcounter++;
                }
            }

            /*
             * Step 3: Analyze the content (or update the content length variable).
             */

            /* If dataLength > 0 (or lengthEncoding is in indefinite form)
             * then octetcounter points to the first content byte. */

            if (lengthEncoding == BerLengthEncoding.DefiniteForm &&
                contentLength == 0)
            {
                contentStartIndex = octetcounter; // No content
            }
            else if (lengthEncoding == BerLengthEncoding.DefiniteForm)
            {
                if ((packetSize - (octetcounter)) < contentLength)
                    throw new InvalidBerTlvPacketException("The specified content length is bigger than the remaining data bytes.");
                contentStartIndex = octetcounter;
            }
            else
            {
                // We need to calculate the dataLength.
                contentStartIndex = octetcounter;

                bool firstZero = false;
                do // repeat until the 2 Zero bytes appear
                {
                    if (octetcounter >= packetSize)
                        throw new InvalidBerTlvPacketException("The End-Of-Content bytes are missing.");

                    subsequent = packet[octetcounter++];
                    if (subsequent != 0)
                    {
                        contentLength++;
                        if (firstZero)
                        {
                            contentLength++;
                            firstZero = false;
                        }
                    } else {
                        if (firstZero == true)
                            break; // two zero octets found -> End-Of-Content
                        else
                            firstZero = true;
                    }
                } while (true);
            }
        }

        private BerClassType berClass;
        public BerClassType Class
        {
            get { return berClass; }
        }

        private BerStructureType structureType;
        public BerStructureType StructureType
        {
            get { return structureType; }
        }

        private BerLengthEncoding lengthEncoding;
        public BerLengthEncoding LengthEncoding
        {
            get { return lengthEncoding; }
        }

        private bool hasHighTagNumber;
        public bool HasHighTagNumber
        {
            get { return hasHighTagNumber; }
        }

        private bool tagOverflow;
        public bool TagOverflow
        {
            get { return tagOverflow; }
        }

        private long tag;
        public long Tag
        {
            get { return tag; }
        }

        private int contentLength;
        public override int ContentLength
        {
            get { return contentLength; }
        }

        public int PacketEndsAt
        {
            get 
            {
                if (lengthEncoding == BerLengthEncoding.DefiniteForm)
                {

                    return contentStartIndex + contentLength - 1;
                }
                else
                {
                    return contentStartIndex + contentLength - 1 + 2; // two Zero bytes
                }
            }
        }
        public int PacketSize
        {
            get
            {
                return PacketEndsAt - packetStartIndex + 1;
            }
        }

        public BerTlvPacket[] GetEncapsulatedPackets()
        {
            if (structureType == BerStructureType.Constructed)
            {
                List<BerTlvPacket> lst = new List<BerTlvPacket>();
                int iLen = contentLength;
                int iStart = contentStartIndex;
                int iEnd = iStart + iLen;
                int octetcounter = iStart;
                while (iStart < iEnd) {
                    BerTlvPacket tmppacket = new BerTlvPacket(packet, iStart);
                    lst.Add(tmppacket);
                    iStart = iStart + tmppacket.PacketSize;
                }
                return lst.ToArray();
            }
            else
                return null; // only constructed packets contain further BerTlvPackets
        }

        public DataObject GetDataObject()
        {
            if (structureType == BerStructureType.Constructed)
                throw new InvalidOperationException("Constructed data is not supported by this method.");

            if (!DataObjectFactory.CanCreate(tag))
                throw new NotSupportedException("Object creation for tag " + tag.ToString() + " is not supported.");

            DataObject obj = DataObjectFactory.CreateInstance(
                tag, 
                packet, 
                contentStartIndex, 
                contentLength);

            return obj;
        }

        private int contentStartIndex;
        public int ContentStartsAt
        {
            get { return contentStartIndex; }
        }

        private int packetStartIndex;
        public int PacketStartsAt
        {
            get { return packetStartIndex; }
        }

        private byte[] packet;
        public override byte[] GetContent()
        {
            byte[] tmp = null;
            if (contentLength > 0)
            {
                tmp = new byte[contentLength];
                Array.Copy(packet, contentStartIndex, tmp, 0, contentLength);
            }
            return tmp;
        }

        public override byte[] ToArray()
        {
            byte[] tmp;
            int len = PacketSize;
            tmp = new byte[len];
            Array.Copy(packet, contentStartIndex, tmp, 0, len);
            return tmp;
        }
    }
}
