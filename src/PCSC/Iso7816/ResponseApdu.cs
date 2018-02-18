using System;

namespace PCSC.Iso7816
{
    /// <summary>A response APDU</summary>
    public class ResponseApdu : Apdu, ICloneable
    {
        private ResponseApdu() {}

        /// <summary>Initializes a new instance of the <see cref="ResponseApdu" /> class.</summary>
        /// <param name="response">The response as byte array that shall be parsed.</param>
        /// <param name="isoCase">The ISO case that was used when sending the <see cref="CommandApdu" />.</param>
        /// <param name="protocol">The communication protocol.</param>
        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol) {
            FullApdu = response;

            if (response != null) {
                Length = response.Length;
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
                Length = response.Length;
                FullApdu = new byte[Length];
                Array.Copy(response, FullApdu, Length);
            } else {
                FullApdu = response;
                Length = response.Length;
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
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            FullApdu = response;
            Length = length;
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
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (copy) {
                if (response != null) {
                    FullApdu = new byte[length];
                    Array.Copy(response, FullApdu, length);
                }
            } else {
                FullApdu = response;
            }
            Length = length;
            Case = isoCase;
            Protocol = protocol;
        }

        /// <summary>Gets a value indicating whether this response has data.</summary>
        /// <value>
        ///     <c>true</c> if this response has data; otherwise, <c>false</c>.</value>
        public bool HasData => FullApdu != null && FullApdu.Length > 2 && Length > 2;

        /// <summary>Indicates if the response APDU is valid.</summary>
        /// <value>
        ///     <see langword="true" /> if the response APDU is valid.</value>
        public override bool IsValid {
            get {
                if (FullApdu == null) {
                    return false;
                }
                return FullApdu.Length >= 2 && Length >= 2;
            }
        }

        /// <summary>Gets the SW1 status byte.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte SW1 {
            get {
                if (FullApdu == null || FullApdu.Length < Length || Length <= 1) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return FullApdu[Length - 2];
            }
        }

        /// <summary>Gets the SW1 status byte.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte SW2 {
            get {
                if (FullApdu == null || FullApdu.Length < Length || Length <= 0) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return FullApdu[Length - 1];
            }
        }

        /// <summary>Gets the combination of SW1 and SW2 as 16bit status word.</summary>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public int StatusWord => (SW1 << 8) | SW2;

        /// <summary>
        /// Gets the length of the response APDU
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        public int DataSize {
            get {
                if (FullApdu == null || FullApdu.Length <= 2 || Length <= 2) {
                    return 0;
                }
                return Length - 2;
            }
        }

        /// <summary>
        /// Gets the full response APDU.
        /// </summary>
        /// <value>
        /// The full APDU as byte array.
        /// </value>
        public byte[] FullApdu { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <exception cref="InvalidApduException">The response APDU is invalid.</exception>
        public byte[] GetData() {
            if (FullApdu == null) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            if (FullApdu.Length <= 2 ||
                Length <= 2) {
                return null;
            }

            var tmp = new byte[Length - 2];
            Array.Copy(FullApdu, tmp, Length - 2);
            return tmp;
        }

        /// <summary>Converts the APDU structure to a transmittable byte array.</summary>
        /// <returns>A byte array containing the APDU parameters and data in the correct order.</returns>
        public override byte[] ToArray() {
            if (FullApdu == null) {
                return null;
            }

            if (FullApdu.Length < Length) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            var tmp = new byte[Length];
            Array.Copy(FullApdu, tmp, Length);
            return tmp;
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>A clone of the current instance.</returns>
        public virtual object Clone() {
            var tmp = new ResponseApdu {
                FullApdu = FullApdu,
                Length = Length
            };
            return tmp;
        }
    }
}