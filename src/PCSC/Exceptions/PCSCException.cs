using System;
using System.Runtime.Serialization;

namespace PCSC
{
    /// <summary>
    /// A general PC/SC exception.
    /// </summary>
    [Serializable]
    public class PCSCException : Exception
    {
        /// <summary>
        /// Serialization name for property <see cref="SCardError"/>
        /// </summary>
	    protected const string SCARD_ERROR_SERIALIZATION_NAME = "SCardError";

	    /// <summary>
        /// The system's error code.
        /// </summary>
        public SCardError SCardError { get; protected set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public PCSCException(SCardError serr)
            : base(SCardHelper.StringifyError(serr)) {
            SCardError = serr;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public PCSCException(SCardError serr, string message)
            : base(message) {
            SCardError = serr;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PCSCException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public PCSCException(SCardError serr, string message, Exception innerException)
            : base(message, innerException) {
            SCardError = serr;
        }

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
	    protected PCSCException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
	    {
		    SCardError = (SCardError)info.GetValue(SCARD_ERROR_SERIALIZATION_NAME, typeof (SCardError));
	    }

	    /// <summary>
		/// Gets data for serialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
	    public override void GetObjectData(SerializationInfo info, StreamingContext context)
	    {
		    base.GetObjectData(info, context);
		    info.AddValue(SCARD_ERROR_SERIALIZATION_NAME, SCardError, typeof (SCardError));
	    }
    }
}