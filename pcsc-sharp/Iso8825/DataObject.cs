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

namespace PCSC.Iso8825
{
    public abstract class DataObject: IEnumerator<byte>
    {
        protected DataObject()
        {
        }

        public DataObject(byte[] data, int startIndex, int length)
        {
            if (data == null)
            {
                if (startIndex > 0 || length > 0)
                    throw new ArgumentNullException("data");
            }
            else
            {
                if (data.Length < startIndex + length)
                    throw new ArgumentOutOfRangeException();
            }

            this.data = data;
            this.startIndex = startIndex;
            this.length = length;
        }

        protected byte[] data;
        protected int startIndex;
        protected int length;

        public int StartsAt
        {
            get { return startIndex; }
        }

        public int EndsAt
        {
            get
            {
                if (length == 0)
                    return startIndex;
                else
                    return startIndex + length - 1;
            }
        }

        public int Length
        {
            get { return length; }
        }

        public byte[] ToArray()
        {
            if (length == 0)
                return null;
            
            // create a copy
            byte[] tmp = new byte[length];
            Array.Copy(data, startIndex, tmp, 0, length);
            return tmp;
        }

        #region IEnumerator<byte> Member
        private int position = -1;
        public virtual byte Current
        {
            get
            {
                try
                {
                    if (position == -1)
                        throw new IndexOutOfRangeException();
                    
                    return data[startIndex + position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
        #region IEnumerator Member

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public virtual bool MoveNext()
        {
            position++;
            return (position + startIndex < length);
        }

        public virtual void Reset()
        {
            position = -1;
        }

        #endregion

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            // do nothing
        }

        #endregion

        public abstract long Tag
        {
            get;
        }
        
    }
}
