using System;
using System.Runtime.Serialization;

namespace PCSC
{
    /// <summary>
    /// The smart card is unresponsive.
    /// </summary>
    public class UnresponsiveCardException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnresponsiveCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public UnresponsiveCardException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresponsiveCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public UnresponsiveCardException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresponsiveCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnresponsiveCardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected UnresponsiveCardException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
