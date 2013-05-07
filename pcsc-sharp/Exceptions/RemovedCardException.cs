using System;

namespace PCSC
{
    public class RemovedCardException : PCSCException
    {
        public RemovedCardException(SCardError serr)
            : base(serr) {}

        public RemovedCardException(SCardError serr, string message)
            : base(serr, message) {}

        public RemovedCardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
