using System;

namespace PCSC
{
    /// <summary>
    /// An internal error occurred.
    /// </summary>
    public class InternalErrorException: PCSCException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InternalErrorException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        public InternalErrorException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public InternalErrorException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
