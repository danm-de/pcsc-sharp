using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// A smart card has been removed.
	/// </summary>
	[Serializable]
	public class RemovedCardException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public RemovedCardException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public RemovedCardException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedCardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public RemovedCardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected RemovedCardException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
