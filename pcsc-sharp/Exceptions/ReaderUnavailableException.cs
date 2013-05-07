using System;

namespace PCSC
{
    public class ReaderUnavailableException : PCSCException
    {
        public ReaderUnavailableException(SCardError serr)
            : base(serr) {}

        public ReaderUnavailableException(SCardError serr, string message)
            : base(serr, message) {}

        public ReaderUnavailableException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
