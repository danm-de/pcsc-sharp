using System;

namespace PCSC.Iso7816
{
    /// <summary>The APDU is invalid.</summary>
    internal class InvalidApduException : Exception
    {
        private readonly Apdu _apdu;

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        public InvalidApduException(string message)
            : base(message) {}

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidApduException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>Initializes a new instance of the <see cref="InvalidApduException" /> class.</summary>
        /// <param name="message">The error message.</param>
        /// <param name="apdu">The APDU.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidApduException(string message, Apdu apdu, Exception innerException)
            : base(message, innerException) {
            _apdu = apdu;
        }

        /// <summary>Gets the invalid APDU.</summary>
        /// <value>The invalid APDU.</value>
        public Apdu InvalidApdu {
            get { return _apdu; }
        }
    }
}