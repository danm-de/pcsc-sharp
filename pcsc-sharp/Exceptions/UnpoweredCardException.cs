using System;

namespace PCSC
{
    public class UnpoweredCardException : PCSCException
    {
        public UnpoweredCardException(SCardError serr)
            : base(serr) {}

        public UnpoweredCardException(SCardError serr, string message)
            : base(serr, message) {}

        public UnpoweredCardException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
