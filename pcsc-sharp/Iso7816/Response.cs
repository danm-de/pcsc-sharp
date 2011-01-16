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
    public class ResponseApduList : IEnumerable<ResponseApdu>, IEnumerator<ResponseApdu>
    {
        private List<ResponseApdu> lstRespApdu = new List<ResponseApdu>();
        int position = -1;

        public ResponseApduList(List<ResponseApdu> lst)
        {
            lstRespApdu = lst;
        }
        public ResponseApduList(ResponseApdu[] lst)
        {
            lstRespApdu = new List<ResponseApdu>();
            lstRespApdu.AddRange(lst);
        }

        #region IEnumerable Member

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable<ResponseApdu> Member

        IEnumerator<ResponseApdu> IEnumerable<ResponseApdu>.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator<ResponseApdu> Member

        public ResponseApdu Current
        {
            get
            {
                try
                {
                    return lstRespApdu[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            // do nothing
        }

        #endregion

        #region IEnumerator Member

        object System.Collections.IEnumerator.Current
        {
            get { return ((ResponseApdu)Current); }
        }

        public bool MoveNext()
        {
            position++;
            return (position < lstRespApdu.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        #endregion
    }
   
    public class Response
    {
        private List<ResponseApdu> _lstRespApdu = new List<ResponseApdu>();
        private List<SCardPCI> _lstRecvPci = new List<SCardPCI>();

        internal Response()
        {  }
 
        private ResponseApduList _respApduList = null;
        public ResponseApduList ResponseApduList
        {
            get
            {
                if (_respApduList == null)
                    _respApduList = new ResponseApduList(_lstRespApdu);
                return _respApduList;
            }
        }

        protected internal void AddResponseApdu(ResponseApdu apdu)
        {
            _lstRespApdu.Add(apdu);
        }
        protected internal void AddRecvPci(SCardPCI recvPci)
        {
            _lstRecvPci.Add(recvPci);
        }
        public virtual byte[] GetData()
        {
            if (_lstRespApdu.Count == 1)
            {
                return _lstRespApdu[0].GetData();
            }
            else if (_lstRespApdu.Count > 0)
            {
                int dataSize = 0;
                
                // calculate the required array size
                foreach (ResponseApdu apdu in _lstRespApdu)
                    dataSize += apdu.DataSize;
                
                // build the array
                byte[] data = new byte[dataSize];
                int position = 0;
                foreach (ResponseApdu apdu in _lstRespApdu)
                {
                    int currentApduSize = apdu.DataSize;
                    if (currentApduSize > 0)
                    {
                        byte[] currentApdu = apdu.FullApdu;
                        Array.Copy(currentApdu, 0, data, position, currentApduSize);
                        position += currentApduSize;
                    }
                }
                return data;
            }
            
            // NoDataExcepton?
            return null;
        }
        public virtual byte SW1
        {
            get 
            {
                if (_lstRespApdu.Count > 0)
                {
                    // return the last status word SW1
                    return _lstRespApdu[_lstRespApdu.Count - 1].SW1;
                }
                else
                    return 0; // TODO
            }
        }
        public virtual byte SW2
        {
            get 
            {
                if (_lstRespApdu.Count > 0)
                {
                    // return the last status word SW2
                    return _lstRespApdu[_lstRespApdu.Count - 1].SW2;
                }
                else
                    return 0; // TODO
            }
        }
        public virtual int StatusWord
        {
            get
            {
                return (((int)SW1) << 8) | (int)SW2;
            }
        }
    }
}
