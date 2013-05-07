using System;

namespace PCSC
{
    public class InsufficientBufferException : PCSCException
    {
        public InsufficientBufferException(SCardError serr)
            : base(serr) { }
        public InsufficientBufferException(SCardError serr, string message)
            : base(serr, message) { }
        public InsufficientBufferException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
