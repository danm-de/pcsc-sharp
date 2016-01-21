using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PCSC.Iso7816
{
    /// <summary>An aggregation of <see cref="ResponseApdu" /> instances.</summary>
    /// <remarks>When using <see cref="IIsoReader.Transmit(CommandApdu)" /> the result can consist of one or more <see cref="ResponseApdu" />. If the <see cref="IIsoReader" /> receives a SW1=0x61 status word, it will automatically transmit a GET RESPONSE command to reader (after waiting <see cref="IIsoReader.RetransmitWaitTime" /> ms) to catch all remaining <see cref="ResponseApdu" />.</remarks>
    public class Response : IEnumerable<ResponseApdu>
    {
        private readonly List<ResponseApdu> _lstResponseApdu = new List<ResponseApdu>();
        private readonly List<SCardPCI> _lstReceivePci = new List<SCardPCI>();

        /// <summary>Gets the <see cref="ResponseApdu" /> at the specified index.</summary>
        /// <value>The <see cref="ResponseApdu" />.</value>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ResponseApdu" /> at the specified index.</returns>
        public ResponseApdu this[int index] => _lstResponseApdu[index];

        /// <summary>The SW1 status of the last received <see cref="ResponseApdu" />.</summary>
        public virtual byte SW1 => (_lstResponseApdu.Count > 0)
            ? _lstResponseApdu[_lstResponseApdu.Count - 1].SW1
            : (byte) 0;

        /// <summary>The SW2 status of the last received <see cref="ResponseApdu" />.</summary>
        public virtual byte SW2 => (_lstResponseApdu.Count > 0)
            ? _lstResponseApdu[_lstResponseApdu.Count - 1].SW2
            : (byte) 0;

        /// <summary>Gets the number of received <see cref="ResponseApdu" />.</summary>
        public virtual int Count => _lstResponseApdu.Count;

        /// <summary>Gets the number of received <see cref="SCardPCI" />.</summary>
        public virtual int PciCount => _lstReceivePci.Count;

        /// <summary>A combination of SW1 and SW2 as 16bit status word.</summary>
        /// <remarks>It contains the status word of the last received <see cref="ResponseApdu" />.</remarks>
        public virtual int StatusWord => (SW1 << 8) | SW2;

        /// <summary>
        ///     <see langword="true" /> if at least one <see cref="ResponseApdu" /> contains data bytes.</summary>
        public virtual bool HasData {
            get { return _lstResponseApdu.Any(responseAdpu => responseAdpu.HasData); }
        }

        /// <summary>Initializes a new instance of the <see cref="Response" /> class.</summary>
        protected internal Response() {}

        /// <summary>Adds the specified response APDU.</summary>
        /// <param name="responseApdu">The response APDU.</param>
        protected internal void Add(ResponseApdu responseApdu) {
            _lstResponseApdu.Add(responseApdu);
        }

        /// <summary>Adds the specified PCI.</summary>
        /// <param name="receivePci">The PCI.</param>
        protected internal void Add(SCardPCI receivePci) {
            _lstReceivePci.Add(receivePci);
        }

        /// <summary>An aggregation of all data bytes in receive order.</summary>
        /// <returns>An aggregation of all data bytes from all <see cref="ResponseApdu" />.</returns>
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

        /// <summary>Gets the <see cref="ResponseApdu" /> of the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ResponseApdu" /> of the specified index.</returns>
        public ResponseApdu Get(int index) {
            return _lstResponseApdu[index];
        }

        /// <summary>Gets the <see cref="SCardPCI" /> of the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="SCardPCI" /> of the specified index.</returns>
        public SCardPCI GetPci(int index) {
            return _lstReceivePci[index];
        }

        /// <summary>Returns an enumerator.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" />, that enumerates all received <see cref="ResponseApdu" /> instance.</returns>
        public IEnumerator<ResponseApdu> GetEnumerator() {
            return new ResponseApduEnumerator(_lstResponseApdu);
        }

        /// <summary>Returns an enumerator.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" />, that enumerates all received <see cref="ResponseApdu" /> instance.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}