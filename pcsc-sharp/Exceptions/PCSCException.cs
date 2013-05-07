using System;

namespace PCSC
{
    public class PCSCException : Exception
    {
        public SCardError SCardError { get; protected set; }

        public PCSCException(SCardError serr)
            : base(SCardHelper.StringifyError(serr)) {
            SCardError = serr;
        }

        public PCSCException(SCardError serr, string message)
            : base(message) {
            SCardError = serr;
        }

        public PCSCException(SCardError serr, string message, Exception innerEx)
            : base(message, innerEx) {
            SCardError = serr;
        }
    }
}