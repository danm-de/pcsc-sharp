using System;

namespace PCSC.Exceptions
{
    /// <summary>
    /// Operation exited successfully
    /// </summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class SuccessException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public SuccessException(SCardError serr) : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public SuccessException(SCardError serr, string message) : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public SuccessException(SCardError serr, string message, Exception innerException) : base(serr, message,
            innerException) { }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SuccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
