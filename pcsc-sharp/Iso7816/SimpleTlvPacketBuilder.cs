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

using PCSC.Iso8825;

namespace PCSC.Iso7816
{
    public class SimpleTlvPacketBuilder: TlvData
    {
        private byte[] packet;
        private byte tag;

        public byte Tag
        {
            get { return tag; }
            set {
                if (value == 0 || value == 255)
                    throw new ArgumentException("0 and 255 are not allowed by ISO/IEC 7816.");
                tag = value; 
            }
        }
        public SimpleTlvPacketBuilder()
        { }

        public byte[] Content
        {
            get { return packet; }
            set { packet = value; }
        }
        public override int ContentLength
        {
            get 
            {
                if (packet == null || packet.Length == 0)
                    return 0;
                else
                    return packet.Length;
            }
        }

        public override byte[] GetContent()
        {
            return packet;
        }

        public override byte[] ToArray()
        {
            int len = (int)ContentLength;

            if (len > 65535)
                throw new InvalidTlvDataException(
                    "Data buffer too big. ISO/IEC 7816 allows values between 0 and 65535 only.",
                    new OverflowException("Data buffer too big. ISO/IEC 7816 allows values between 0 and 65535 only."));

            byte[] outdata = null;
            int index = 0;
            if (len < 255)
            {
                // Case 1: Length < 255, 1 Tag byte, 1 Length byte, n data bytes
                outdata = new byte[len + 2];
                outdata[index++] = tag;
                outdata[index++] = (byte)len;
            }
            else
            {
                // Case 2: Length >= 255, 1 Tag byte, 3 Length bytes, n data bytes
                outdata = new byte[len + 4];
                outdata[index++] = tag;
                outdata[index++] = 255; // B2 = 255 indicates that the size of n bytes is coded in B3+B4
                outdata[index++] = (byte)((0xFF00 & len) >> 8);
                outdata[index++] = (byte)((0xFF & len));
            }
            
            // Copy the data
            if (len > 0)
                Array.Copy(packet, 0, outdata, index, len);
            
            return outdata;
        }
    }
}
