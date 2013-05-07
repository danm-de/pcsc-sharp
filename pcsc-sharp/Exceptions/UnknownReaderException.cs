using System;

namespace PCSC
{
    public class UnknownReaderException : PCSCException
    {
        public UnknownReaderException(SCardError serr)
            : base(serr) {}

        public UnknownReaderException(SCardError serr, string message)
            : base(serr, message) {}

        public UnknownReaderException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
