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
using System.Threading;

namespace PCSC.Iso7816
{
    /* 
     *      Case    Command data    Expected response data
     *      ==============================================
     *      1       No data         No data
     *      2       No data         Data
     *      3       Data            No data
     *      4       Data            Data 
     */
    public class ResponseApdu : Apdu, ICloneable
    {
        protected byte[] response = null;
        protected int length = 0;

        private ResponseApdu() { }
        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol proto)
        {
            this.response = response;
            if (response != null)
                length = response.Length;
            
            this.isocase = isoCase;
            this.proto = proto;
        }
        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol proto, bool copy)
        {
            this.isocase = isoCase;
            this.proto = proto;

            if (response == null)
                return;
            if (copy)
            {
                length = response.Length;
                this.response = new byte[length];
                Array.Copy(response, this.response, length);
            }
            else
            {
                this.response = response;
                length = response.Length;
            }
        }
        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol proto)
        {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response.Length < length))
                throw new ArgumentOutOfRangeException("length");

            this.response = response;
            this.length = length;
            this.isocase = isoCase;
            this.proto = proto;
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol proto, bool copy)
        {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response.Length < length))
                throw new ArgumentOutOfRangeException("length");

            if (copy)
            {
                if (response != null)
                {
                    this.response = new byte[length];
                    Array.Copy(response, this.response, length);
                }
            }
            else
            {
                this.response = response;
            }
            this.length = length;
            this.isocase = isoCase;
            this.proto = proto;
        }

        public bool HasData
        {
            get
            {
                if (response != null &&
                    response.Length > 2 &&
                    length > 2)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (response == null)
                    return false;
                if (response.Length < 2 || length < 2) // a valid APDU response contains SW1 and SW2
                    return false;
                return true;
            }
        }

        public byte SW1
        {
            get
            {
                if (response != null &&
                    response.Length > 1 &&
                    length > 1)
                {
                    return response[length - 2];
                }
                else
                    throw new InvalidApduException("The response APDU is invalid.");
            }
        }
        public byte SW2
        {
            get
            {
                if (response != null &&
                    response.Length > 0 &&
                    length > 0)
                {
                    return response[length - 1];
                }
                else
                    throw new InvalidApduException("The response APDU is invalid.");

            }
        }
        public int StatusWord
        {
            get
            {
                return (((int)SW1) << 8) | (int)SW2;
            }
        }
        public byte[] GetData()
        {
            if (response == null)
                throw new InvalidApduException("The response APDU is invalid.");
            if (response.Length <= 2 ||
                length <= 2)
                return null;

            byte[] tmp = new byte[length - 2];
            Array.Copy(response, tmp, length - 2);
            return tmp;

        }
        public int DataSize
        {
            get
            {
                if (response == null ||
                    response.Length <= 2 ||
                    length <= 2)
                    return 0;
                else
                    return length - 2;

            }
        }
        public byte[] FullApdu
        {
            get
            {
                return response;
            }
        }

        public override byte[] ToArray()
        {
            if (response == null) return null;
            if (response.Length <= length)
            {
                byte[] tmp = new byte[length];
                Array.Copy(response, tmp, length);
                return tmp;
            }
            else
                throw new InvalidApduException("The response APDU is null or invalid.");
        }

        #region ICloneable Member
        public object Clone()
        {
            ResponseApdu tmp = new ResponseApdu();
            tmp.response = response;
            tmp.length = length;
            return tmp;
        }
        #endregion
    }
}