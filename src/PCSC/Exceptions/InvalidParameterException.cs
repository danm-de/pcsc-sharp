using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// One or more arguments contain invalid parameters.
	/// </summary>
	[Serializable]
	public class InvalidParameterException : PCSCException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidParameterException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        public InvalidParameterException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public InvalidParameterException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InvalidParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
