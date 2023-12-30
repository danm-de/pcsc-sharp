using System;

namespace PCSC.Exceptions
{
    /// <summary>
    /// No smart card is currently inserted.
    /// </summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class NoSmartcardException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public NoSmartcardException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public NoSmartcardException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoSmartcardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoSmartcardException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
