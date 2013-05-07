using System;

namespace PCSC
{
    public class NoServiceException : PCSCException
    {
        public NoServiceException(SCardError serr)
            : base(serr) {}

        public NoServiceException(SCardError serr, string message)
            : base(serr, message) {}

        public NoServiceException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
