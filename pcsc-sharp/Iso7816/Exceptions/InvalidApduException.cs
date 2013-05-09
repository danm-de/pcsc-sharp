using System;

namespace PCSC.Iso7816
{
    internal class InvalidApduException : Exception
    {
        private readonly Apdu _apdu;

        public InvalidApduException(string message)
            : base(message) {}

        public InvalidApduException(string message, Exception innerException)
            : base(message, innerException) {}

        public InvalidApduException(string message, Apdu apdu, Exception innerException)
            : base(message, innerException) {
            _apdu = apdu;
        }

        public Apdu InvalidApdu {
            get { return _apdu; }
        }
    }
}
