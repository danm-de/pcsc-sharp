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

namespace PCSC.Iso8825.Asn1
{
    public class Asn1IA5String : DataObject, IAsn1Type
    {
        private static Decoder asciiDec = Encoding.ASCII.GetDecoder();
        private static Encoder asciiEnc = Encoding.ASCII.GetEncoder();

        // Needed to create an empty instance for the DataObjectFactory
        public Asn1IA5String()
        { }

        public Asn1IA5String(string content)
        {

            data = ConvertToAscii(content, 0, content.Length);
            startIndex = 0;

            if (data != null)
                length = data.Length;
            else
                length = 0;
        }

        public Asn1IA5String(string content, int contentStartIndex, int length)
        {
            data = ConvertToAscii(content, contentStartIndex, length);
            startIndex = 0;

            if (data != null)
                this.length = length;
            else
                this.length = 0;
        }
        
        public Asn1IA5String(byte[] packet, int contentStartIndex, int length)
            :base(packet, contentStartIndex, length)
        {
        }

        protected static byte[] ConvertToAscii(string text, int startIndex, int length)
        {
            if (text == null)
                return null;
            if (text == string.Empty)
                return new byte[0];

            char[] charbuf = text.ToCharArray();

            if (startIndex + length > charbuf.Length)
                throw new ArgumentOutOfRangeException();
            
            int bufsize = asciiEnc.GetByteCount(charbuf, startIndex, length, true);
            byte[] buf = new byte[bufsize];

            int bytesUsed = 0, charsUsed = 0;
            bool completed = false;
            asciiEnc.Convert(charbuf, startIndex, length, buf, 0, bufsize, true, out charsUsed, out bytesUsed, out completed);

            return buf;
        }

        public override string ToString()
        {
            if (data == null)
                return null;

            if (length == 0)
                return String.Empty;

            char[] buf = null;
            int buflen = length;
            bool repeat = true;

            int bytesUsed = 0, charsUsed = 0;
            bool completed = false;

            do
            {
                buf = new char[buflen];
                try
                {
                    asciiDec.Convert(data, startIndex, length, buf, 0, buflen, true, out bytesUsed,
                        out charsUsed, out completed);

                    if (completed)
                        repeat = false;
                    else
                    {
                        if (bytesUsed != length)
                        {
                            buflen *= 2;
                            repeat = true;
                        }
                        else
                            repeat = false; // invalid ASCII string detected
                    }
                }
                catch (ArgumentException)
                {
                    // Buffer was not big enough.
                    buflen *= 2;
                    repeat = true;
                }
            } while (repeat);
            return new string(buf, 0, charsUsed);
        }

        public static implicit operator string(Asn1IA5String text)
        {
            return text.ToString();
        }
        public static implicit operator Asn1IA5String(string text)
        {
            return new Asn1IA5String(text);
        }
        public static Asn1IA5String operator +(Asn1IA5String a, Asn1IA5String b)
        {
            int bufsize = a.Length + b.Length;
            byte[] buf = new byte[bufsize];

            int pos = 0;
            if (a.data != null)
            {
                Array.Copy(a.data, a.startIndex, buf, 0, a.length);
                pos = a.length;
            }
            if (b.data != null)
            {
                Array.Copy(b.data, b.startIndex, buf, pos, b.length);
            }

            return new Asn1IA5String(buf, 0, bufsize);
        }

        #region IAsn1Type Member

        public Asn1Type GetAsn1Type()
        {
            return Asn1Type.IA5String;
        }

        #endregion

        public override long Tag
        {
            get { return (long)GetAsn1Type(); }
        }
    }
}
