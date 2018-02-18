using System;
using System.Runtime.Serialization;

namespace PCSC
{
    /// <summary>
    /// The requested feature is unsupported.
    /// </summary>
    public class UnsupportedFeatureException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedFeatureException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public UnsupportedFeatureException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedFeatureException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public UnsupportedFeatureException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedFeatureException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnsupportedFeatureException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected UnsupportedFeatureException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
