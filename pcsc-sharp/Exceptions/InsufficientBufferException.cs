using System;

namespace PCSC
{
    /// <summary>
    /// A supplied buffer is insufficient.
    /// </summary>
    public class InsufficientBufferException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InsufficientBufferException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public InsufficientBufferException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public InsufficientBufferException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
