using System;

namespace PCSC
{
    /// <summary>
    /// A sharing violation occurred.
    /// </summary>
    public class SharingViolationException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharingViolationException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public SharingViolationException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SharingViolationException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public SharingViolationException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SharingViolationException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public SharingViolationException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
