using System;
using System.Collections;
using System.Collections.Generic;

namespace PCSC.Iso7816
{
    /// <summary>
    /// A <see cref="ResponseApdu"/> enumerator.
    /// </summary>
    public class ResponseApduEnumerator : IEnumerable<ResponseApdu>, IEnumerator<ResponseApdu>
    {
        private readonly List<ResponseApdu> _lstRespApdu;
        private int _position = -1;

        /// <summary>
        /// Returns the current element.
        /// </summary>
        /// <returns>The current element.</returns>
        /// <exception cref="System.InvalidOperationException"> if the enumerator reaches the end and therefore <see cref="Current"/> is invalid.</exception>
        public ResponseApdu Current {
            get {
                try {
                    return _lstRespApdu[_position];
                } catch (IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Returns the current element.
        /// </summary>
        /// <returns>The current element.</returns>
        /// <exception cref="System.InvalidOperationException"> if the enumerator reaches the end and therefore <see cref="Current"/> is invalid.</exception>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseApduEnumerator"/> class.
        /// </summary>
        /// <param name="lst">A list of <see cref="ResponseApdu"/>.</param>
        public ResponseApduEnumerator(List<ResponseApdu> lst) {
            _lstRespApdu = lst;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseApduEnumerator"/> class.
        /// </summary>
        /// <param name="apdus">An enumeration of <see cref="ResponseApdu"/>.</param>
        public ResponseApduEnumerator(IEnumerable<ResponseApdu> apdus) {
            _lstRespApdu = new List<ResponseApdu>(apdus);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseApduEnumerator"/> class.
        /// </summary>
        /// <param name="apdus">An array of <see cref="ResponseApdu"/>.</param>
        public ResponseApduEnumerator(ResponseApdu[] apdus) {
            _lstRespApdu = new List<ResponseApdu>(apdus);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.IEnumerator" /> objekt, that can be used to enumerate through all <see cref="ResponseApdu"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this;
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.IEnumerator" /> objekt, that can be used to enumerate through all <see cref="ResponseApdu"/>.
        /// </returns>
        public IEnumerator<ResponseApdu> GetEnumerator() {
            return this;
        }

        /// <summary>
        /// Sets the enumerator the the next element.
        /// </summary>
        /// <returns><c>true</c> if there exists another element. Otherwise <c>false</c>.</returns>
        public bool MoveNext() {
            _position++;
            return (_position < _lstRespApdu.Count);
        }

        /// <summary>
        /// Resets the enumerator to the beginning.
        /// </summary>
        public void Reset() {
            _position = -1;
        }

        /// <summary>
        /// Disposes the enumerator and releases all managed resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {}
    }
}