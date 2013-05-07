using System;

namespace PCSC
{
    public class SharingViolationException : PCSCException
    {
        public SharingViolationException(SCardError serr)
            : base(serr) {}

        public SharingViolationException(SCardError serr, string message)
            : base(serr, message) {}

        public SharingViolationException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
