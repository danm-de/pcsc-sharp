using System;
using System.Diagnostics;

namespace PCSC.Iso7816
{
    /// <summary>The APDU is invalid.</summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class InvalidApduException : Exception
    {
        [NonSerialized]
        private readonly Apdu _apdu;

        /// <summary>
        /// APDU bytes
        /// </summary>
        public byte[] ApduBytes { get; }

        /// <summary>
        /// APDU
        /// </summary>
        public Apdu Apdu => _apdu;

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        public InvalidApduException(string message)
            : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidApduException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        /// <param name="apdu">The APDU.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidApduException(string message, Apdu apdu, Exception innerException)
            : base(message, innerException) {
            _apdu = apdu;
            try {
                ApduBytes = apdu.ToArray();
            } catch (Exception exception) {
                Trace.TraceError("Could not get APDU bytes: {0}", exception);
            }
        }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApduException" /> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidApduException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
