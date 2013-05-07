using System;

namespace PCSC
{
    public class InternalErrorException: PCSCException
    {
        public InternalErrorException(SCardError serr)
            : base(serr) { }
        public InternalErrorException(SCardError serr, string message)
            : base(serr, message) { }
        public InternalErrorException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
