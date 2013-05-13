using System;

namespace PCSC
{
    /// <summary>
    /// Invalid PC/SC context exception.
    /// </summary>
    public class InvalidContextException : PCSCException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidContextException(SCardError serr)
            : base(serr, SCardHelper.StringifyError(serr)) {}
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        public InvalidContextException(SCardError serr, string message)
            : base(serr, message) {}
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public InvalidContextException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
