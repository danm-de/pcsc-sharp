using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// The share mode is invalid.
	/// </summary>
	[Serializable]
	public class InvalidShareModeException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidShareModeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public InvalidShareModeException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidShareModeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public InvalidShareModeException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidShareModeException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidShareModeException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InvalidShareModeException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
