using System;

namespace PCSC
{
    public class CommunicationErrorException : PCSCException
    {
        public CommunicationErrorException(SCardError serr)
            : base(serr) { }
        public CommunicationErrorException(SCardError serr, string message)
            : base(serr, message) { }
        public CommunicationErrorException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) { }
    }
}
