using System;

namespace PCSC
{
    public class InvalidScopeTypeException : PCSCException
    {
        public InvalidScopeTypeException(SCardError serr)
            : base(serr) {}

        public InvalidScopeTypeException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidScopeTypeException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
