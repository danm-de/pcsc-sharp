using System;

namespace PCSC
{
    public class NoSmartcardException : PCSCException
    {
        public NoSmartcardException(SCardError serr)
            : base(serr) {}

        public NoSmartcardException(SCardError serr, string message)
            : base(serr, message) {}

        public NoSmartcardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
