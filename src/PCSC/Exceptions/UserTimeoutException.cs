using System;
using System.Runtime.Serialization;

namespace PCSC.Exceptions
{
    /// <summary>
    /// The user-specified time-out value has expired.
    /// </summary>
    [Serializable]
    public class UserTimeoutException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTimeoutException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public UserTimeoutException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTimeoutException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public UserTimeoutException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTimeoutException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public UserTimeoutException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UserTimeoutException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}
