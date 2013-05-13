using System;

namespace PCSC
{
    /// <summary>
    /// PC/SC exception type.
    /// </summary>
    public class PCSCException : Exception
    {
        /// <summary>
        /// The system's error code.
        /// </summary>
        public SCardError SCardError { get; protected set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public PCSCException(SCardError serr)
            : base(SCardHelper.StringifyError(serr)) {
            SCardError = serr;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public PCSCException(SCardError serr, string message)
            : base(message) {
            SCardError = serr;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PCSCException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public PCSCException(SCardError serr, string message, Exception innerException)
            : base(message, innerException) {
            SCardError = serr;
        }
    }
}