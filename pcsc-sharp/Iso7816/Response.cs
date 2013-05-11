using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PCSC.Iso7816
{
    public class Response : IEnumerable<ResponseApdu>
    {
        private readonly List<ResponseApdu> _lstRespApdu = new List<ResponseApdu>();
        private readonly List<SCardPCI> _lstRecvPci = new List<SCardPCI>();

        public ResponseApduList ResponseApduList {
            get { return new ResponseApduList(_lstRespApdu); }
        }

        public virtual byte SW1 {
            get {
                return (_lstRespApdu.Count > 0)
                    ? _lstRespApdu[_lstRespApdu.Count - 1].SW1
                    : (byte) 0;
            }
        }

        public virtual byte SW2 {
            get {
                return (_lstRespApdu.Count > 0)
                    ? _lstRespApdu[_lstRespApdu.Count - 1].SW2
                    : (byte) 0;
            }
        }

        public virtual int StatusWord {
            get { return (SW1 << 8) | SW2; }
        }

        public virtual bool HasData {
            get { return _lstRespApdu.Any(responseAdpu => responseAdpu.HasData); }
        }

        protected internal Response() {}

        protected internal void AddResponseApdu(ResponseApdu apdu) {
            _lstRespApdu.Add(apdu);
        }

        protected internal void AddRecvPci(SCardPCI recvPci) {
            _lstRecvPci.Add(recvPci);
        }

        public virtual byte[] GetData() {
            if (_lstRespApdu.Count == 1) {
                return _lstRespApdu[0].GetData();
            }

            if (_lstRespApdu.Count <= 0) {
                return null;
            }

            // calculate the required array size
            var dataSize = _lstRespApdu.Sum(apdu => apdu.DataSize);

            // build the array
            var data = new byte[dataSize];
            var position = 0;
            foreach (var apdu in _lstRespApdu) {
                var currentApduSize = apdu.DataSize;
                if (currentApduSize > 0) {
                    var currentApdu = apdu.FullApdu;
                    Array.Copy(currentApdu, 0, data, position, currentApduSize);
                    position += currentApduSize;
                }
            }
            return data;
        }

        public IEnumerator<ResponseApdu> GetEnumerator() {
            return ResponseApduList;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}