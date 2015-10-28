using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// A supplied buffer is insufficient.
	/// </summary>
	[Serializable]
	public class InsufficientBufferException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InsufficientBufferException(SCardError serr)
            : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public InsufficientBufferException(SCardError serr, string message)
            : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBufferException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public InsufficientBufferException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InsufficientBufferException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
