using System;

namespace PCSC
{
    public class InvalidProtocolException : PCSCException
    {
        public InvalidProtocolException(SCardError serr)
            : base(serr) {}

        public InvalidProtocolException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidProtocolException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
