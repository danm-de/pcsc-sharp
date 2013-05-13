using System;

namespace PCSC
{
    /// <summary>
    /// A communication error occurred.
    /// </summary>
    public class CommunicationErrorException : PCSCException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">Error number.</param>
        public CommunicationErrorException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        public CommunicationErrorException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception.</param>
        public CommunicationErrorException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
