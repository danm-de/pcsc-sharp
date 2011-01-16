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

namespace PCSC.Iso7816
{
    public class ClassByte
    {
        private const byte SECURE_MESSAGING_MASK       = 0x4 + 0x8;
        private const byte LOGICAL_CHANNEL_NUMBER_MASK = 0x1 + 0x2;

        private byte cla = 0;
        public ClassByte(byte Cla)
        {
            this.cla = Cla;
        }
        public ClassByte(ClaHighPart highPart, SecureMessagingFormat secMsgFmt, int logChannel)
        {
            if (logChannel > 3 || logChannel < 0)
                throw new ArgumentOutOfRangeException(
                    "logChannel",
                    "Logical channels must be in the range between 0 and 3.");
            this.cla = (byte)((int)highPart | (int)secMsgFmt | logChannel);
        }

        public byte Value
        {
            get { return cla; }
            set { cla = value; }
        }

        public ClaHighPart HighPart
        {
            get
            {
                // get the high part (b8,b7,b6,b5)
                byte h = (byte)(0xF0 & cla);

                // return the high part
                return (ClaHighPart)h;
            }
            set
            {
                // save the low part (b4,b3,b2,b1)
                byte l = (byte)(0x0F & cla);

                // combine the user specified high part with the saved low part
                cla = (byte)((int)value | (int)l);
            }
        }
        public SecureMessagingFormat Security
        {
            get
            {
                byte sec = (byte)(cla & SECURE_MESSAGING_MASK);
                return (SecureMessagingFormat)sec;
            }
            set
            {
                byte inversemask;
                unchecked
                {
                    inversemask = (byte)(~(SECURE_MESSAGING_MASK));
                }
                // save old settings
                byte tmp = (byte)(cla & inversemask);
                
                // set new Secure Messaging Format
                cla = (byte)((int)tmp | (int)value);
            }
        }

        public int LogicalChannel
        {
            get
            {
                int logch = (cla & LOGICAL_CHANNEL_NUMBER_MASK);
                return logch;
            }
            set
            {
                if (value > 3 || value < 0)
                    throw new ArgumentException(
                        "Logical channels must be in the range between 0 and 3.",
                        new OverflowException());

                byte inversemask;
                unchecked
                {
                    inversemask = (byte)(~(LOGICAL_CHANNEL_NUMBER_MASK));
                }
                // save old settings
                byte tmp = (byte)(cla & inversemask);

                // set new logical channel setting
                cla = (byte)((int)tmp | (int)value);
            }
        }

        public static implicit operator byte(ClassByte bInfo)
        {
            return bInfo.cla;
        }
        public static implicit operator ClassByte(byte b)
        {
            return new ClassByte(b);
        }

    }
}
