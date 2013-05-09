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

        protected IsoCase isocase;
        public virtual IsoCase Case {
            get { return isocase; }
            protected set { isocase = value; }
        }

        protected SCardProtocol proto;
        public virtual SCardProtocol Protocol {
            get { return proto; }
            protected set { proto = value; }
        }

        public abstract byte[] ToArray();

        public abstract bool IsValid { get; }

        public static explicit operator byte[](Apdu apdu) {
            return apdu.ToArray();
        }
    }
}
