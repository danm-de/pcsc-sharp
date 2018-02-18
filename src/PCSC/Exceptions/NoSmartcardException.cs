using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// No smart card is currently inserted.
	/// </summary>
	[Serializable]
	public class NoSmartcardException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public NoSmartcardException(SCardError serr)
            : base(serr) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public NoSmartcardException(SCardError serr, string message)
            : base(serr, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSmartcardException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoSmartcardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NoSmartcardException(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }
    }
}
