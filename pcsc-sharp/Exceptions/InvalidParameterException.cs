using System;

namespace PCSC
{
    public class InvalidParameterException : PCSCException
    {
        public InvalidParameterException(SCardError serr)
            : base(serr) {}

        public InvalidParameterException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidParameterException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
