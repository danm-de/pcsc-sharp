using System;
using System.Runtime.Serialization;

namespace PCSC
{
    /// <summary>
    /// The requested reader name is unknown.
    /// </summary>
    public class UnknownReaderException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownReaderException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public UnknownReaderException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownReaderException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public UnknownReaderException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownReaderException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownReaderException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected UnknownReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
