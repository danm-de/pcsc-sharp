using System;

namespace PCSC
{
    public class InvalidContextException : PCSCException
    {
        public InvalidContextException(SCardError serr)
            : base(serr, SCardHelper.StringifyError(serr)) {}

        public InvalidContextException(SCardError serr, string message)
            : base(serr, message) {}

        public InvalidContextException(SCardError serr, string message, Exception innerEx)
            : base(serr, message, innerEx) {}
    }
}
