using System;

namespace PCSC
{
    public class NotReadyException : PCSCException
    {
        public NotReadyException(SCardError serr)
            : base(serr) {}

        public NotReadyException(SCardError serr, string message)
            : base(serr, message) {}

        public NotReadyException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
