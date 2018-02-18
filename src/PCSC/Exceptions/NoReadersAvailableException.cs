using System;
using System.Runtime.Serialization;

namespace PCSC
{
	/// <summary>
	/// The group does not contain any reader
	/// </summary>
	[Serializable]
	public class NoReadersAvailableException : PCSCException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		public NoReadersAvailableException(SCardError serr) : base(serr)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		/// <param name="message">An error message text.</param>
		public NoReadersAvailableException(SCardError serr, string message) : base(serr, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
		/// </summary>
		/// <param name="serr">System's error code</param>
		/// <param name="message">An error message text.</param>
		/// <param name="innerException">The inner exception.</param>
		public NoReadersAvailableException(SCardError serr, string message, Exception innerException) : base(serr, message, innerException)
		{
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NoReadersAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}