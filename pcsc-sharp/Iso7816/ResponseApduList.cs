using System;
using System.Collections;
using System.Collections.Generic;

namespace PCSC.Iso7816
{
    public class ResponseApduList : IEnumerable<ResponseApdu>, IEnumerator<ResponseApdu>
    {
        private readonly List<ResponseApdu> _lstRespApdu = new List<ResponseApdu>();
        private int _position = -1;

        public ResponseApdu this[int index] {
            get { return _lstRespApdu[index]; }
        }

        public int Count {
            get { return _lstRespApdu.Count; }
        }

        public ResponseApdu Current {
            get {
                try {
                    return _lstRespApdu[_position];
                } catch (IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }
        
        object IEnumerator.Current {
            get { return Current; }
        }

        public ResponseApduList(List<ResponseApdu> lst) {
            _lstRespApdu = lst;
        }

        public ResponseApduList(IEnumerable<ResponseApdu> apdus) {
            _lstRespApdu = new List<ResponseApdu>(apdus);
        }

        public ResponseApduList(ResponseApdu[] apdus) {
            _lstRespApdu = new List<ResponseApdu>(apdus);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this;
        }

        public IEnumerator<ResponseApdu> GetEnumerator() {
            return this;
        }

        public bool MoveNext() {
            _position++;
            return (_position < _lstRespApdu.Count);
        }

        public void Reset() {
            _position = -1;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {}
    }
}