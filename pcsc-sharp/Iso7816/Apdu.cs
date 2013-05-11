namespace PCSC.Iso7816
{
    // The Application Protocol Data Unit (APDU), defined by the ISO/IEC 7816 standards
    public abstract class Apdu
    {
        /************************************************
         * See http://de.wikipedia.org/wiki/APDU        *
         ************************************************
         * Command APDU case 4                          *
         ************************************************
         * CLA INS P1 P2 Lc Data Le                     *
         ************************************************/

        /************************************************
         * Command APDU case 3                          *
         ************************************************
         * CLA INS P1 P2 Lc Data                        *
         ************************************************/

        /************************************************
         * Command APDU case 2                          *
         ************************************************
         * CLA INS P1 P2 Le                             *
         ************************************************/

        /************************************************
         * Command APDU case 1                          *
         ************************************************
         * CLA INS P1 P2                                *
         ************************************************/

        private IsoCase _isoCase;
        private SCardProtocol _protocol;
        
        public IsoCase Case {
            get { return _isoCase; }
            protected set { _isoCase = value; }
        }

        public SCardProtocol Protocol {
            get { return _protocol; }
            protected set { _protocol = value; }
        }

        public abstract byte[] ToArray();

        public abstract bool IsValid { get; }

        public static explicit operator byte[](Apdu apdu) {
            return apdu.ToArray();
        }
    }
}
