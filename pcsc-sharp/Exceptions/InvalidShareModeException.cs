using System;

namespace PCSC
{
    public class InvalidShareModeException : PCSCException
    {
        public InvalidShareModeException(SCardError serr)
            : base(serr) {}

        public InvalidShareModeException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidShareModeException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
