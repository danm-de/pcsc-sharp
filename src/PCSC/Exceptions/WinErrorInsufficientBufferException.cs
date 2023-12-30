using System;

namespace PCSC.Exceptions
{
    /// <summary>
    /// A supplied buffer is insufficient.
    /// </summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class WinErrorInsufficientBufferException : InsufficientBufferException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WinErrorInsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public WinErrorInsufficientBufferException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinErrorInsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public WinErrorInsufficientBufferException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinErrorInsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public WinErrorInsufficientBufferException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WinErrorInsufficientBufferException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
