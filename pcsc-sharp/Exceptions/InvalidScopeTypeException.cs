using System;

namespace PCSC
{
    /// <summary>
    /// Invalid PC/SC scope exception.
    /// </summary>
    public class InvalidScopeTypeException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScopeTypeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidScopeTypeException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScopeTypeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public InvalidScopeTypeException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScopeTypeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidScopeTypeException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
