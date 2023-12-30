﻿using System;

namespace PCSC.Exceptions
{
    /// <summary>
    /// The group does not contain any reader
    /// </summary>
#if NETSTANDARD2_0 || NET6_0 || NET7_0
    [Serializable]
#endif
    public class NoReadersAvailableException : PCSCException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        public NoReadersAvailableException(SCardError serr) : base(serr) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        public NoReadersAvailableException(SCardError serr, string message) : base(serr, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoReadersAvailableException"/> class.
        /// </summary>
        /// <param name="serr">System's error code</param>
        /// <param name="message">An error message text.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoReadersAvailableException(SCardError serr, string message, Exception innerException) : base(serr,
            message, innerException) { }

#if NETSTANDARD2_0 || NET6_0 || NET7_0
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoReadersAvailableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :
            base(info, context) { }
#endif
    }
}
