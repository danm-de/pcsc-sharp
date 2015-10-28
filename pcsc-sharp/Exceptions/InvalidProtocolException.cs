using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// An invalid protocol has been requested.
	/// </summary>
	[Serializable]
	public class InvalidProtocolException : PCSCException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidProtocolException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidProtocolException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidProtocolException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public InvalidProtocolException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidProtocolException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidProtocolException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InvalidProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
