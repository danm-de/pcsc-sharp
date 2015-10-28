using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// Operation exited successfully
	/// </summary>
	[Serializable]
	public class SuccessException : PCSCException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SuccessException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		public SuccessException(SCardError serr) : base(serr)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuccessException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		/// <param name="message">An error message text.</param>
		public SuccessException(SCardError serr, string message) : base(serr, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuccessException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		/// <param name="message">An error message text.</param>
		/// <param name="innerException">The inner exception.</param>
		public SuccessException(SCardError serr, string message, Exception innerException) : base(serr, message, innerException)
		{
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SuccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}