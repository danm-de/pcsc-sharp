using System;

namespace PCSC
{
    /// <summary>
    /// The reader or the smart card is not ready.
    /// </summary>
    public class NotReadyException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public NotReadyException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public NotReadyException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotReadyException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
