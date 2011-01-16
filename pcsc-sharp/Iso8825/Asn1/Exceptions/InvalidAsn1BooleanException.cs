using System;
using System.Collections.Generic;
using System.Text;

namespace PCSC.Iso8825.Asn1
{
    public class InvalidAsn1BooleanException : Exception
    {
        public InvalidAsn1BooleanException()
            : base() { }
        public InvalidAsn1BooleanException(string message)
            : base(message) { }
        public InvalidAsn1BooleanException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
