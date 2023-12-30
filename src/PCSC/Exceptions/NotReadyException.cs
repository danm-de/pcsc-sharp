using System;

namespace PCSC.Exceptions
{
    /// <summary>
    /// The reader or the smart card is not ready.
    /// </summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class NotReadyException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public NotReadyException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public NotReadyException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotReadyException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotReadyException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NotReadyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
