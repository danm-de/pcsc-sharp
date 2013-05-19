using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PCSC.Iso7816
{
    public class Response : IEnumerable<ResponseApdu>
    {
        private readonly List<ResponseApdu> _lstResponseApdu = new List<ResponseApdu>();
        private readonly List<SCardPCI> _lstReceivePci = new List<SCardPCI>();

        public ResponseApdu this[int index] {
            get { return _lstResponseApdu[index]; }
        }

        public virtual byte SW1 {
            get {
                return (_lstResponseApdu.Count > 0)
                    ? _lstResponseApdu[_lstResponseApdu.Count - 1].SW1
                    : (byte) 0;
            }
        }

        public virtual byte SW2 {
            get {
                return (_lstResponseApdu.Count > 0)
                    ? _lstResponseApdu[_lstResponseApdu.Count - 1].SW2
                    : (byte) 0;
            }
        }

        public virtual int Count {
            get { return _lstResponseApdu.Count; }
        }

        public virtual int PciCount {
            get { return _lstReceivePci.Count; }
        }

        public virtual int StatusWord {
            get { return (SW1 << 8) | SW2; }
        }

        public virtual bool HasData {
            get { return _lstResponseApdu.Any(responseAdpu => responseAdpu.HasData); }
        }

        protected internal Response() {}

        protected internal void Add(ResponseApdu responseApdu) {
            _lstResponseApdu.Add(responseApdu);
        }

        protected internal void Add(SCardPCI receivePci) {
            _lstReceivePci.Add(receivePci);
        }

        public virtual byte[] GetData() {
            if (_lstResponseApdu.Count == 1) {
                return _lstResponseApdu[0].GetData();
            }

            if (_lstResponseApdu.Count <= 0) {
                return null;
            }

            // calculate the required array size
            var dataSize = _lstResponseApdu.Sum(apdu => apdu.DataSize);

            // build the array
            var data = new byte[dataSize];
            var position = 0;
            foreach (var apdu in _lstResponseApdu) {
                var currentApduSize = apdu.DataSize;
                if (currentApduSize > 0) {
                    var currentApdu = apdu.FullApdu;
                    Array.Copy(currentApdu, 0, data, position, currentApduSize);
                    position += currentApduSize;
                }
            }
            return data;
        }

        public ResponseApdu Get(int index) {
            return _lstResponseApdu[index];
        }

        public SCardPCI GetPci(int index) {
            return _lstReceivePci[index];
        }

        public IEnumerator<ResponseApdu> GetEnumerator() {
            return new ResponseApduEnumerator(_lstResponseApdu);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}