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
    public class InstructionByte
    {
        protected byte ins = 0;

        public InstructionByte(InstructionCode code)
        {
            this.ins = (byte)code;
        }
        protected internal InstructionByte(byte ins)
        {
            this.ins = ins;
        }

        public InstructionCode Code
        {
            get
            {
                return (InstructionCode)ins;
            }
            set
            {
                this.ins = (byte)value;
            }
        }

        public byte Value
        {
            get { return ins; }
            set { ins = value; }
        }

        public static implicit operator byte(InstructionByte insByteInfo)
        {
            return insByteInfo.Value;
        }
        public static implicit operator InstructionByte(byte b)
        {
            return new InstructionByte(b);
        }
        public static implicit operator InstructionByte(InstructionCode code)
        {
            return new InstructionByte(code);
        }
    }
}
