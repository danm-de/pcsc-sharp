using System;

namespace PCSC
{
    public class InvalidValueException : PCSCException
    {
        public InvalidValueException(SCardError serr)
            : base(serr) {}

        public InvalidValueException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidValueException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
