using System;

namespace PCSC
{
    public class UnsupportedFeatureException : PCSCException
    {
        public UnsupportedFeatureException(SCardError serr)
            : base(serr) {}

        public UnsupportedFeatureException(SCardError serr, string message)
            : base(serr, message) {}

        public UnsupportedFeatureException(SCardError serr, string message, Exception innerException)
            : base(serr, message, innerException) {}
    }
}
