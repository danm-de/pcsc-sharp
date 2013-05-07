using System;

namespace PCSC
{
    public class UnresponsiveCardException : PCSCException
    {
        public UnresponsiveCardException(SCardError serr)
            : base(serr) {}

        public UnresponsiveCardException(SCardError serr, string message)
            : base(serr, message) {}

        public UnresponsiveCardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
