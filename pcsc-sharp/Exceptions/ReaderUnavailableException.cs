using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// The requested reader is unavailable.
	/// </summary>
	[Serializable]
	public class ReaderUnavailableException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderUnavailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public ReaderUnavailableException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderUnavailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public ReaderUnavailableException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderUnavailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public ReaderUnavailableException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ReaderUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
