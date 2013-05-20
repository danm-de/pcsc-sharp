using System;

namespace PCSC.Iso7816
{
    /// <summary>A response APDU</summary>
    public class ResponseApdu : Apdu, ICloneable
    {
        private byte[] _response;
        private int _length;

        private ResponseApdu() {}

        /// <summary>Initializes a new instance of the <see cref="ResponseApdu" /> class.</summary>
        /// <param name="response">The response as byte array that shall be parsed.</param>
        /// <param name="isoCase">The ISO case that was used when sending the <see cref="CommandApdu" />.</param>
        /// <param name="protocol">The communication protocol.</param>
        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol) {
            _response = response;

            if (response != null) {
                _length = response.Length;
            }

            Case = isoCase;
            Protocol = protocol;
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseApdu" /> class.</summary>
        /// <param name="response">The response as byte array that shall be parsed.</param>
        /// <param name="isoCase">The ISO case that was used when sending the <see cref="CommandApdu" />.</param>
        /// <param name="protocol">The communication protocol.</param>
        /// <param name="copy">If <see langword="true" /> the bytes of the supplied response will be copied.</param>
        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            Case = isoCase;
            Protocol = protocol;

            if (response == null) {
                return;
            }

            if (copy) {
                _length = response.Length;
                _response = new byte[_length];
                Array.Copy(response, _response, _length);
            } else {
                _response = response;
                _length = response.Length;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseApdu" /> class.</summary>
        /// <param name="response">The response as byte array that shall be parsed.</param>
        /// <param name="length">The size of the response.</param>
        /// <param name="isoCase">The ISO case that was used when sending the <see cref="CommandApdu" />.</param>
        /// <param name="protocol">The communication protocol.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length" /> is greater than the <paramref name="response" /> size.</exception>
        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            _response = response;
            _length = length;
            Case = isoCase;
            Protocol = protocol;
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseApdu" /> class.</summary>
        /// <param name="response">The response as byte array that shall be parsed.</param>
        /// <param name="length">The size of the response.</param>
        /// <param name="isoCase">The ISO case that was used when sending the <see cref="CommandApdu" />.</param>
        /// <param name="protocol">The communication protocol.</param>
        /// <param name="copy">If <see langword="true" /> the bytes of the supplied response will be copied.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length" /> is greater than the <paramref name="response" /> size.</exception>
        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            if (copy) {
                if (response != null) {
                    _response = new byte[length];
                    Array.Copy(response, _response, length);
                }
            } else {
                _response = response;
            }
            _length = length;
            Case = isoCase;
            Protocol = protocol;
        }

        /// <summary>Gets a value indicating whether this response has data.</summary>
        /// <value>
        ///     <c>true</c> if this response has data; otherwise, <c>false</c>.</value>
        public bool HasData {
            get { return _response != null && _response.Length > 2 && _length > 2; }
        }

        /// <summary>Indicates if the response APDU is valid.</summary>
        /// <value>
        ///     <see langword="true" /> if the response APDU is valid.</value>
        public override bool IsValid {
            get {
                if (_response == null) {
                    return false;
                }
                return _response.Length >= 2 && _length >= 2;
            }
        }

        /// <summary>Gets the SW1 status byte.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte SW1 {
            get {
                if (_response == null || _response.Length < _length || _length <= 1) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return _response[_length - 2];
            }
        }

        /// <summary>Gets the SW1 status byte.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte SW2 {
            get {
                if (_response == null || _response.Length < _length || _length <= 0) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return _response[_length - 1];
            }
        }

        /// <summary>Gets the combination of SW1 and SW2 as 16bit status word.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public int StatusWord {
            get { return (SW1 << 8) | SW2; }
        }

        /// <summary>
        /// Gets the length of the response APDU
        /// </summary>
        public int Length {
            get { return _length; }
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        public int DataSize {
            get {
                if (_response == null || _response.Length <= 2 || _length <= 2) {
                    return 0;
                }
                return _length - 2;
            }
        }

        /// <summary>
        /// Gets the full response APDU.
        /// </summary>
        /// <value>
        /// The full APDU as byte array.
        /// </value>
        public byte[] FullApdu {
            get { return _response; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte[] GetData() {
            if (_response == null) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            if (_response.Length <= 2 ||
                _length <= 2) {
                return null;
            }

            var tmp = new byte[_length - 2];
            Array.Copy(_response, tmp, _length - 2);
            return tmp;
        }

        /// <summary>Converts the APDU structure to a transmittable byte array.</summary>
        /// <returns>A byte array containing the APDU parameters and data in the correct order.</returns>
        public override byte[] ToArray() {
            if (_response == null) {
                return null;
            }

            if (_response.Length < _length) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            var tmp = new byte[_length];
            Array.Copy(_response, tmp, _length);
            return tmp;
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>A clone of the current instance.</returns>
        public virtual object Clone() {
            var tmp = new ResponseApdu {
                _response = _response,
                _length = _length
            };
            return tmp;
        }
    }
}