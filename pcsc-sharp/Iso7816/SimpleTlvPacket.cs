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
using PCSC.Iso8825.BasicEncodingRules;

namespace PCSC.Iso7816
{
    public class SimpleTlvPacket: TlvData
    {
        private byte[] packet;

        protected SimpleTlvPacket() { }
        public SimpleTlvPacket(byte[] tlvPacket)
        {
            this.packet = tlvPacket;
        }
        
        public byte Tag
        {
            get 
            {
                if (packet == null || packet.Length == 0)
                    throw new InvalidSimpleTlvPacketException();
                else
                    // tag is coded in B1
                    return packet[0];
            }
        }

        public override int ContentLength
        {
            get
            {
                if (packet == null || packet.Length == 0)
                    return 0;
                else
                {
                    if (packet.Length > 1)
                    {
                        // Check if Length < 255 (B2)
                        if (packet[1] < 255)
                            return packet[1];
                        if (packet.Length > 3)
                        { // B2 = 255
                            // 16bit value is coded in B3 and B4
                            int length = (((int)packet[2]) << 8) + (int)packet[3];
                            return length;
                        }
                        throw new InvalidSimpleTlvPacketException("The SIMPLE-TLV object is invalid. The Length bytes are incorrect.");
                    }
                    return 0;
                }
            }
        }

        public override byte[] GetContent()
        {
            int len = (int)ContentLength;
            byte[] tmp = null;
            
            if (len < 255)
            {
                // Length < 255, 1 Tag byte, 1 Length byte, n data bytes
                if (packet.Length < (len + 2))
                    throw new InvalidSimpleTlvPacketException("Invalid Length field.");
                else
                {
                    tmp = new byte[len];
                    Array.Copy(packet, 2, tmp, 0, len);
                }
            }
            else
            {
                // Length >= 255, 1 Tag byte, 3 Length bytes, n data bytes
                if (packet.Length < (len + 4))
                    throw new InvalidSimpleTlvPacketException("Invalid Length field.");
                else 
                {
                    tmp = new byte[len];
                    Array.Copy(packet, 4, tmp, 0, len);
                }
            }
            return tmp;
        }

        public override byte[] ToArray()
        {
            return packet;
        }
    }
}
