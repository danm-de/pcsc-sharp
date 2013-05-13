using System;

namespace PCSC
{
    /// <summary>
    /// One or more arguments contain invalid parameters.
    /// </summary>
    public class InvalidParameterException : PCSCException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidParameterException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        public InvalidParameterException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public InvalidParameterException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
